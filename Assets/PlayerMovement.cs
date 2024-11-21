using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float wallrunSpeed;
    bool readyToJump;
    public bool wallrunning;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;
    Vector3 velocity;

    CharacterController controller;

    [Header("Speed Display")]
    [SerializeField]
    public TextMeshProUGUI text_speed;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        readyToJump = true;

        walkSpeed = moveSpeed;
        sprintSpeed = moveSpeed * 2f;
        crouchSpeed = moveSpeed * 0.5f;
        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        grounded = controller.isGrounded;

        MyInput();
        SpeedControl();

        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Apply gravity
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKey(sprintKey))
        {
            moveSpeed = sprintSpeed;
        }
        else if (Input.GetKey(crouchKey))
        {
            moveSpeed = crouchSpeed;
        }
        else
        {
            moveSpeed = walkSpeed;
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
    }

    private void SpeedControl()
    {
        if (text_speed != null)
        {
            Vector3 flatVel = new Vector3(controller.velocity.x, 0f, controller.velocity.z);
            text_speed.SetText("Speed: " + flatVel.magnitude);
        }
        else
        {
            Debug.LogError("text_speed is not assigned in the Inspector");
        }
    }

    private void Jump()
    {
        velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}