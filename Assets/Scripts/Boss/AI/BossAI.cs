using UnityEngine;

public class BossAI : MonoBehaviour
{
    public Transform target; // Mục tiêu (người chơi)
    public float attackRange = 2f;
    public float summonCooldown = 10f;
    private float lastSummonTime;
    private float attackCooldown = 4f;
    private float lastAttackTime;

    public BossDashSkill dashSkill;
    public BossAttackSkill attackSkill;
    public BossSummonSkill summonSkill;
    public Animator animator;

    private void Update()
    {
        // Kiểm tra xem target có null không (tránh lỗi khi không có mục tiêu)
        if (target == null) return;

        // Kiểm tra trạng thái triệu hồi
        if (summonSkill.IsSummoning)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        bool didSomething = false;

        // Ưu tiên triệu hồi
        if (Time.time - lastSummonTime >= summonCooldown)
        {
            summonSkill.Execute();
            lastSummonTime = Time.time;
            didSomething = true;
        }

        // Tấn công nếu gần và không đang lướt
        if (!didSomething && !dashSkill.IsDashing && distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            attackSkill.Execute();
            lastAttackTime = Time.time;
            didSomething = true;
        }

        // Lướt nếu ngoài tầm tấn công và có thể dash
        if (!didSomething && distance > attackRange && dashSkill.CanDash())
        {
            dashSkill.Execute(target.position);
            didSomething = true;
        }

        // Nếu chưa làm gì, tiếp tục đi bộ theo người chơi
        if (!didSomething)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            // Tính toán vị trí đích sao cho boss dừng lại khi còn cách người chơi 1 đơn vị
            float targetDistance = 2f; // Khoảng cách cần giữ
            if (distance > targetDistance)
            {
                Vector3 targetPosition = target.position - direction * targetDistance;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 2f);
            }

            // Lật người theo hướng di chuyển
            FlipBasedOnMovement(target.position.x);
        }
    }

    private void FlipBasedOnMovement(float targetXPosition)
    {
        if (target == null) return;

        bool isMovingRight = targetXPosition > transform.position.x;

        if (isMovingRight && transform.localScale.x < 0)
        {
            Flip();
        }
        else if (!isMovingRight && transform.localScale.x > 0)
        {
            Flip();
        }
    }

    private void Flip()
    {
        // Lật sprite theo trục X
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
