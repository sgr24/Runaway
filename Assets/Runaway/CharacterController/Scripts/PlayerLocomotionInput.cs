using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runaway.FinalCharacterController
{
    [DefaultExecutionOrder(-2)]
    public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocomotionMapActions
    {
        [SerializeField] private bool holdToRun = true;
        [SerializeField] private CharacterController characterController;
        [SerializeField] Animator playerAnimator;
        [SerializeField] private float runSpeedMultiplier = 2f;
        [SerializeField] private bool holdToCrouch = true;
        [SerializeField] private float runAcceleration = 20f;
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private float walkSpeed = 5f;

        public bool RunToggledOn { get; private set; }
        public PlayerControls PlayerControls { get; private set; }
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }

        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();

            PlayerControls.PlayerLocomotionMap.Enable();
            PlayerControls.PlayerLocomotionMap.SetCallbacks(this);
        }

        private void OnDisable()
        {
            PlayerControls.PlayerLocomotionMap.Disable();
            PlayerControls.PlayerLocomotionMap.RemoveCallbacks(this);
        }

        public void OnMovement(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
            print(MovementInput);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            // Implement movement logic here
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                RunToggledOn = holdToRun || !RunToggledOn;
            }
            else if (context.canceled)
            {
                RunToggledOn = !holdToRun && RunToggledOn;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            // Implement jump logic here
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            // Implement crouch logic here
        }

        public void OnClimb(InputAction.CallbackContext context)
        {
            // Implement climb logic here
        }
    }
}
