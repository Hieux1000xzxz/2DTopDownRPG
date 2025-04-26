using UnityEngine;

public class BossDashSkill : MonoBehaviour
{
    public float dashSpeed = 10f;
    public float dashDuration = 0.75f;
    public float cooldown = 8f;
    private float lastDashTime = -Mathf.Infinity;

    public Animator animator;

    public bool IsDashing = false; // Thêm dòng này

    public void Execute(Vector3 targetPosition)
    {
        if (Time.time - lastDashTime < cooldown) return;

        StopAllCoroutines();
        StartCoroutine(DashTowards(targetPosition));
        lastDashTime = Time.time;
    }

    private System.Collections.IEnumerator DashTowards(Vector3 targetPosition)
    {
        animator.SetBool("IsDashing", true);
        IsDashing = true; // Khi bắt đầu dash

        Vector3 direction = (targetPosition - transform.position).normalized;
        float timer = 0;

        while (timer < dashDuration)
        {
            transform.position += direction * dashSpeed * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("IsDashing", false);
        IsDashing = false; // Khi kết thúc dash
    }

    public bool CanDash()
    {
        return Time.time - lastDashTime >= cooldown;
    }
}
