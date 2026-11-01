using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: ParallaxBackground.cs
/// 所屬模組: Visuals
/// 責任描述: 2D 視差背景滾動效果器。根據攝影機位移與自訂視差乘數 (Parallax Effect)
///           驅動多層灰階背景無限無縫滾動。
/// 架構分層: Visuals / Background
/// ============================================================================
/// </summary>
public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private float parallaxMultiplier = 0.5f;

    private float textureUnitSizeX;
    private Vector3 lastCameraPosition;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera != null)
        {
            lastCameraPosition = targetCamera.transform.position;
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            textureUnitSizeX = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
        }
        else
        {
            textureUnitSizeX = 20f;
        }
    }

    private void LateUpdate()
    {
        if (targetCamera == null) return;

        Vector3 deltaMovement = targetCamera.transform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxMultiplier, 0f, 0f);
        lastCameraPosition = targetCamera.transform.position;

        // 無縫無窮循環判定
        if (Mathf.Abs(targetCamera.transform.position.x - transform.position.x) >= textureUnitSizeX)
        {
            float offsetPositionX = (targetCamera.transform.position.x - transform.position.x) % textureUnitSizeX;
            transform.position = new Vector3(targetCamera.transform.position.x + offsetPositionX, transform.position.y, transform.position.z);
        }
    }
}
