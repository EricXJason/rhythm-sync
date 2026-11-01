using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: CameraBeatPulse.cs
/// 所屬模組: Visuals
/// 責任描述: 攝影機節奏律動控制器。訂閱 BeatPulseEvent 重音脈衝，
///           微幅縮放 Camera 正交視野 (Orthographic Size)，達成沉浸式節拍震撼感。
/// 架構分層: Visuals / Camera
/// 聯動關係:
///   - 事件監聽: EventBus (BeatPulseEvent)
/// ============================================================================
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraBeatPulse : MonoBehaviour
{
    [SerializeField] private float baseOrthoSize = 6f;
    [SerializeField] private float pulseAmount = 0.3f;
    [SerializeField] private float returnSpeed = 8f;

    private Camera cam;
    private float targetSize;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        targetSize = baseOrthoSize;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<BeatPulseEvent>(OnBeatPulse);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<BeatPulseEvent>(OnBeatPulse);
    }

    private void OnBeatPulse(BeatPulseEvent evt)
    {
        // 遇到重音節拍時瞬間推近視野
        targetSize = baseOrthoSize - pulseAmount * Mathf.Clamp01(evt.Intensity * 2f);
    }

    private void Update()
    {
        if (cam == null) return;

        // 平滑回彈至預設大小
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * 20f);
        targetSize = Mathf.Lerp(targetSize, baseOrthoSize, Time.deltaTime * returnSpeed);
    }
}
