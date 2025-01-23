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
        [SerializeField] private Animator playerAnimator;
        [SerializeField] private float runSpeedMultiplier = 2f;
        [SerializeField] private bool holdToCrouch = true;
        [SerializeField] private float runAcceleration = 20f;
        [SerializeField] private float walkSpeed = 5f;

        public bool RunToggledOn { get; private set; }
        public PlayerControls PlayerControls { get; private set; }
        public Vector2 MovementInput { get; private set; }
        public Vector2 LookInput { get; private set; }

        private float currentSpeed = 0f;
        private float verticalVelocity;
        private Vector3 currentMovement;
        private Vector3 appliedMovement;
        private bool isMovementPressed;
        private bool isRunPressed;

        // Jumping variables
        private bool isJumpPressed = false;
        private float initialJumpVelocity;
        private float maxJumpHeight = 4.0f;
        private float maxJumpTime = 0.75f;
        private bool isJumping = false;
        private int jumpCount = 0;
        private Dictionary<int, float> initialJumpVelocities = new Dictionary<int, float>();
        private Dictionary<int, float> jumpGravities = new Dictionary<int, float>();
        private Coroutine currentJumpResetRoutine = null;

        // Rotation speed
        private float rotationFactorPerFrame = 20.0f;
        private int zero = 0;

        private void SetupJumpVariables()
        {
            float timeToApex = maxJumpTime / 2;
            float gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
            initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;

            float secondJumpGravity = (-2 * (maxJumpHeight + 2)) / Mathf.Pow((timeToApex * 1.25f), 2);
            float secondJumpInitialVelocity = (2 * (maxJumpHeight + 2)) / (timeToApex * 1.25f);

            float thirdJumpGravity = (-2 * (maxJumpHeight + 4)) / Mathf.Pow((timeToApex * 1.5f), 2);
            float thirdJumpInitialVelocity = (2 * (maxJumpHeight + 4)) / (timeToApex * 1.5f);

            initialJumpVelocities.Add(1, initialJumpVelocity);
            initialJumpVelocities.Add(2, secondJumpInitialVelocity);
            initialJumpVelocities.Add(3, thirdJumpInitialVelocity);

            jumpGravities.Add(0, gravity);
            jumpGravities.Add(1, gravity);
            jumpGravities.Add(2, secondJumpGravity);
            jumpGravities.Add(3, thirdJumpGravity);
        }

        private void OnEnable()
        {
            PlayerControls = new PlayerControls();
            PlayerControls.Enable();
            PlayerControls.PlayerLocomotionMap.Enable();
            PlayerControls.PlayerLocomotionMap.SetCallbacks(this);

            SetupJumpVariables();
        }

        private void OnDisable()
        {
            PlayerControls.PlayerLocomotionMap.Disable();
            PlayerControls.PlayerLocomotionMap.RemoveCallbacks(this);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            MovementInput = context.ReadValue<Vector2>();
            playerAnimator.SetFloat("inputX", MovementInput.x);
            playerAnimator.SetFloat("inputY", MovementInput.y);

            // Apply movement logic
            isMovementPressed = MovementInput.x != zero || MovementInput.y != zero;
            MoveCharacter();
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
            isJumpPressed = context.ReadValueAsButton();
            if (isJumpPressed && characterController.isGrounded)
            {
                Jump();
            }
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
            playerAnimator.SetBool("isCrouching", holdToCrouch && context.performed);
        }

        public void OnClimb(InputAction.CallbackContext context)
        {
            playerAnimator.SetBool("isClimbing", context.performed);
        }

        // Method to handle looking around
        public void OnLook(InputAction.CallbackContext context)
        {
            LookInput = context.ReadValue<Vector2>();
            RotateCharacter();
        }

        private void MoveCharacter()
        {
            Vector3 move = new Vector3(MovementInput.x, 0, MovementInput.y);
            move = transform.TransformDirection(move);

            // Apply acceleration based on running or walking
            if (RunToggledOn)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, runSpeedMultiplier, runAcceleration * Time.deltaTime);
            }
            else
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, walkSpeed, runAcceleration * Time.deltaTime);
            }

            // Apply gravity
            if (!characterController.isGrounded)
            {
                verticalVelocity -= 9.81f * Time.deltaTime; // Simple gravity
            }
            else
            {
                verticalVelocity = 0; // Reset when grounded
            }

            move.y = verticalVelocity; // Apply vertical velocity to the movement

            // Move the character
            appliedMovement = move * currentSpeed * Time.deltaTime;
            characterController.Move(appliedMovement);
        }

        // Character rotation logic
        private void RotateCharacter()
        {
            Vector3 positionToLookAt = new Vector3(MovementInput.x, 0, MovementInput.y);
            if (isMovementPressed)
            {
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
            }
        }

        private void Jump()
        {
            if (jumpCount < 3) // Limit jumps to 3 (for example)
            {
                playerAnimator.SetTrigger("jump");
                verticalVelocity = initialJumpVelocities[jumpCount + 1];
                jumpCount++;

                // Reset jump after a brief delay if needed
                if (currentJumpResetRoutine != null)
                {
                    StopCoroutine(currentJumpResetRoutine);
                }
                currentJumpResetRoutine = StartCoroutine(ResetJumpCount());
            }
        }

        private IEnumerator ResetJumpCount()
        {
            yield return new WaitForSeconds(maxJumpTime);
            jumpCount = 0;
        }
    }
}
