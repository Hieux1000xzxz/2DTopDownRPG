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
    [SerializeField] private Transform[] bulletSpawnPoints; // Mảng các điểm sinh đạn
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float attackRate = 2f; // Số lần bắn mỗi giây

    [Header("Spear Attack Settings")]
    [SerializeField] private float spearAttackLength = 2f; // Chiều dài vùng tấn công
    [SerializeField] private float spearAttackWidth = 1f;  // Chiều rộng vùng tấn công
    [SerializeField] private LayerMask enemyLayer;         // Lớp của kẻ địch
    [SerializeField] private Transform attackPoint;        // Điểm xuất phát của vùng tấn công
    [SerializeField] private float attackCooldown = 0.5f;  // Thời gian hồi chiêu
    [SerializeField] private float attackDuration = 1f; // thời gian animation chém kiếm


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
        ProcessInput();
        UpdateAnimator();
        CheckDashTrigger();
        CheckWeaponSwitch();
        CheckShoot();
        CheckSpearAttack();
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
        else if (!(isAttacking && animator.GetInteger("WeaponType") == 2)) // ✅ chỉ khóa khi đang chém kiếm
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
        if (isAttacking == false && Input.GetKeyDown(KeyCode.Alpha1))
        {
            animator.SetInteger("WeaponType", 1); // không vũ khí
        }
        else if (isAttacking == false && Input.GetKeyDown(KeyCode.Alpha2))
        {
            animator.SetInteger("WeaponType", 2); // kiếm
        }
        else if (isAttacking == false && Input.GetKeyDown(KeyCode.Alpha3))
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

        if (Input.GetMouseButtonUp(0) && animator.GetInteger("WeaponType") == 3)
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
        // Xác định hướng dựa trên lastMoveDirection
        int directionIndex = GetDirectionIndex(lastMoveDirection);

        // Lấy điểm sinh đạn tương ứng
        Transform spawnPoint = bulletSpawnPoints[directionIndex];
        if (bulletPrefab == null)
        {
            Debug.LogError("Bullet prefab chưa được gán!");
            return;
        }
        // Tạo đạn tại điểm sinh đạn
        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.direction = lastMoveDirection;
        bulletScript.speed = bulletSpeed;

        // Bỏ qua va chạm giữa đạn và người chơi
        Collider2D bulletCol = bullet.GetComponent<Collider2D>();
        if (bulletCol != null && playerCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCol, playerCollider);
        }
    }
    private void CheckSpearAttack()
    {
        if (Input.GetMouseButtonDown(0) && animator.GetInteger("WeaponType") == 2 && canAttack)
        {
            StartCoroutine(SpearAttack());
        }
    }


    private IEnumerator SpearAttack()
    {
        canAttack = false;
        isAttacking = true;
        yield return new WaitForSeconds(0.3f); // delay đầu tiên

        // Đòn chém 1
        DoSpearHit();
        yield return new WaitForSeconds(0.5f); // delay giữa 2 đòn

        // Đòn chém 2
        DoSpearHit();
        yield return new WaitForSeconds(attackDuration - 0.8f); // phần còn lại

        isAttacking = false;
        yield return new WaitForSeconds(attackCooldown - attackDuration);
        canAttack = true;
    }




    private void DoSpearHit()
    {
        Vector2 attackPosition = (Vector2)attackPoint.position + lastMoveDirection.normalized * (spearAttackLength / 2f);
        float attackAngle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            attackPosition,
            new Vector2(spearAttackLength, spearAttackWidth),
            attackAngle,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("Kẻ địch bị trúng đòn: " + enemy.name);
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(20); // Gây 30 sát thương (có thể điều chỉnh)
            }
            Flash flash = enemy.GetComponent<Flash>();
            if (flash != null)
            {
                flash.StartFlash();
            }
            // Gọi KnockBack
            KnockBack knockBack = enemy.GetComponent<KnockBack>();
            if (knockBack != null)
            {
                knockBack.ApplyKnockBack(transform.position); // Đẩy lùi từ vị trí người chơi
            }
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;

        // Tính toán vị trí và góc xoay của vùng tấn công
        Vector2 attackPosition = (Vector2)attackPoint.position + lastMoveDirection.normalized * (spearAttackLength / 2f);
        float attackAngle = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;

        // Hiển thị vùng tấn công hình chữ nhật
        Gizmos.matrix = Matrix4x4.TRS(attackPosition, Quaternion.Euler(0, 0, attackAngle), Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(spearAttackLength, spearAttackWidth, 0));
    }
    private int GetDirectionIndex(Vector2 direction)
    {
        if (direction.x > 0.5f && Mathf.Abs(direction.y) <= 0.5f) return 0; // Phải
        if (direction.x < -0.5f && Mathf.Abs(direction.y) <= 0.5f) return 1; // Trái
        if (direction.y > 0.5f && Mathf.Abs(direction.x) <= 0.5f) return 2; // Trên
        if (direction.y < -0.5f && Mathf.Abs(direction.x) <= 0.5f) return 3; // Dưới
        if (direction.x > 0.5f && direction.y > 0.5f) return 4; // Phải-Trên
        if (direction.x > 0.5f && direction.y < -0.5f) return 5; // Phải-Dưới
        if (direction.x < -0.5f && direction.y > 0.5f) return 6; // Trái-Trên
        if (direction.x < -0.5f && direction.y < -0.5f) return 7; // Trái-Dưới

        return 0; // Mặc định là Phải
    }

}
