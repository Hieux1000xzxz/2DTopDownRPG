using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    // Singleton instance
    public static PlayerManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject playerPrefab;
    private GameObject currentPlayer;

    private void Awake()
    {
        // Singleton pattern implementation
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Find spawn point (optional)
        GameObject spawnPoint = GameObject.FindGameObjectWithTag("PlayerSpawn");
        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.transform.position : new Vector3(0, 0, 0);

        // Check if player already exists in the scene
        PlayerController existingPlayer = FindAnyObjectByType<PlayerController>();

        if (existingPlayer != null)
        {
            currentPlayer = existingPlayer.gameObject;
            // Move player to spawn point if available
            currentPlayer.transform.position = spawnPosition;
        }
        else if (playerPrefab != null)
        {
            // If no player exists, instantiate one from the prefab
            currentPlayer = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Player instantiated at scene load: " + scene.name);
        }
        else
        {
            Debug.LogError("PlayerPrefab is not assigned in PlayerManager!");
        }

        // Make sure the player stays in this scene and inform any listeners
        if (currentPlayer != null)
        {
            SceneManager.MoveGameObjectToScene(currentPlayer, scene);

            // Broadcast that player is ready in scene
            currentPlayer.SendMessage("OnPlayerSpawned", SendMessageOptions.DontRequireReceiver);
        }
    }

    // Public method to get the current player reference
    public GameObject GetCurrentPlayer()
    {
        return currentPlayer;
    }
}