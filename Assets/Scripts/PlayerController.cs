using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    // Componenets
    Rigidbody rb;
    CapsuleCollider col;

    [Header("Basic Movement")]
    public float speed = 80;
    public float sprintingSpeed = 120;
    public float crouchSpeed = 50;
    float height = 0;

    [Header("Limiters")]
    public float maxWalkSpeed = 100;
    public float maxSprintSpeed = 150;

    [Header("Jumping")]
    public float JumpPower = 300f;
    public float fallMultiplier = 2;
    bool isGrounded;

    [Header("Slope Handeling")]
    public float maxSlopeAngle = 40;
    RaycastHit slopeHit;

    // Movement States
    MovementStates state;
    enum MovementStates 
    {
        Walking,
        Sprinting,
        Crouching,
    }

    [Header("Coyote Time")]
    public float CoyoteTime = 0.3f;
    float coyoteTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        height = col.height / 2;
    }

    private void Update()
    {
        HandleStates();
        HandleJumping();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }


    // Handles movement states
    void HandleStates() 
    {
        // Changes state. Priority is sprinting
        if (Input.GetKey(KeyCode.LeftShift))
        {
            state = MovementStates.Sprinting;
        }
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            state = MovementStates.Crouching;
        }
        else 
        {
            state = MovementStates.Walking;
        }
    }
    
    // Applies Movement
    void HandleMovement()
    {
        rb.useGravity = !checkSlope();

        // Applying Force
        Vector3 movement = transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");

        // Changing speed depending on state
        float curSpeed = speed;
        float maxSpeed = maxWalkSpeed;

        if (state == MovementStates.Sprinting)
        {
            curSpeed = sprintingSpeed;
            maxSpeed = maxSprintSpeed;
        }
        else if (state==MovementStates.Crouching)
        {
            curSpeed = crouchSpeed;
        }

        if (checkSlope())
        {
            if (rb.velocity.y > 0) 
            {
                rb.AddForce(-0.01f * Vector3.down);
            }
            rb.AddForce(GetSlopeDirection(movement) * curSpeed, ForceMode.Force);
        }
        else 
        {
            rb.AddForce(curSpeed * movement.normalized, ForceMode.Acceleration);
        }

        // Limiting Speed
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.y);
        if (flatVel.magnitude >= maxSpeed) 
        {
            Vector3 maxVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(maxVel.x, rb.velocity.y, maxVel.z);
        }
    }

    // Applies Jumping
    private void HandleJumping()
    {
        // Actual Check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, height + 0.1f, LayerMask.GetMask("Ground")); // Ground Check

        // Coyote Time Implenmentation and sticking to ground
        if (isGrounded)
        {
            coyoteTimer = CoyoteTime;
        }
        else
        {
            if (coyoteTimer > -1)
            {
                coyoteTimer -= Time.deltaTime;
            }
        }

        // Applying Force
        if (Input.GetButtonDown("Jump") && coyoteTimer > 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * JumpPower);
            coyoteTimer = 0;
        }

        // Fall Multiplier
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    // Checks if on slope
    bool checkSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, height + 0.3f)) 
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    // Edits direction if on slope
    private Vector3 GetSlopeDirection(Vector3 direction) 
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal);
    }

}
