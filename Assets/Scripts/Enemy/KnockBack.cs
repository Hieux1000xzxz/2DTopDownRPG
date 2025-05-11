using UnityEngine;

public class KnockBack : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float knockBackForce = 5f; // Lực đẩy lùi
    [SerializeField] private float knockBackDuration = 0.2f; // Thời gian đẩy lùi

    private Rigidbody2D rb;
    public bool isKnockedBack = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockBack(Vector2 sourcePosition)
    {
        if (isKnockedBack) return;

        isKnockedBack = true;

        // Tính toán hướng đẩy lùi
        Vector2 knockBackDirection = (transform.position - (Vector3)sourcePosition).normalized;

        // Áp dụng lực đẩy lùi bằng AddForce
        rb.AddForce(knockBackDirection * knockBackForce, ForceMode2D.Impulse);

        // Dừng đẩy lùi sau một khoảng thời gian
        Invoke(nameof(StopKnockBack), knockBackDuration);
    }

    private void StopKnockBack()
    {
        rb.Sleep(); // Đảm bảo dừng chuyển động
        isKnockedBack = false;
    }
}
