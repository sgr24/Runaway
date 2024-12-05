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
        // Set climbing speed
        Ctx.AppliedMovementY = Ctx.ClimbSpeed;  // Example: climbing speed
    }

    public override void UpdateState()
    {
        // Check for state transitions
        CheckSwitchStates();
        // Handle climbing logic
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
        // Transition to grounded state if climb input is released
        if (!Ctx.IsClimbPressed)
        {
            SwitchState(Factory.Grounded());
        }
    }

    private void HandleClimbing()
    {
        // Implement climbing logic here
    }
}

