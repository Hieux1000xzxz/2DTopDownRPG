using System.Collections;
using UnityEngine;

public class BossSummonSkill : MonoBehaviour
{
    public GameObject summonPrefab;
    public Transform[] summonPoints;
    public Animator animator;
    public float summonDuration = 2f; // Thời gian animation Summon
    public Transform player;  // Mục tiêu là người chơi
    public bool IsSummoning = false;
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
                summonScript.target = player;
            }
        }

        // Chờ trong thời gian triệu hồi
        yield return new WaitForSeconds(summonDuration);

        // Kết thúc triệu hồi
        IsSummoning = false;
        animator.SetBool("IsSummoning", false);

    }
}
