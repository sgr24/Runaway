using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallRunState : PlayerBaseState
{
    public PlayerWallRunState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        // Set animator parameters for wall running
        Ctx.Animator.SetBool(Ctx.IsWallRunningHash, true);
        // Modify movement speed for wall running
        Ctx.AppliedMovementX *= Ctx.WallRunSpeedMultiplier;
        Ctx.AppliedMovementY = 0;  // Example: neutralize gravity
    }

    public override void UpdateState()
    {
        // Check for state transitions
        CheckSwitchStates();
        // Handle wall running logic
        HandleWallRunning();
    }

    public override void ExitState()
    {
        // Reset animator parameters when exiting wall running state
        Ctx.Animator.SetBool(Ctx.IsWallRunningHash, false);
    }

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        // Transition to grounded state if wall run input is released
        if (!Ctx.IsWallRunPressed)
        {
            SwitchState(Factory.Grounded());
        }
    }

    private void HandleWallRunning()
    {
        // Implement wall running logic here
    }
}
