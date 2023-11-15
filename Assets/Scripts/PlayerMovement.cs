using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;


    public float airSpeed = 10f;
    public float groundSpeed = 10f;
    public float crouchSpeed = 5f;
    public float acceleration = 7f;
    public float fallSpeed = 4f;
    public float fastFall = 2f;

    public float dashForce = 10f;
    public float dashCooldown = 1.0f;
    private float lastDashTime = 0f;

    public float jumpingPower = 18f;
    private bool isFacingRight = true;

    private bool isWallSliding;
    public float wallSlidingSpeed = 1f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.05f;
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private int airJumpsLeft = 2;

    private bool isCrouching = false;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private TrailRenderer tr;
    private Animator animator;
    Damageable damageable;

    public bool isStunned = false;
    public float stunMobility = 0;

    private void Start()
    {
        animator = GetComponent<Animator>();
        damageable = GetComponent<Damageable>();
        animator.speed = 60.0f / 50.0f;
    }

    private void Update()
    {


        horizontal = Input.GetAxisRaw("Horizontal");

        bool grounded = IsGrounded();
        animator.SetBool("IsGrounded", grounded);

        animator.SetBool("IsRunning", grounded && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)));

        if (grounded && Input.GetKey(KeyCode.DownArrow))
        {
            isCrouching = true;
            animator.SetTrigger("CrouchTrigger");
        }
        else
        {
            isCrouching = false;
        }

        animator.SetBool("IsCrouching", isCrouching);

        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                airJumpsLeft = 2;
                animator.SetTrigger("JumpTrigger");
            }
            else if (airJumpsLeft > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                airJumpsLeft--;
                animator.SetTrigger("JumpTrigger");
            }

        }

        if (Input.GetMouseButtonDown(0) && IsGrounded() && Mathf.Abs(horizontal) == 0 && !isCrouching)
        {
            animator.SetTrigger("Nlight");
        }

        if (Input.GetMouseButtonDown(1) && IsGrounded() && Mathf.Abs(horizontal) == 0 && !isCrouching)
        {
            animator.SetTrigger("Nstk");
        }


        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }

        // Handle horizontal dash
        if (Input.GetKeyDown(KeyCode.E) && Time.time - lastDashTime >= dashCooldown && Mathf.Abs(horizontal) > 0 && !isCrouching && IsGrounded())
        {
            HorizontalDash();
            lastDashTime = Time.time;
            animator.SetTrigger("DashTrigger");
        }
    }

    private Vector2 targetVelocity;

    private void FixedUpdate()
    {


        if (!isWallJumping)
        {
            if (IsGrounded())
            {
                if (isCrouching)
                {
                    targetVelocity = new Vector2(horizontal * crouchSpeed, rb.velocity.y);
                }
                else
                {
                    targetVelocity = new Vector2(horizontal * groundSpeed, rb.velocity.y);
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    rb.gravityScale = fallSpeed + fastFall; // Fast fall state
                }
                else
                {
                    rb.gravityScale = fallSpeed; // Normal falling speed when not fast falling
                }

                targetVelocity = new Vector2(horizontal * airSpeed, rb.velocity.y);
            }
        }

        if (!damageable.IsHit)
        {
            rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGrounded())
        {
            rb.gravityScale = 1f;
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.5f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
            animator.SetBool("IsOnWall", true);
        }
        else
        {
            isWallSliding = false;
            animator.SetBool("IsOnWall", false);
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
            airJumpsLeft = 2;
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump") && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if (transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
            airJumpsLeft = 2;
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void HorizontalDash()
    {
        if (IsGrounded())
        {
            rb.AddForce(Vector2.right * dashForce * (isFacingRight ? 1f : -1f), ForceMode2D.Impulse);
        }
    }

    public void IsStunned()
    {
        if (isStunned == true)
        {
            airSpeed = stunMobility;
            groundSpeed = stunMobility;
            acceleration = stunMobility;
        }
    }
}
