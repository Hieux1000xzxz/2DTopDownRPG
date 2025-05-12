using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private GameObject floatingTextPrefab;
    [SerializeField] private Slider healthSlider; // Gán từ Inspector hoặc tự động tìm
    private int currentHealth;
    public static PlayerHealth Instance;

    private void Awake()
    {
       
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Tìm lại healthSlider trong scene mới (nếu không dùng DontDestroyOnLoad cho UI)
        healthSlider = GameObject.FindGameObjectWithTag("HealthBar")?.GetComponent<Slider>();
        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"Player nhận {amount} sát thương. Máu còn: {currentHealth}");

        if (floatingTextPrefab != null)
        {
            ShowFloatingText(amount);
        }

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player đã chết!");
        GameManager.Instance?.GameOver(); // Kiểm tra null để tránh lỗi
        Destroy(gameObject); // Hủy nhân vật
    }

    private void ShowFloatingText(int damage)
    {
        var text = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        text.GetComponent<FloatingDamageText>()?.SetDamage(damage);
    }

    private void UpdateHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện để tránh memory leak
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}