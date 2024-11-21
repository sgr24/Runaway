using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    public float wallRunForce;
    public float maxWallRunTime;
    public float wallRunTimer;

    [Header("Input")]
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    [Header("References")]
    public Transform orientation;
    private PlayerMovement pm;
    private CharacterController controller;

    [Header("Wall Run Detection")]
    public KeyCode upwardsRunKey = KeyCode.LeftShift;
    public KeyCode downwardsRunKey = KeyCode.LeftControl;

    // Variables to track upward and downward running
    private bool upwardsRunning = false;
    private bool downwardsRunning = false;
    // Adjustable in code and inspector
    public float wallClimbSpeed = 3f;

    [Header("Respawn Settings")]
    public Transform respawnPoint;
    public float fallLimit = -10f;

    private Vector3 moveDirection;
    private Vector3 velocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        pm = GetComponent<PlayerMovement>();
    }
    private void Update()
    {
        CheckForWall();
        StateMachine();
        CheckFallOffMap();

        // Apply gravity
        if (pm.wallrunning)
        {
            velocity.y += Physics.gravity.y * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if (pm.wallrunning)
            WallRunningMovement();
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        // Getting Inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Stage 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {
            if (!pm.wallrunning)
                StartWallRun();
        }
        // Stage 2 - None
        else
        {
            if (pm.wallrunning)
                StopWallRun();
        }

        // Update running states
        upwardsRunning = Input.GetKey(upwardsRunKey);
        downwardsRunning = Input.GetKey(downwardsRunKey);
    }

    private void StartWallRun()
    {
        pm.wallrunning = true;
        velocity = Vector3.zero; // Reset velocity when starting wall run
    }

    private void WallRunningMovement()
    {
        Vector3 wallNormal = wallRight ? rightWallhit.normal : leftWallhit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
            wallForward = -wallForward;

        // Running forward
        moveDirection = wallForward * wallRunForce;

        // Upwards/downwards movement
        if (upwardsRunning)
            moveDirection += Vector3.up * wallClimbSpeed;
        if (downwardsRunning)
            moveDirection += Vector3.down * wallClimbSpeed;

        // Push to the wall force
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            moveDirection += -wallNormal * 100;

        // Move the character
        controller.Move(moveDirection * Time.deltaTime);
    }

    private void StopWallRun()
    {
        pm.wallrunning = false;
        velocity = Vector3.zero; // Reset velocity when stopping wall run
    }

    private void CheckFallOffMap()
    {
        if (transform.position.y < fallLimit)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        velocity = Vector3.zero;
        transform.position = respawnPoint.position;
    }
}