using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    CharacterController _characterController;
    Animator _animator;
    PlayerInput _playerInput;

    int _isWalkingHash;
    int _isRunningHash;

    // variables to store player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _currentRunMovement;
    Vector3 _appliedMovement;
    bool _isMovementPressed;
    bool _isRunPressed;

    float _rotationFactorPerFrame = 20.0f;
    float _runMultiplier = 10.0f;
    int _zero = 0;

    // gravity variables
    float _gravity = -9.8f;
    float _groundedGravity = -.05f;

    // jumping variables
    bool _isJumpPressed = false;
    float _initialJumpVelocity;
    float _maxJumpHeight = 4.0f;
    float _maxJumpTime = 0.75f;
    bool _isJumping = false;
    int _isJumpingHash;
    int _jumpCountHash;
    bool _isJumpAnimating = false;
    int _jumpCount = 0;
    Dictionary<int, float> _initialJumpVelocities = new Dictionary<int, float>();
    Dictionary<int, float> _jumpGravities = new Dictionary<int, float>();
    Coroutine _currentJumpResetRoutine = null;

    //Start is called before the first frame update
    void Start()
    {

    }

    //Update is called once per frame
    void Update()
    {

    }
}
