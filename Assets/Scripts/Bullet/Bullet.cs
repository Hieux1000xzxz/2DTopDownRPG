using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public Vector2 direction;

    private void Update()
    {
        // Di chuyển viên đạn
        transform.Translate(direction.normalized * speed * Time.deltaTime, Space.World);

        // Tính góc xoay theo hướng bay
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        // Lật sprite nếu bay sang trái
        Vector3 scale = transform.localScale;
        if (direction.x < 0)
            scale.y = -Mathf.Abs(scale.y); // lật theo trục X bằng cách đảo scale.y nếu xoay Z
        else
            scale.y = Mathf.Abs(scale.y);
        transform.localScale = scale;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(10); // Gây sát thương
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
