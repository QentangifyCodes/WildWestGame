using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    bool onSlope;

    [Header("Limiters")]
    public float maxWalkSpeed = 100;
    public float maxSprintSpeed = 150;

    [Header("Jumping")]
    public float JumpPower = 300f;
    public float fallMultiplier = 2;
    bool isGrounded;
    
    // Movement States
    MovementStates state;
    enum MovementStates 
    {
        Walking,
        Sprinting,
        Crouching
    }

    [Header("Coyote Time")]
    public float CoyoteTime = 0.3f;
    float coyoteTimer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
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

    private void HandleJumping()
    {
        // Actual Check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, col.height/2+0.3f, LayerMask.GetMask("Ground")); // Ground Check

        // Coyote Time Implenmentation and sticking to ground
        if (isGrounded)
        {
            coyoteTimer = CoyoteTime;
            rb.velocity = new Vector3(rb.velocity.x, -4.5f, rb.velocity.y);
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
            rb.AddForce(transform.up * JumpPower);
            coyoteTimer = 0;
        }

        // Fall Multiplier
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }
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
    void HandleMovement()
    {
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
        rb.AddForce(curSpeed*movement.normalized, ForceMode.Acceleration);

        // Limiting Speed
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.y);
        if (flatVel.magnitude >= maxSpeed) 
        {
            Vector3 maxVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(maxVel.x, rb.velocity.y, maxVel.z);
        }
    }

}
