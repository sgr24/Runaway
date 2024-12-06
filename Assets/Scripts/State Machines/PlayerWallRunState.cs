using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallRunState : PlayerBaseState
{
    private bool wallLeft;
    private bool wallRight;
    private float wallRunTimer;
    private bool exitingWall;
    private float exitWallTimer;

    public PlayerWallRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
        : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        Ctx.Animator.SetBool(Ctx.IsWallRunningHash, true);
        Ctx.AppliedMovementX *= Ctx.WallRunSpeedMultiplier;
        Ctx.AppliedMovementY = 0;
        Ctx.UseGravity = false;  // Disable gravity
        wallRunTimer = Ctx.MaxWallRunTime;
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleWallRunning();
        Ctx.CharacterController.Move(Ctx.AppliedMovement * Time.deltaTime);
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsWallRunningHash, false);
        Ctx.UseGravity = true;  // Enable gravity
    }

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        if (exitingWall)
        {
            exitWallTimer -= Time.deltaTime;
            if (exitWallTimer <= 0)
            {
                exitingWall = false;
            }
        }
        else
        {
            CheckForWall();
            if (!wallLeft && !wallRight || Ctx.CurrentMovementInput.y <= 0)
            {
                SwitchState(Factory.Grounded());
            }
        }
    }

    private void HandleWallRunning()
    {
        if (wallRunTimer > 0)
        {
            wallRunTimer -= Time.deltaTime;
        }
        else
        {
            exitingWall = true;
            exitWallTimer = Ctx.ExitWallTime;
        }

        WallRunningMovement();
    }

    private void WallRunningMovement()
    {
        Vector3 wallNormal = wallRight ? Ctx.RightWallHit.normal : Ctx.LeftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        if ((Ctx.transform.forward - wallForward).magnitude > (Ctx.transform.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        Ctx.AppliedMovement = wallForward * Ctx.WallRunForce * Time.deltaTime;

        if (Ctx.UpwardsRunning)
        {
            var movement = Ctx.AppliedMovement;
            movement.y = Ctx.WallClimbSpeed;
            Ctx.AppliedMovement = movement;
        }
        else if (Ctx.DownwardsRunning)
        {
            var movement = Ctx.AppliedMovement;
            movement.y = -Ctx.WallClimbSpeed;
            Ctx.AppliedMovement = movement;
        }

        if (!(wallLeft && Ctx.HorizontalInput > 0) && !(wallRight && Ctx.HorizontalInput < 0))
        {
            Ctx.AppliedMovement += -wallNormal * 100 * Time.deltaTime;
        }

        if (Ctx.UseGravity)
        {
            Ctx.AppliedMovement += Vector3.up * Ctx.GravityCounterForce * Time.deltaTime;
        }
    }

    private void CheckForWall()
    {
        RaycastHit leftWallHit;
        RaycastHit rightWallHit;

        wallRight = Physics.Raycast(Ctx.transform.position, Ctx.transform.right, out rightWallHit, Ctx.WallCheckDistance, Ctx.WhatIsWall);
        wallLeft = Physics.Raycast(Ctx.transform.position, -Ctx.transform.right, out leftWallHit, Ctx.WallCheckDistance, Ctx.WhatIsWall);

        Ctx.SetWallHits(leftWallHit, rightWallHit);
    }
}
