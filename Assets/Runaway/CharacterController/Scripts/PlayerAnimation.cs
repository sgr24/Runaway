using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runaway.FinalCharacterController
{
    public class PlayerAnimationAndMovementController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float locomotionBlendSpeed = 4f;

        private CharacterController _characterController;
        private PlayerInput _playerInput;
        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");
        private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");

        private Vector3 _currentBlendInput = Vector3.zero;

        private Vector2 _currentMovementInput;
        private Vector3 _currentMovement;
        private Vector3 _currentRunMovement;
        private bool _isMovementPressed;
        private bool _isRunPressed;
        private bool _isJumpPressed;

        // Constants
        private float _rotationFactorPerFrame = 30.0f;
        private float _runMultiplier = 15.0f;
        private float _gravity = -9.8f;
        private float _groundedGravity = -0.05f;

        private void Awake()
        {
            _playerInput = new PlayerInput();
            _characterController = GetComponent<CharacterController>();
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
        }

        private void OnEnable()
        {
            _playerInput.CharacterControls.Enable();
            _playerInput.CharacterControls.Move.started += OnMovementInput;
            _playerInput.CharacterControls.Move.canceled += OnMovementInput;
            _playerInput.CharacterControls.Move.performed += OnMovementInput;
            _playerInput.CharacterControls.Run.started += OnRun;
            _playerInput.CharacterControls.Run.canceled += OnRun;
            _playerInput.CharacterControls.Jump.started += OnJump;
            _playerInput.CharacterControls.Jump.canceled += OnJump;
        }

        private void OnDisable()
        {
            _playerInput.CharacterControls.Disable();
            _playerInput.CharacterControls.Move.started -= OnMovementInput;
            _playerInput.CharacterControls.Move.canceled -= OnMovementInput;
            _playerInput.CharacterControls.Move.performed -= OnMovementInput;
            _playerInput.CharacterControls.Run.started -= OnRun;
            _playerInput.CharacterControls.Run.canceled -= OnRun;
            _playerInput.CharacterControls.Jump.started -= OnJump;
            _playerInput.CharacterControls.Jump.canceled -= OnJump;
        }

        private void OnMovementInput(InputAction.CallbackContext context)
        {
            _currentMovementInput = context.ReadValue<Vector2>();
            _currentMovement.x = _currentMovementInput.x;
            _currentMovement.z = _currentMovementInput.y;
            _currentRunMovement.x = _currentMovementInput.x * _runMultiplier;
            _currentRunMovement.z = _currentMovementInput.y * _runMultiplier;
            _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            _isRunPressed = context.ReadValueAsButton();
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            _isJumpPressed = context.ReadValueAsButton();
        }

        private void Update()
        {
            HandleGravity();
            HandleRotation();
            UpdateAnimationState();

            if (_isRunPressed)
            {
                _characterController.Move(_currentRunMovement * Time.deltaTime);
            }
            else
            {
                _characterController.Move(_currentMovement * Time.deltaTime);
            }
        }

        private void HandleRotation()
        {
            if (_isMovementPressed)
            {
                Vector3 positionToLookAt = new Vector3(_currentMovement.x, 0, _currentMovement.z);
                Quaternion currentRotation = transform.rotation;
                Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
                transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
            }
        }

        private void HandleGravity()
        {
            if (_characterController.isGrounded)
            {
                _currentMovement.y = _groundedGravity;
                _currentRunMovement.y = _groundedGravity;
            }
            else
            {
                _currentMovement.y += _gravity * Time.deltaTime;
                _currentRunMovement.y += _gravity * Time.deltaTime;
            }
        }

        private void UpdateAnimationState()
        {
            bool isRunning = _playerState.CurrentPlayerMovementState == PlayerMovementState.Running;

            Vector2 inputTarget = isRunning ? _currentMovementInput * 2f : _currentMovementInput;
            _currentBlendInput = Vector3.Lerp(_currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);

            _animator.SetFloat(inputXHash, _currentBlendInput.x);
            _animator.SetFloat(inputYHash, _currentBlendInput.y);
            _animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
        }
    }
}
