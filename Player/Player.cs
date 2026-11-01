using UnityEngine;

/// <summary>
/// ============================================================================
/// 檔案名稱: Player.cs
/// 所屬模組: Player
/// 責任描述: 跑酷玩家主實體控制器。負責快取物理組件、射線檢測、輸入狀態接收，
///           並驅動純 C# 有限狀態機 (FSM) 進行行為狀態流轉。
/// 架構分層: Player / Controller
/// 聯動關係:
///   - 狀態相依: PlayerStateMachine, IPlayerState (Run, Jump, Slide, Ledge, Knockback, Dead)
///   - 事件發布: EventBus (PlayerDamagedEvent, PlayerDiedEvent)
/// ----------------------------------------------------------------------------
/// 【Unity 場景掛載指引】
/// 1. 掛載物件: 場景中的主角 GameObject，命名為「Player」。
/// 2. 必備組件: Rigidbody2D, BoxCollider2D, Animator, SpriteRenderer。
/// ============================================================================
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    [Header("Movement & Speed Settings")]
    [SerializeField] private float baseMoveSpeed = 8f;
    [SerializeField] private float maxMoveSpeed = 20f;
    [SerializeField] private float speedMultiplier = 1.05f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float doubleJumpForce = 12f;

    [Header("Slide Settings")]
    [SerializeField] private float slideSpeed = 12f;
    [SerializeField] private float slideDuration = 0.6f;
    [SerializeField] private float slideCooldown = 0.5f;
    [SerializeField] private float slideColliderHeight = 0.5f;

    [Header("Ground & Ledge Raycast Checks")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private LayerMask ledgeLayer;

    [Header("Knockback Settings")]
    [SerializeField] private Vector2 knockbackForce = new Vector2(-4f, 6f);
    [SerializeField] private float knockbackDuration = 0.4f;

    // 核心組件快取
    public Rigidbody2D Rb { get; private set; }
    public BoxCollider2D Coll { get; private set; }
    public Animator Anim { get; private set; }
    public SpriteRenderer Sr { get; private set; }

    // 狀態機與狀態實例
    private PlayerStateMachine stateMachine;
    public PlayerRunState RunState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerSlideState SlideState { get; private set; }
    public PlayerLedgeState LedgeState { get; private set; }
    public PlayerKnockbackState KnockbackState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }

    // 輸入快取狀態
    public bool InputJump { get; private set; }
    public bool InputSlide { get; private set; }

    // 運行狀態變數
    public float CurrentMoveSpeed { get; private set; }
    public float NormalColliderHeight { get; private set; }
    public float NormalGravityScale { get; private set; }
    public Vector3 LedgeTargetPosition { get; private set; }
    public int CurrentHealth { get; private set; } = 3;
    public int CurrentScore { get; private set; } = 0;
    public float DistanceTraveled => transform.position.x;

    public float JumpForce => jumpForce;
    public float DoubleJumpForce => doubleJumpForce;
    public float SlideSpeed => slideSpeed;
    public float SlideDuration => slideDuration;
    public float SlideColliderHeight => slideColliderHeight;
    public Vector2 KnockbackForce => knockbackForce;
    public float KnockbackDuration => knockbackDuration;

    private float slideCooldownTimer = 0f;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Coll = GetComponent<BoxCollider2D>();
        Anim = GetComponent<Animator>();
        Sr = GetComponent<SpriteRenderer>();

        NormalColliderHeight = Coll.size.y;
        NormalGravityScale = Rb.gravityScale;
        CurrentMoveSpeed = baseMoveSpeed;

        // 初始化狀態機與各狀態
        stateMachine = new PlayerStateMachine();
        RunState = new PlayerRunState(this, stateMachine);
        JumpState = new PlayerJumpState(this, stateMachine);
        SlideState = new PlayerSlideState(this, stateMachine);
        LedgeState = new PlayerLedgeState(this, stateMachine);
        KnockbackState = new PlayerKnockbackState(this, stateMachine);
        DeadState = new PlayerDeadState(this, stateMachine);
    }

    private void Start()
    {
        stateMachine.Initialize(RunState);
    }

    private void Update()
    {
        slideCooldownTimer -= Time.deltaTime;
        HandleKeyboardInput();
        stateMachine.Update();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    private void HandleKeyboardInput()
    {
        // 鍵盤輸入相容 (PC 測試或模擬器)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            TriggerJumpInput();
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            TriggerSlideInput();
        }
    }

    /// <summary>
    /// 由外部 UI 跳躍按鈕 (TouchJumpButton) 或鍵盤觸發跳躍
    /// </summary>
    public void TriggerJumpInput()
    {
        InputJump = true;
    }

    /// <summary>
    /// 由外部 UI 滑行按鈕 (TouchSlideButton) 或鍵盤觸發滑行
    /// </summary>
    public void TriggerSlideInput()
    {
        InputSlide = true;
    }

    public void ConsumeJumpInput() => InputJump = false;
    public void ConsumeSlideInput() => InputSlide = false;

    public bool IsGrounded()
    {
        if (groundCheck == null) return true;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public bool CheckLedge()
    {
        if (ledgeCheck == null) return false;
        RaycastHit2D hit = Physics2D.Raycast(ledgeCheck.position, Vector2.right, 0.4f, ledgeLayer);
        if (hit.collider != null)
        {
            LedgeTargetPosition = hit.point + new Vector2(0.5f, 1.2f);
            return true;
        }
        return false;
    }

    public bool CanSlide() => slideCooldownTimer <= 0f && IsGrounded();
    public void ResetSlideCooldown() => slideCooldownTimer = slideCooldown;

    public void SetColliderHeight(float height)
    {
        Vector2 size = Coll.size;
        size.y = height;
        Coll.size = size;
    }

    public void TakeDamage(int damage = 1)
    {
        if (stateMachine.CurrentState == DeadState || stateMachine.CurrentState == KnockbackState) return;

        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            stateMachine.ChangeState(DeadState);
        }
        else
        {
            stateMachine.ChangeState(KnockbackState);
        }
    }

    public void ResetKnockbackInvincibility()
    {
        // 擊退結束後短暫閃爍或無敵邏輯
    }

    public void AddScore(int amount)
    {
        CurrentScore += amount;
        EventBus.Publish(new ScoreChangedEvent(CurrentScore, 1));
    }
}
