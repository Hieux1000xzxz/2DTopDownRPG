using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    public bool IsDead => currentHealth <= 0; // Thuộc tính kiểm tra trạng thái chết
    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public event OnHealthChanged HealthChanged;
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} bị trúng đòn! Máu còn lại: {currentHealth}");
        // Gọi sự kiện khi máu thay đổi
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} đã chết!");
        // Thêm logic khi chết, ví dụ: animation, rơi vật phẩm, v.v.
        Destroy(gameObject);
    }
}
