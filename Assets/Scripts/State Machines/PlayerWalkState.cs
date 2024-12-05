using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerBaseState
{
    private float walkSpeed = 5f; // Configurable walk speed

    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        // Set animator parameters for walking
        Ctx.Animator.SetBool(Ctx.IsWalkingHash, true);
        Ctx.Animator.SetBool(Ctx.IsRunningHash, false);
    }

    public override void UpdateState()
    {
        CheckSwitchStates();
        // Apply movement based on input
        Ctx.AppliedMovementX = Ctx.CurrentMovementInput.x * walkSpeed;
        Ctx.AppliedMovementZ = Ctx.CurrentMovementInput.y * walkSpeed;
    }

    public override void ExitState()
    {
        // Any cleanup logic if needed
    }

    public override void InitializeSubState() { }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMovementPressed)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && Ctx.IsRunPressed)
        {
            SwitchState(Factory.Run());
        }
    }
}
