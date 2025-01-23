using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runaway.FinalCharacterController
{
    public class PlayerState : MonoBehaviour
    {
        [field: SerializeField] public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idle;

        public void SetPlayerMovementState(PlayerMovementState playerMovementState)
        {
            CurrentPlayerMovementState = playerMovementState;
        }

        // Moved the method inside the PlayerState class to access CurrentPlayerMovementState
        public bool InGroundedState()
        {
            return CurrentPlayerMovementState == PlayerMovementState.Idle || 
                   CurrentPlayerMovementState == PlayerMovementState.Walking || 
                   CurrentPlayerMovementState == PlayerMovementState.Running;
        }
    }

    public enum PlayerMovementState
    {
        Idle = 0,
        Walking = 1,
        Running = 2,
        Jumping = 3,
        Falling = 4,
        Crouching = 5,
        WallRunning = 6,
        Climbing = 7,
    }
}
