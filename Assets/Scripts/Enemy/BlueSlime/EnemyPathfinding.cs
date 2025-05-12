using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathfinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private KnockBack knockBack;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        knockBack = GetComponent<KnockBack>();
    }

    private void FixedUpdate() {
        // Không di chuyển nếu đang bị knockback
        if (knockBack != null && knockBack.isKnockedBack)
        {
            return;
        }
        rb.MovePosition(rb.position + moveDir * (moveSpeed * Time.fixedDeltaTime));
    }

    public void MoveTo(Vector2 targetPosition) {
        moveDir = targetPosition;
    }
}
