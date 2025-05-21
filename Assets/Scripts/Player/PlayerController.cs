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
    [SerializeField] private Transform[] bulletSpawnPoints;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float attackRate = 2f;

    [Header("Spear Attack Settings")]
    [SerializeField] private float spearAttackLength = 2f;
    [SerializeField] private float spearAttackWidth = 1f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackDuration = 1f;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D playerCollider;

    private Vector2 moveDirection;
    private Vector2 lastMoveDirection = Vector2.right;

    private bool isRunning;
    private bool isDashing;
    private bool canDash = true;
    private float dashTimer;
    private bool isAttacking;
    private bool canAttack = true;
    private Coroutine attackCoroutine;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
    }

    private void Update()
    {
        HandleInput();
        UpdateAnimator();
        HandleDashInput();
        HandleWeaponSwitch();
        HandleShooting();
        HandleSpearAttack();
    }

    private void FixedUpdate()
    {
        if (isDashing)
            PerformDash();
        else if (!(isAttacking && IsUsingWeapon(2)))
            Move();
    }

    private void HandleInput()
    {
        moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (moveDirection.sqrMagnitude > 0.01f)
            lastMoveDirection = moveDirection;

        isRunning = Input.GetKey(runKey);
    }

    private void Move()
    {
        float speed = isRunning ? runSpeed : walkSpeed;
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
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

    private void HandleDashInput()
    {
        if (Input.GetKeyDown(dashKey) && canDash && isRunning && moveDirection.sqrMagnitude > 0)
            StartDash();
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashDuration;
        canDash = false;
    }

    private void PerformDash()
    {
        rb.MovePosition(rb.position + lastMoveDirection * dashForce * Time.fixedDeltaTime);
        dashTimer -= Time.fixedDeltaTime;

        if (dashTimer <= 0f)
        {
            isDashing = false;
            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    private void ResetDash() => canDash = true;

    private void HandleWeaponSwitch()
    {
        if (isAttacking) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) animator.SetInteger("WeaponType", 1);
        if (Input.GetKeyDown(KeyCode.Alpha2)) animator.SetInteger("WeaponType", 2);
        if (Input.GetKeyDown(KeyCode.Alpha3)) animator.SetInteger("WeaponType", 3);
    }

    private void HandleShooting()
    {
        if (!IsUsingWeapon(3) || isDashing) return;

        if (Input.GetMouseButton(0))
        {
            // Chỉ bắt đầu bắn nếu được phép
            if (!isAttacking && canAttack)
            {
                isAttacking = true;
                attackCoroutine = StartCoroutine(ShootLoop());
            }
        }

        if (Input.GetMouseButtonUp(0) && isAttacking)
        {
            // Dừng bắn và bắt đầu cooldown
            isAttacking = false;

            if (attackCoroutine != null)
            {
                StopCoroutine(attackCoroutine);
                attackCoroutine = null;
            }

            StartCoroutine(ShootCooldown());
        }
    }


    private IEnumerator ShootLoop()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(1f / attackRate);
        }
    }

    private void Shoot()
    {
        int index = GetDirectionIndex(lastMoveDirection);
        Transform spawnPoint = bulletSpawnPoints[index];
        if (!bulletPrefab) return;

        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
        var bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.direction = lastMoveDirection;
        bulletScript.speed = bulletSpeed;

        if (bullet.TryGetComponent(out Collider2D bulletCol) && playerCollider)
            Physics2D.IgnoreCollision(bulletCol, playerCollider);
    }
    private IEnumerator ShootCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(0.5f); // cooldown sau khi thả chuột
        canAttack = true;
    }




    private void HandleSpearAttack()
    {
        if (Input.GetMouseButtonDown(0) && IsUsingWeapon(2) && canAttack && isDashing == false)
            StartCoroutine(SpearAttackRoutine());
    }

    private IEnumerator SpearAttackRoutine()
    {
        canAttack = false;
        isAttacking = true;

        var originalConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.3f);
        DoSpearHit();

        yield return new WaitForSeconds(0.5f);
        DoSpearHit();

        yield return new WaitForSeconds(attackDuration - 0.8f);

        isAttacking = false;
        rb.constraints = originalConstraints;

        yield return new WaitForSeconds(attackCooldown - attackDuration);
        canAttack = true;
    }

    private void DoSpearHit()
    {
        Vector2 attackPos = (Vector2)attackPoint.position + lastMoveDirection * (spearAttackLength / 2f);
        float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            attackPos, new Vector2(spearAttackLength, spearAttackWidth), angle, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy.TryGetComponent(out EnemyHealth health))
                health.TakeDamage(20);

            if (enemy.TryGetComponent(out Flash flash))
                flash.StartFlash();

            if (enemy.TryGetComponent(out KnockBack knockBack))
                knockBack.ApplyKnockBack(transform.position);
        }
    }

    private bool IsUsingWeapon(int weaponType)
    {
        return animator.GetInteger("WeaponType") == weaponType;
    }

    private int GetDirectionIndex(Vector2 dir)
    {
        if (dir.x > 0.5f && Mathf.Abs(dir.y) <= 0.5f) return 0;
        if (dir.x < -0.5f && Mathf.Abs(dir.y) <= 0.5f) return 1;
        if (dir.y > 0.5f && Mathf.Abs(dir.x) <= 0.5f) return 2;
        if (dir.y < -0.5f && Mathf.Abs(dir.x) <= 0.5f) return 3;
        if (dir.x > 0.5f && dir.y > 0.5f) return 4;
        if (dir.x > 0.5f && dir.y < -0.5f) return 5;
        if (dir.x < -0.5f && dir.y > 0.5f) return 6;
        if (dir.x < -0.5f && dir.y < -0.5f) return 7;

        return 0; // mặc định
    }

    private void OnDrawGizmosSelected()
    {
        if (!attackPoint) return;

        Vector2 pos = (Vector2)attackPoint.position + lastMoveDirection.normalized * (spearAttackLength / 2f);
        float angle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;

        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(spearAttackLength, spearAttackWidth, 0));
    }
}
