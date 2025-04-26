using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 300;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Boss took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Boss died!");
        // Boss chết, có thể gọi animation chết hoặc destroy gameobject
        Destroy(gameObject);
    }
}
