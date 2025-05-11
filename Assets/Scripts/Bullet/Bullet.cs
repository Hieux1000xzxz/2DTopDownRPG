using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public Vector2 direction;

    private void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        // Xoay viên đạn theo hướng bay (sprite gốc hướng theo trục Y)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(15); // Gây sát thương
        }
        // TODO: xử lý khi trúng kẻ địch nếu cần
        KnockBack knockBack = collision.GetComponent<KnockBack>();
        if (knockBack != null)
        {
            knockBack.ApplyKnockBack(transform.position); // Đẩy lùi từ vị trí đạn
        }
        Flash flash = collision.GetComponent<Flash>();
        if (flash != null)
        {
            flash.StartFlash();
        }
        Destroy(gameObject);
    }

    private void Start()
    {
        Destroy(gameObject, 4f); // tự hủy sau 3 giây
    }
  
}
