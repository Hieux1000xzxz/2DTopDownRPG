using System.Collections;
using UnityEngine;

public class BossSummonSkill : MonoBehaviour
{
    public GameObject summonPrefab;
    public Transform[] summonPoints;
    public Animator animator;
    public float summonDuration = 2f; // Thời gian animation Summon
    private Transform target;  // Mục tiêu là người chơi
    public bool IsSummoning = false;

    private void Start()
    {
        // Tìm kiếm đối tượng có tag "Player" khi bắt đầu
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }
    }

    public void Execute()
    {
        StopAllCoroutines();
        StartCoroutine(SummonRoutine());
    }

    private IEnumerator SummonRoutine()
    {
        IsSummoning = true;
        animator.SetBool("IsSummoning", true);

        // Gọi quái ra ngay lập tức hoặc delay vài frame tùy bạn
        foreach (Transform point in summonPoints)
        {
            GameObject summonedObject = Instantiate(summonPrefab, point.position, Quaternion.identity);

            // Gán mục tiêu là người chơi cho vật triệu hồi
            SummonObject summonScript = summonedObject.GetComponent<SummonObject>();
            if (summonScript != null)
            {
                summonScript.target = target;
            }
        }

        // Chờ trong thời gian triệu hồi
        yield return new WaitForSeconds(summonDuration);

        // Kết thúc triệu hồi
        IsSummoning = false;
        animator.SetBool("IsSummoning", false);

    }
}
