using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private GameObject floatingTextPrefab;
    private int currentHealth;
    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Player bị trúng đòn! Máu còn lại: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        if (floatingTextPrefab != null)
        {
            ShowFloatingText(amount);
        }
    }

    private void Die()
    {
        Debug.Log("Player chết rồi!");
        // Có thể thêm animation chết ở đây
        Destroy(gameObject);
    }
    private void ShowFloatingText(int damage)
    {
        GameObject text = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
        FloatingDamageText floating = text.GetComponent<FloatingDamageText>();
        floating.SetDamage(damage);
    }
}
