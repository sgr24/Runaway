using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runaway.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerController : MonoBehaviour
    {
        #region Class Variables
        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;

        [Header("Base Movement")]
        public float runAcceleration = 35f;
        public float runSpeed = 4f;
        public float walkAcceleration = 15f;
        public float walkSpeed = 2f;
        public float drag = 20f;
        public float movingThreshold = 0.1f;

        [Header("Camera Settings")]
        public float lookSenseH = 0.1f;
        public float lookSenseV = 0.1f;
        public float lookLimitV = 89f;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;

        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero;

        private Vector3 _currentVelocity;
        #endregion

        #region Startup
        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
        }
        #endregion

        #region Update Logic
        private void Update()
        {
            UpdateMovementState();
            HandleLateralMovement();
        }

        private void UpdateMovementState()
        {
            bool isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;
            bool isMovingLaterally = IsMovingLaterally();
            bool isRunning = _playerLocomotionInput.RunToggledOn && isMovingLaterally;

            PlayerMovementState lateralState = isRunning ? PlayerMovementState.Running : isMovingLaterally || isMovementInput ? PlayerMovementState.Walking : PlayerMovementState.Idle;

            _playerState.SetPlayerMovementState(lateralState);
        }

        private void HandleLateralMovement()
        {
            bool isRunning = _playerState.CurrentPlayerMovementState == PlayerMovementState.Running;

            float lateralAcceleration = isRunning ? runAcceleration : walkAcceleration;
            float clampLateralMagnitude = isRunning ? runSpeed : walkSpeed;

            Vector3 cameraForwardXZ = new Vector3(_playerCamera.transform.forward.x, 0f, _playerCamera.transform.forward.z).normalized;
            Vector3 cameraRightXZ = new Vector3(_playerCamera.transform.right.x, 0f, _playerCamera.transform.right.z).normalized;
            Vector3 movementDirection = cameraRightXZ * _playerLocomotionInput.MovementInput.x + cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

            Vector3 movementDelta = movementDirection * lateralAcceleration * Time.deltaTime;
            _currentVelocity += movementDelta;

            Vector3 currentDrag = _currentVelocity.normalized * drag * Time.deltaTime;
            _currentVelocity = (_currentVelocity.magnitude > drag * Time.deltaTime) ? _currentVelocity - currentDrag : Vector3.zero;
            _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, clampLateralMagnitude);

            _characterController.Move(_currentVelocity * Time.deltaTime);
        }
        #endregion

        #region Late Update Logic
        private void LateUpdate()
        {
            HandleCameraRotation();
        }

        private void HandleCameraRotation()
        {
            _cameraRotation.x += lookSenseH * _playerLocomotionInput.LookInput.x;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - lookSenseV * _playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

            transform.rotation = Quaternion.Euler(0f, _cameraRotation.x, 0f);
            _playerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);
        }
        #endregion

        #region State Checks
        private bool IsMovingLaterally()
        {
            Vector3 lateralVelocity = new Vector3(_currentVelocity.x, 0f, _currentVelocity.z);
            return lateralVelocity.magnitude > movingThreshold;
        }
        #endregion
    }
}
