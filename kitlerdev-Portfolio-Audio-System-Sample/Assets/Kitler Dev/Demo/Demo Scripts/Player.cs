using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 20f;
    [SerializeField] private float airControl = 0.5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 15f;
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float fallGravityScale = 5f;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 25f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.5f;
    [SerializeField] private int maxAirDashes = 1;

    [Header("Ground Detection")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.5f);

    // Component references
    private Rigidbody2D rb;
    private Collider2D col;

    // Movement variables
    private float horizontalInput;
    private bool isFacingRight = true;
    private float originalGravity;

    // Jump variables
    private bool isJumping;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    private bool wasGrounded;

    // Dash variables
    private bool isDashing;
    private float dashCooldownCounter;
    private int airDashCount;
    private Vector2 dashDirection;

    // Audio references (assuming you have AudioManager setup)
    // public AudioManager audioManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        if (isDashing) return; // Skip input during dash

        GetInput();
        HandleJump();
        HandleDash();
        UpdateTimers();
        CheckGround();
        HandleAudio();
        FlipCharacter();
    }

 
    void FixedUpdate()
    {
        if (isDashing) return; // Skip movement during dash

        HandleMovement();
        HandleGravity();
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // Jump input buffer
        if (Input.GetKeyDown(KeyCode.Space))
        {
           
            jumpBufferCounter = jumpBufferTime;
        }
    }

    private void HandleMovement()
    {
        float targetSpeed = horizontalInput * moveSpeed;
        float currentSpeed = rb.velocity.x;

        // Calculate acceleration rate
        float accelerationRate = (Mathf.Abs(targetSpeed) > 0.1f) ? acceleration : deceleration;

        // Reduce air control
        if (!isGrounded) accelerationRate *= airControl;

        // Calculate speed difference
        float speedDiff = targetSpeed - currentSpeed;
        float movement = speedDiff * accelerationRate;

        // Apply movement force
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void HandleJump()
    {
        // Coyote time logic
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
           
            airDashCount = 0; // Reset air dashes when grounded
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump buffer logic
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        // Perform jump
        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0 && !isJumping)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpBufferCounter = 0;
            coyoteTimeCounter = 0;
            isJumping = true;

            // Play jump sound
             AudioManager.Instance.Play(Audio.Jump);
        }

        // Variable jump height (shorter jump when button released early)
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0)
        {
           
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            isJumping = false;
        }
    }

    private void HandleDash()
    {
        // Update dash cooldown
        if (dashCooldownCounter > 0)
        {
            dashCooldownCounter -= Time.deltaTime;
        }

        // Check dash input
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCooldownCounter <= 0)
        {
            // Check if we can air dash
            if (!isGrounded && airDashCount >= maxAirDashes) return;

            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        dashCooldownCounter = dashCooldown;

        // Determine dash direction
        dashDirection = new Vector2(horizontalInput, 0f);
        if (dashDirection.magnitude < 0.1f) // If no input, dash forward
        {
            dashDirection = isFacingRight ? Vector2.right : Vector2.left;
        }
        dashDirection.Normalize();

        // Apply dash velocity
        rb.velocity = dashDirection * dashSpeed;
        rb.gravityScale = 0f; // Remove gravity during dash

        // Increment air dash count if in air
        if (!isGrounded)
        {
            airDashCount++;
        }

        // Play dash sound
        AudioManager.Instance.Play(Audio.Dash);

        yield return new WaitForSeconds(dashDuration);

        // End dash
        isDashing = false;
        rb.gravityScale = originalGravity;
    }

    private void HandleGravity()
    {
        // Increased gravity when falling
        if (rb.velocity.y < 0)
        {
            rb.gravityScale = fallGravityScale;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }
    }

    private void CheckGround()
    {
        wasGrounded = isGrounded;

        // Calculate ground check position
        Vector2 checkPosition = (Vector2)transform.position + groundCheckOffset;

        // Perform ground check
        isGrounded = Physics2D.OverlapBox(checkPosition, groundCheckSize, 0f, groundLayer);

        // Reset jump state when landing
        if (!wasGrounded && isGrounded)
        {
            isJumping = false;

            // Play land sound (only if falling fast enough)
            if (rb.velocity.y < -5f)
            {
                 AudioManager.Instance.Play(Audio.Land);
            }
        }
    }

    private void HandleAudio()
    {
        // Play footstep sounds based on movement
        if (isGrounded && Mathf.Abs(rb.velocity.x) > 0.1f && Mathf.Abs(horizontalInput) > 0.1f)
        {
            // You might want to use a coroutine or timer for footsteps
            // AudioManager.Instance.Play(Audio.Footstep);
        }
    }

    private void FlipCharacter()
    {
        // Flip character based on movement direction
        if (horizontalInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void UpdateTimers()
    {
        // Update coyote time
        if (coyoteTimeCounter > 0)
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Update jump buffer
        if (jumpBufferCounter > 0)
        {
            jumpBufferCounter -= Time.deltaTime;
        }
    }

    // Visual debug for ground check
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector2 checkPosition = Application.isPlaying ?
            (Vector2)transform.position + groundCheckOffset :
            (Vector2)transform.position + groundCheckOffset;
        Gizmos.DrawWireCube(checkPosition, groundCheckSize);
    }
}