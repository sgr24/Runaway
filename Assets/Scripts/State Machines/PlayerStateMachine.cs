using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    // Declare reference variables
    CharacterController _characterController;
    Animator _animator;
    PlayerInput _playerInput;

    // Variables to store optimized setter/getter parameter IDs
    int _isWalkingHash;
    int _isRunningHash;
    int _isJumpingHash;
    int _jumpCountHash;
    int _isCrouchingHash;
    int _isClimbingHash;
    int _isWallRunningHash;

    // Variables to store player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _appliedMovement;
    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isCrouchPressed;
    bool _isClimbPressed;

    // Constants
    float _rotationFactorPerFrame = 13.0f;
    float _runMultiplier = 6.0f;
    int _zero = 0;

    // Gravity variables
    float _gravity = -9.8f;
    float _groundedGravity = -5f;

    // Jumping variables
    bool _isJumpPressed = false;
    float _initialJumpVelocity;
    float _maxJumpHeight = 4.0f;
    float _maxJumpTime = .50f;
    bool _isJumping = false;
    bool _requireNewJumpPress = false;
    int _jumpCount = 0;
    Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();
    Dictionary<int, float> _jumpGravities = new Dictionary<int, float>();
    Coroutine _currentJumpResetRoutine = null;

    // Wall Running Variables
    public RaycastHit LeftWallHit { get; set; }
    public RaycastHit RightWallHit { get; set; }
    public float WallRunForce;
    public float WallJumpUpForce;
    public float WallJumpSideForce;
    public float WallClimbSpeed;
    public float MaxWallRunTime;
    public float WallCheckDistance;
    public bool UseGravity;
    public float GravityCounterForce;
    public float WallRunSpeedMultiplier = 3f;

    // Additional necessary variables...
    public bool UpwardsRunning;
    public bool DownwardsRunning;
    public float HorizontalInput;
    public float VerticalInput;

    // Climbing variables
    public LayerMask WhatIsWall;
    public bool WallFront { get; private set; }
    public bool ExitingWall { get; set; }
    public float ExitWallTime = 0.5f; 
    public float ExitWallTimer { get; set; }
    public float ClimbJumpUpForce = 10.0f; 
    public float ClimbJumpBackForce = 10.0f; 
    public int ClimbJumps = 2;
    public int ClimbJumpsLeft { get; set; }
    public Transform LastWall { get; set; }
    public RaycastHit FrontWallHit { get; private set; }
    public float ClimbSpeed = 1.5f;
    public float DetectionLength = 1.0f; 
    public float SphereCastRadius = 0.5f; 
    public float MaxWallLookAngle = 45.0f; 
    public float MinWallNormalAngleChange = 10.0f; 
    public Vector3 LastWallNormal { get; set; }
    public float ClimbTimer { get; set; }
    public float MaxClimbTime = 5.0f; 


    // State variables
    PlayerBaseState _currentState;
    PlayerStateFactory _states;

    // Getters and setters
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public Animator Animator { get { return _animator; } }
    public CharacterController CharacterController { get { return _characterController; } }
    public Coroutine CurrentJumpResetRoutine { get { return _currentJumpResetRoutine; } set { _currentJumpResetRoutine = value; } }
    public Dictionary<int, float> InitialJumpVelocities { get { return _initialJumpVelocities; } }
    public Dictionary<int, float> JumpGravities { get { return _jumpGravities; } }
    public int JumpCount { get { return _jumpCount; } set { _jumpCount = value; } }
    public int IsWalkingHash { get { return _isWalkingHash; } }
    public int IsRunningHash { get { return _isRunningHash; } }
    public int IsJumpingHash { get { return _isJumpingHash; } }
    public int JumpCountHash { get { return _jumpCountHash; } }
    public int IsCrouchingHash { get { return _isCrouchingHash; } }
    public int IsClimbingHash { get { return _isClimbingHash; } }
    public int IsWallRunningHash { get { return _isWallRunningHash; } }
    public bool IsMovementPressed { get { return _isMovementPressed; } }
    public bool IsRunPressed { get { return _isRunPressed; } }
    public bool RequireNewJumpPress { get { return _requireNewJumpPress; } set { _requireNewJumpPress = value; } }
    public bool IsJumping { set { _isJumping = value; } }
    public bool IsJumpPressed { get { return _isJumpPressed; } }
    public bool IsCrouchPressed { get { return _isCrouchPressed; } }
    public bool IsClimbPressed { get { return _isClimbPressed; } }
    public float GroundedGravity { get { return _groundedGravity; } }
    public float CurrentMovementY { get { return _currentMovement.y; } set { _currentMovement.y = value; } }
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = value; } }
    public float AppliedMovementX { get { return _appliedMovement.x; } set { _appliedMovement.x = value; } }
    public float AppliedMovementZ { get { return _appliedMovement.z; } set { _appliedMovement.z = value; } }
    public float RunMultiplier { get { return _runMultiplier; } }
    public Vector2 CurrentMovementInput { get { return _currentMovementInput; } }
    public Vector3 AppliedMovement { get { return _appliedMovement; } set { _appliedMovement = value; } }

    // Awake is called earlier than Start in Unity's event life cycle
    void Awake()
    {
        // Initially set reference variables
        _playerInput = new PlayerInput();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        // Setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        // Set the parameter hash references
        _isWalkingHash = Animator.StringToHash("isWalking");
        _isRunningHash = Animator.StringToHash("isRunning");
        _isJumpingHash = Animator.StringToHash("isJumping");
        _jumpCountHash = Animator.StringToHash("jumpCount");
        _isCrouchingHash = Animator.StringToHash("isCrouching");
        _isClimbingHash = Animator.StringToHash("isClimbing");
        _isWallRunningHash = Animator.StringToHash("isWallRunning");

        // Set the player input callbacks
        _playerInput.CharacterControls.Move.started += OnMovementInput;
        _playerInput.CharacterControls.Move.canceled += OnMovementInput;
        _playerInput.CharacterControls.Move.performed += OnMovementInput;
        _playerInput.CharacterControls.Run.started += OnRun;
        _playerInput.CharacterControls.Run.canceled += OnRun;
        _playerInput.CharacterControls.Jump.started += OnJump;
        _playerInput.CharacterControls.Jump.canceled += OnJump;
        _playerInput.CharacterControls.Crouch.started += OnCrouch;  
        _playerInput.CharacterControls.Crouch.canceled += OnCrouch; 
        _playerInput.CharacterControls.Climb.started += OnClimb;    
        _playerInput.CharacterControls.Climb.canceled += OnClimb;   

        SetupJumpVariables();
    }

    // SwitchState method to handle state transitions
    public void SwitchState(PlayerBaseState newState)
    {
        _currentState.ExitState();
        _currentState = newState;
        _currentState.EnterState();
    }

    // method to detect climable objects
    private void WallCheck()
    {
        RaycastHit frontWallHit;
        bool wallFront = Physics.SphereCast(transform.position, SphereCastRadius, transform.forward, out frontWallHit, DetectionLength, WhatIsWall);
        float wallLookAngle = Vector3.Angle(transform.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != LastWall || Mathf.Abs(Vector3.Angle(LastWallNormal, frontWallHit.normal)) > MinWallNormalAngleChange;

        if ((wallFront && newWall) || _characterController.isGrounded)
        {
            ClimbTimer = MaxClimbTime;
            ClimbJumpsLeft = ClimbJumps;
        }

        // Assign the local hit to the property
        FrontWallHit = frontWallHit;
        WallFront = wallFront;
    }


    public void SetWallHits(RaycastHit leftHit, RaycastHit rightHit)
    {
        LeftWallHit = leftHit;
        RightWallHit = rightHit;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WhatIsWall"))
        {
            SwitchState(_states.WallRun());
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("WhatIsWall"))
        {
            SwitchState(_states.Grounded());
        }
    }


    // set the initial velocity and gravity using jump heights and durations
    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
        float secondJumpGravity = (-2 * (_maxJumpHeight + 2)) / Mathf.Pow((timeToApex * 1.25f), 2);
        float secondJumpInitialVelocity = (2 * (_maxJumpHeight + 2)) / (timeToApex * 1.25f);
        float thirdJumpGravity = (-2 * (_maxJumpHeight + 4)) / Mathf.Pow((timeToApex * 1.5f), 2);
        float thirdJumpInitialVelocity = (2 * (_maxJumpHeight + 4)) / (timeToApex * 1.5f);

        _initialJumpVelocities.Add(1, _initialJumpVelocity);
        _initialJumpVelocities.Add(2, secondJumpInitialVelocity);
        _initialJumpVelocities.Add(3, thirdJumpInitialVelocity);

        _jumpGravities.Add(0, _gravity);
        _jumpGravities.Add(1, _gravity);
        _jumpGravities.Add(2, secondJumpGravity);
        _jumpGravities.Add(3, thirdJumpGravity);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();
        _currentState.UpdateStates();
        _characterController.Move(_appliedMovement * Time.deltaTime);

        WallCheck();
    }

    void HandleRotation()
    {
        Vector3 positionToLookAt;
        // the change in position our character should point to
        positionToLookAt.x = _currentMovementInput.x;
        positionToLookAt.y = _zero;
        positionToLookAt.z = _currentMovementInput.y;
        // the current rotation of our character
        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed)
        {
            // creates a new rotation based on where the player is currently pressing
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            // rotate the character to face the positionToLookAt            
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    // callback handler function to set the player input values
    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != _zero || _currentMovementInput.y != _zero;
    }

    // callback handler function for jump buttons
    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _requireNewJumpPress = false;
    }

    // callback handler function for run buttons
    void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    // callback handler function for crouch buttons
    void OnCrouch(InputAction.CallbackContext context)
    {
        _isCrouchPressed = context.ReadValueAsButton();
    }

    // callback handler function for climb buttons
    void OnClimb(InputAction.CallbackContext context)
    {
        _isClimbPressed = context.ReadValueAsButton();
    }

    void OnEnable()
    {
        // enable the character controls action map
        _playerInput.CharacterControls.Enable();
    }

    void OnDisable()
    {
        // disable the character controls action map
        _playerInput.CharacterControls.Disable();
    }
}
