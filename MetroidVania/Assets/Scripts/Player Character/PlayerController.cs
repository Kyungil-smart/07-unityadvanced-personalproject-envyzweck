using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    public CharacterData characterData;
    public CharacterStats stats;
    
    public delegate void OnHealthChanged(int current, int max);
    public event OnHealthChanged HealthChanged;
    public delegate void OnManaChanged(int current, int max);
    public event OnManaChanged ManaChanged;

    public StateMachine StateMachine { get; private set; }
    public Vector2 moveInput;
    public Rigidbody2D rb;
    public Animator animator;
    private SpriteRenderer _spriteRenderer;

    [SerializeField] public float moveSpeed = 5f;
    [SerializeField] public float jumpForce = 7f;

    private PlayerInput _playerInput;

    public Transform groundCheck;
    [SerializeField] public Vector2 groundCheckBoxSize = new Vector2(0.3f, 0.05f);
    public LayerMask groundLayer;

    [SerializeField] private float jumpBufferTime = 0.15f;
    public float jumpBufferCounter;
    
    public float attackRange = 0.8f;   
    public float attackOffset = 0.5f;   
    [Range(0f, 0.5f)] public float attackStartTime = 0.1f;  
    [Range(0f, 0.5f)] public float attackActiveTime = 0.1f; 
    public LayerMask enemyLayer;

    // 콤보
    public int ComboStep = 0;
    private float _lastClickTime;
    public float comboDelay = 1f;
    public bool bufferedAttack = false;
    
    public bool IsAttackingFrame { get; set; }

    // 상태 캐싱
    public IdleState IdleState { get; private set; }
    public RunState RunState { get; private set; }
    public JumpState JumpState { get; private set; }
    public AttackState AttackState { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        //튀는 현상 방지
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        stats = new CharacterStats(characterData);
        StateMachine = new StateMachine();
        IdleState = new IdleState(this);
        RunState = new RunState(this);
        JumpState = new JumpState(this);
        AttackState = new AttackState(this);

        StateMachine.ChangeState(IdleState);
    }

    private void Update()
    {
        moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();
        // 캐릭터 플립
        if (moveInput.x > 0)
            _spriteRenderer.flipX = false;
        else if (moveInput.x < 0)
            _spriteRenderer.flipX = true;

        // 점프 입력 버퍼
        if (_playerInput.actions["Jump"].triggered)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        // 버퍼 내에 착지하면 점프
        if (jumpBufferCounter > 0 && IsGrounded())
        {
            StateMachine.ChangeState(JumpState);
            jumpBufferCounter = 0;
        }

        // 버퍼 시간 감소
        jumpBufferCounter -= Time.deltaTime;

        animator.SetBool("IsGrounded", IsGrounded());

        // 공격 입력
        if (_playerInput.actions["Attack"].triggered)
        {
            bufferedAttack = true;
        }
        
        StateMachine.Update();
    }
    
    public bool IsGrounded()
    {
        return Physics2D.OverlapBox(groundCheck.position, groundCheckBoxSize, 0f, groundLayer);
    }

    public void TakeDamage(int damage)
    {
        stats.currentHealth -= damage;
        if (stats.currentHealth < 0) stats.currentHealth = 0;

        HealthChanged?.Invoke(stats.currentHealth, stats.maxHealth);
    }

    public void Heal(int amount)
    {
        stats.currentHealth += amount;
        if (stats.currentHealth > stats.maxHealth) stats.currentHealth = stats.maxHealth;

        HealthChanged?.Invoke(stats.currentHealth, stats.maxHealth);
    }

    public void UseMana(int amount)
    {
        stats.currentMana -= amount;
        if (stats.currentMana < 0) stats.currentMana = 0;
        ManaChanged?.Invoke(stats.currentMana, stats.maxMana);
    }

    public void RecoverMana(int amount)
    {
        stats.currentMana += amount;
        if (stats.currentMana > stats.maxMana) stats.currentMana = stats.maxMana;
        ManaChanged?.Invoke(stats.currentMana, stats.maxMana);
    }

    private void OnDrawGizmosSelected()
    {
        // [MODIFIED] 0.5f 대신 실제 변수인 attackOffset을 사용합니다.
        float dir = (_spriteRenderer != null && _spriteRenderer.flipX) ? -1f : 1f;
        
        // 캐릭터의 중심에서 attackOffset만큼 떨어진 곳을 공격 중심으로 설정
        Vector3 attackPos = transform.position + new Vector3(dir * attackOffset, 0, 0);

        // 기본 가이드 라인 (하늘색 선)
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(attackPos, attackRange);

        // 공격 판정 프레임 시각화
        if (IsAttackingFrame)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f); // 반투명 빨간색
            Gizmos.DrawSphere(attackPos, attackRange);
        }
        
        // 지면 체크 박스도 시각화 해주면 좋습니다 (옵션)
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(groundCheck.position, groundCheckBoxSize);
        }
    }
}