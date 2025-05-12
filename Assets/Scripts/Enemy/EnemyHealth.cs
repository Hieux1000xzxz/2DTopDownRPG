using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    private GameManager gameManager;
    public bool IsDead => currentHealth <= 0; // Thuộc tính kiểm tra trạng thái chết
    public delegate void OnHealthChanged(int currentHealth, int maxHealth);
    public event OnHealthChanged HealthChanged;
    
    public GameObject deathPrefab; // Prefab để tạo hiệu ứng chết
    [SerializeField] private GameObject floatingTextPrefab; // Prefab cho text nổi
    
    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene!");
        }
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} bị trúng đòn! Máu còn lại: {currentHealth}");
        // Gọi sự kiện khi máu thay đổi
        HealthChanged?.Invoke(currentHealth, maxHealth);
        if (floatingTextPrefab != null)
        {
            ShowFloatingText(amount);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} đã chết!");
        // Thêm logic khi chết, ví dụ: animation, rơi vật phẩm, v.v.
        Instantiate(deathPrefab, transform.position, Quaternion.identity);
        gameManager.OnEnemyDefeated();

        Destroy(gameObject);
    }
    private void ShowFloatingText(int damage)
    {
        GameObject text = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        FloatingDamageText floating = text.GetComponent<FloatingDamageText>();
        floating.SetDamage(damage);
    }
}
