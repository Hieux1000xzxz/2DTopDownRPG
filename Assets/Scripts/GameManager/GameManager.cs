using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject levelCompleteUI;
    public GameObject gameOverUI;

    public bool allEnemiesDefeated = false;
    private bool isGameOver = false;

    private int totalEnemies; // Thêm biến để theo dõi tổng số quái vật còn sống

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Đếm số lượng quái vật còn sống khi bắt đầu trò chơi
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    public void OnEnemyDefeated()
    {
        totalEnemies--; // Giảm số lượng quái vật khi một con bị tiêu diệt

        if (totalEnemies <= 0)
        {
            allEnemiesDefeated = true;
            Debug.Log("Tất cả quái đã bị tiêu diệt!");
        }
    }

    public void TryCompleteLevel()
    {
        if (allEnemiesDefeated)
        {
            Debug.Log("Qua màn thành công!");
            ShowLevelCompleteUI();
        }
        else
        {
            Debug.Log("Chưa tiêu diệt hết quái!");
        }
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Game Over!");
        ShowGameOverUI();
    }

    private void ShowLevelCompleteUI()
    {
        if (levelCompleteUI != null)
            levelCompleteUI.SetActive(true);
        Time.timeScale = 0f; // Tạm dừng game
    }

    private void ShowGameOverUI()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ReloadLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextIndex);
        else
            Debug.Log("Bạn đã hoàn thành tất cả màn chơi!");
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
