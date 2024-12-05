using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchState : PlayerBaseState
{
    public PlayerCrouchState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        // Set animator parameters for crouching
        Ctx.Animator.SetBool(Ctx.IsCrouchingHash, true);
        // Modify movement speed for crouching
        Ctx.AppliedMovementX *= 0.5f;  // Example: half speed when crouching
        Ctx.AppliedMovementZ *= 0.5f;
    }

    public override void UpdateState()
    {
        // Check for state transitions
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        // Reset animator parameters when exiting crouching state
        Ctx.Animator.SetBool(Ctx.IsCrouchingHash, false);
    }

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        // Transition to grounded state if crouch input is released
        if (!Ctx.IsCrouchPressed)
        {
            SwitchState(Factory.Grounded());
        }
    }
}
