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
            playerAnimator.SetFloat("inputX", MovementInput.x);
            playerAnimator.SetFloat("inputY", MovementInput.y);
            print(MovementInput);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector3 move = new Vector3(MovementInput.x, 0, MovementInput.y);
            move = transform.TransformDirection(move);
            float speed = RunToggledOn ? runSpeed : walkSpeed;
            characterController.Move(move * speed * Time.deltaTime);
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                RunToggledOn = holdToRun || !RunToggledOn;
                playerAnimator.SetBool("isRunning", RunToggledOn);
            }
            else if (context.canceled)
            {
                RunToggledOn = !holdToRun && RunToggledOn;
                playerAnimator.SetBool("isRunning", RunToggledOn);
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            playerAnimator.SetTrigger("jump");
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            playerAnimator.SetBool("isCrouching", holdToCrouch && context.performed);
        }

        public void OnClimb(InputAction.CallbackContext context)
        {
            playerAnimator.SetBool("isClimbing", context.performed);
        }
    }
}
