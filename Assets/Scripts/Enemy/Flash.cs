using UnityEngine;
using System.Collections;

public class Flash : MonoBehaviour
{
    [Header("Flash Settings")]
    [SerializeField] private Material whiteFlashMaterial;
    [SerializeField] private float flashDuration = 0.1f;

    private Material originalMaterial;
    private SpriteRenderer spriteRenderer;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.sharedMaterial;
        }
    }

    public void StartFlash()
    {
        if (spriteRenderer == null || whiteFlashMaterial == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.material = whiteFlashMaterial;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.material = originalMaterial;
    }
}
