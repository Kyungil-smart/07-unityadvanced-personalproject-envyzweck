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
    [SerializeField] public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    [SerializeField] private float jumpBufferTime = 0.15f;
    private float jumpBufferCounter;

    // 콤보
    public int ComboStep = 0;
    private float _lastClickTime;
    public float comboDelay = 1f;
    public bool bufferedAttack = false;

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
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
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
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }
}