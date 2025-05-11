using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    [Header("Hiệu ứng di chuyển")]
    public float moveSpeed = 2.5f;
    public float lifetime = 1.0f;
    public float fadeStartTime = 0.6f;

    [Header("Hiệu ứng hiển thị")]
    public float startScale = 0.5f;
    public float maxScale = 1.2f;
    public float scaleTime = 0.2f;

    [Header("Màu sắc")]
    public Color damageColor = Color.red;
    public Color criticalColor = new Color(1f, 0.5f, 0f); // Màu cam cho đòn chí mạng

    private TextMeshPro textMesh;
    private float timeLived = 0f;
    private Vector3 moveDirection;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        if (textMesh == null)
        {
            Debug.LogError("TextMeshPro component không tìm thấy!");
            Destroy(gameObject);
            return;
        }

        // Thiết lập ban đầu
        transform.localScale = Vector3.one * startScale;

        // Tạo hướng di chuyển ngẫu nhiên (chủ yếu đi lên)
        float randomX = Random.Range(-0.7f, 0.7f);
        moveDirection = new Vector3(randomX, 1f, 0);
    }

    private void Update()
    {
        // Tăng thời gian
        timeLived += Time.deltaTime;

        // Di chuyển text lên trên
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        // Hiệu ứng phóng to ban đầu
        if (timeLived < scaleTime)
        {
            float scaleProgress = timeLived / scaleTime;
            float currentScale = Mathf.Lerp(startScale, maxScale, scaleProgress);
            transform.localScale = Vector3.one * currentScale;
        }
        else if (timeLived < scaleTime * 2)
        {
            // Co nhỏ lại về kích thước bình thường
            float scaleProgress = (timeLived - scaleTime) / scaleTime;
            float currentScale = Mathf.Lerp(maxScale, 1.0f, scaleProgress);
            transform.localScale = Vector3.one * currentScale;
        }

        // Bắt đầu mờ dần
        if (timeLived >= fadeStartTime)
        {
            float fadeProgress = (timeLived - fadeStartTime) / (lifetime - fadeStartTime);
            Color color = textMesh.color;
            color.a = Mathf.Lerp(1f, 0f, fadeProgress);
            textMesh.color = color;
        }

        // Hủy sau khi hết thời gian
        if (timeLived >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    // Thiết lập damage và hiển thị
    public void SetDamage(int damage, bool isCritical = false)
    {
        if (textMesh != null)
        {
            textMesh.text = " - " + damage.ToString();
            textMesh.color = isCritical ? criticalColor : damageColor;

            // Nếu là đòn chí mạng, tăng kích thước
            if (isCritical)
            {
                maxScale = 1.5f;
                moveSpeed *= 1.2f;
            }
        }
    }

    // Phương thức đơn giản chỉ nhận damage
    public void SetDamage(int damage)
    {
        // Ngẫu nhiên có 20% cơ hội là đòn chí mạng để demo
        bool isCritical = Random.value < 0.2f;
        SetDamage(damage, isCritical);
    }
}