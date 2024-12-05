using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbState : PlayerBaseState
{
    public PlayerClimbState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        // Set animator parameters for climbing
        Ctx.Animator.SetBool(Ctx.IsClimbingHash, true);
        Ctx.LastWall = null; // Reset last wall for climbing logic
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleClimbing();
    }

    public override void ExitState()
    {
        // Reset animator parameters when exiting climbing state
        Ctx.Animator.SetBool(Ctx.IsClimbingHash, false);
    }

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsClimbPressed || !Ctx.WallFront || Ctx.ExitingWall)
        {
            SwitchState(Factory.Grounded());
        }
    }

    private void HandleClimbing()
    {
        if (Ctx.IsClimbPressed && Ctx.WallFront)
        {
            Vector3 climbMovement = new Vector3(0, Ctx.ClimbSpeed * Time.deltaTime, 0);
            Ctx.CharacterController.Move(climbMovement);

            // Handle climb jump
            if (Ctx.IsJumpPressed && Ctx.ClimbJumpsLeft > 0)
            {
                ClimbJump();
            }
        }
    }

    private void ClimbJump()
    {
        Ctx.ExitingWall = true;
        Ctx.ExitWallTimer = Ctx.ExitWallTime;

        Vector3 forceToApply = Vector3.up * Ctx.ClimbJumpUpForce + Ctx.FrontWallHit.normal * Ctx.ClimbJumpBackForce;
        Ctx.CharacterController.Move(forceToApply * Time.deltaTime);

        Ctx.ClimbJumpsLeft--;
    }
}
