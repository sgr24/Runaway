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
        Ctx.Rb.useGravity = false;  // Use Rigidbody's useGravity
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
        Ctx.Rb.useGravity = true;  // Use Rigidbody's useGravity
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
        Ctx.Rb.useGravity = Ctx.UseGravity;

        Vector3 wallNormal = wallRight ? Ctx.RightWallHit.normal : Ctx.LeftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        if ((Ctx.Orientation.forward - wallForward).magnitude > (Ctx.Orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        Ctx.Rb.AddForce(wallForward * Ctx.WallRunForce, ForceMode.Force);

        if (Ctx.UpwardsRunning)
        {
            Ctx.Rb.velocity = new Vector3(Ctx.Rb.velocity.x, Ctx.WallClimbSpeed, Ctx.Rb.velocity.z);
        }
        else if (Ctx.DownwardsRunning)
        {
            Ctx.Rb.velocity = new Vector3(Ctx.Rb.velocity.x, -Ctx.WallClimbSpeed, Ctx.Rb.velocity.z);
        }

        if (!(wallLeft && Ctx.HorizontalInput > 0) && !(wallRight && Ctx.HorizontalInput < 0))
        {
            Ctx.Rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        if (Ctx.UseGravity)
        {
            Ctx.Rb.AddForce(Vector3.up * Ctx.GravityCounterForce, ForceMode.Force);
        }
    }

    private void CheckForWall()
    {
        RaycastHit leftWallHit;
        RaycastHit rightWallHit;

        wallRight = Physics.Raycast(Ctx.transform.position, Ctx.Orientation.right, out rightWallHit, Ctx.WallCheckDistance, Ctx.WhatIsWall);
        wallLeft = Physics.Raycast(Ctx.transform.position, -Ctx.Orientation.right, out leftWallHit, Ctx.WallCheckDistance, Ctx.WhatIsWall);

        Ctx.SetWallHits(leftWallHit, rightWallHit);
    }

}
