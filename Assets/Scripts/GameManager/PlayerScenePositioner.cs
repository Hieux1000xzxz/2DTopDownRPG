using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScenePositioner : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Tìm điểm spawn trong scene mới
        GameObject spawnPoint = GameObject.FindWithTag("PlayerSpawn");

        if (spawnPoint != null && PlayerHealth.Instance != null)
        {
            PlayerHealth.Instance.transform.position = spawnPoint.transform.position;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy Player hoặc PlayerSpawn trong scene.");
        }
    }
}
