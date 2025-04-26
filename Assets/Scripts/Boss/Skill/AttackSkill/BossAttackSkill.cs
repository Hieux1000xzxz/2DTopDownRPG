using System.Collections;
using UnityEngine;

public class BossAttackSkill : MonoBehaviour
{
    public Animator animator;
    public float attackDuration = 1.5f;   // Thời gian tấn công
    public float attackRange = 1.5f;       // Phạm vi tấn công
    public float attackWidth = 1f;         // Chiều rộng tấn công
    public int damageAmount = 20;          // Lượng damage gây ra

    public LayerMask playerLayer;          // Layer mask của player
    public Transform attackOrigin;         // Vị trí gốc để tạo vùng tấn công
    private Vector2 attackDirection = Vector2.right;  // Hướng mặc định (sẽ update sau)

    private Transform player; // Tham chiếu đến người chơi

    private void Awake()
    {
        // Lấy tham chiếu đến đối tượng người chơi (giả sử người chơi có tag "Player")
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public void Execute()
    {
        // Kiểm tra nếu animator hoặc attackOrigin là null, tránh lỗi khi thiếu thành phần
        if (animator == null || attackOrigin == null) return;

        StopAllCoroutines();  // Dừng tất cả coroutine trước khi bắt đầu tấn công
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        if (animator == null) yield break; // Kiểm tra nếu animator là null

        animator.SetBool("IsAttacking", true);

        // Cập nhật hướng tấn công dựa trên vị trí của boss và người chơi
        SetAttackDirection();

        // Kiểm tra gây damage trong thời gian attack
        Vector2 center = (Vector2)attackOrigin.position + attackDirection * attackRange * 0.5f;
        Vector2 size = new Vector2(attackRange, attackWidth);

        // Kiểm tra xem có player nào trong phạm vi không
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, playerLayer);

        foreach (Collider2D hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }

        yield return new WaitForSeconds(attackDuration);

        animator.SetBool("IsAttacking", false);  // Kết thúc animation tấn công
    }

    // Đổi hướng tấn công của boss dựa trên vị trí của boss và người chơi
    private void SetAttackDirection()
    {
        if (player == null) return;  // Kiểm tra nếu player là null

        attackDirection = (player.position.x > transform.position.x) ? Vector2.right : Vector2.left;
    }

    // Vẽ vùng tấn công trong editor (chỉ khi chưa chạy game)
    private void OnDrawGizmos()
    {
        if (attackOrigin == null) return;

        Gizmos.color = Color.red;  // Màu sắc của vùng tấn công

        // Cập nhật hướng tấn công dựa trên vị trí của boss và người chơi
        SetAttackDirection();

        // Tính toán vị trí trung tâm của vùng tấn công
        Vector2 center = (Vector2)attackOrigin.position + attackDirection * attackRange * 0.5f;

        // Kích thước của vùng tấn công (dài và rộng)
        Vector2 size = new Vector2(attackRange, attackWidth);

        // Vẽ vùng tấn công theo hướng của attackDirection
        Gizmos.DrawWireCube(center, size);
    }
}
