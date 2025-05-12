using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class CameraTargetSetter : MonoBehaviour
{
    private CinemachineCamera virtualCamera;


    void OnEnable()
    {
        // Đăng ký sự kiện khi scene được load
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Hàm này được gọi mỗi khi scene mới được load
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        virtualCamera = FindAnyObjectByType<CinemachineCamera>();
        if (virtualCamera != null)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }

    }

}