using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private float horizontal;

    public float airSpeed = 10f;
    public float groundSpeed = 10f;
    private float acceleration = 5f;
    public float fallSpeed = 8f;
    public float fastFall = 5f;

    public float jumpingPower = 16f;
    private bool isFacingRight = true;

    private bool isWallSliding;
    private float wallSlidingSpeed = 1f;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    private float wallJumpingDuration = 0.05f;
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);

    private int airJumpsLeft = 2;

    private bool canDash = true;
    private bool isDashing = false;
    public float dashingTime;
    public float dashSpeed = 35f;
    public float dashJumpIncrease = 18f;
    public float TimeBtwDashes;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private TrailRenderer tr;
    private Animator animator;

    private void Start()
{
    animator = GetComponent<Animator>();
}

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        bool grounded = IsGrounded();
        animator.SetBool("IsGrounded", grounded);

        animator.SetBool("IsRunning", grounded && (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)));

        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                airJumpsLeft = 2;
            }
            else if (airJumpsLeft > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
                airJumpsLeft--;
            }

            animator.SetTrigger("JumpTrigger");
        }

        WallSlide();
        WallJump();

        if (!isWallJumping && !isDashing)
        {
            Flip();
        }
    }

    

    private Vector2 targetVelocity;

    private void FixedUpdate()
    {
        if (IsGrounded())
        {

            if (Input.GetKeyDown(KeyCode.E))
            {
                DashAbility();
                animator.SetTrigger("DashTrigger");
            }
        }
        if (!isWallJumping)
        {
            if (IsGrounded())
            {
                targetVelocity = new Vector2(horizontal * groundSpeed, rb.velocity.y);
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
        if (isDashing)
        {
            if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
            {
                Flip();
            }
        }

        // Smoothly interpolate towards the target velocity
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
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
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
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

    private void DashAbility()
    {
        if (canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        // save original values
        float originalGroundSpeed = groundSpeed;
        float originalAirSpeed = airSpeed;
        float originalJumpingPower = jumpingPower;


        groundSpeed = dashSpeed;
        airSpeed = dashSpeed;
        jumpingPower = dashJumpIncrease;
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;

        groundSpeed = originalGroundSpeed;
        airSpeed = originalAirSpeed;
        jumpingPower = originalJumpingPower;

        yield return new WaitForSeconds(TimeBtwDashes);
        isDashing = false;
        canDash = true;
    }

}