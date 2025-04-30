using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
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

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint; // điểm cố định để sinh đạn
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float attackRate = 3f; // số lần bắn mỗi giây

    private Collider2D playerCollider;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 moveDirection;
    private Vector2 lastMoveDirection = Vector2.right; // mặc định hướng phải
    private bool isRunning;
    private bool isDashing;
    private bool canDash = true;
    private float dashTimer;
    private bool isAttacking = false;
    private Coroutine attackCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        ProcessInput();
        UpdateAnimator();
        CheckDashTrigger();
        CheckWeaponSwitch();
        CheckShoot();
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
        animator.SetBool("IsAttacking", isAttacking);
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

    private void CheckWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetInteger("WeaponType", 1); // không vũ khí
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetInteger("WeaponType", 2); // kiếm
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            animator.SetInteger("WeaponType", 3); // súng
        }
    }

    private void CheckShoot()
    {
        if (Input.GetMouseButtonDown(0) && animator.GetInteger("WeaponType") == 3)
        {
            isAttacking = true;
            if (attackCoroutine == null)
                attackCoroutine = StartCoroutine(AttackLoop());
        }

        if (Input.GetMouseButtonUp(0))
        {
            isAttacking = false;
            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }
        }
    }

    private IEnumerator AttackLoop()
    {
        while (isAttacking)
        {
            Shoot();
            yield return new WaitForSeconds(1f / attackRate);
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.direction = lastMoveDirection;
        bulletScript.speed = bulletSpeed;

        Collider2D bulletCol = bullet.GetComponent<Collider2D>();
        if (bulletCol != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCol, playerCollider);
        }
    }
}
