using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 12f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashCooldown = 2f;
    [SerializeField] private KeyCode dashKey = KeyCode.V;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    private Rigidbody2D rb;
    private Animator animator;

    private Vector2 moveDirection;
    private Vector2 lastMoveDirection;

    private bool isRunning;
    private bool isDashing;
    private bool canDash = true;
    private float dashTimer;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        ProcessInput();
        UpdateAnimator();
        CheckDashTrigger();
    }

    private void ProcessInput()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveDirection.sqrMagnitude > 0.01f)
        {
            lastMoveDirection = moveDirection;
        }

        isRunning = Input.GetKey(runKey);
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("MoveX", lastMoveDirection.x);
        animator.SetFloat("MoveY", lastMoveDirection.y);
        animator.SetFloat("Speed", moveDirection.sqrMagnitude);
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsDashing", isDashing);
    }

    private void CheckDashTrigger()
    {
        if (Input.GetKeyDown(dashKey) && canDash && isRunning && moveDirection.sqrMagnitude > 0)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        canDash = false;
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            ProcessDash();
        }
        else
        {
            ProcessMovement();
        }
    }

    private void ProcessDash()
    {
        rb.MovePosition(rb.position + lastMoveDirection * dashForce * Time.fixedDeltaTime);
        dashTimer -= Time.fixedDeltaTime;

        if (dashTimer <= 0f)
        {
            isDashing = false;
            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    private void ProcessMovement()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }

    private void ResetDash()
    {
        canDash = true;
    }
}