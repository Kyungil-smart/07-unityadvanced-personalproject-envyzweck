using UnityEngine;
using System.Collections;

public abstract class BaseMonster : MonoBehaviour, IDamageable
{
    public MonsterData data;
    protected Transform player;
    protected SpriteRenderer sr;
    protected Rigidbody2D rb;
    protected Animator animator;
    
    public float respawnDistance = 25f;
    public float deathRespawnTime = 5f;
    
    public MonsterSpawner mySpawner;

    [Header("Stats")]
    protected int currentHealth;
    protected Vector2 spawnPoint;
    protected bool isDead = false;
    
    [Header("배회")]
    public float wanderRange = 3f;    
    private int _wanderDirection = 1;    
    private float _leftBoundary;
    private float _rightBoundary;
    [SerializeField]private float  _chaseDistance = 3f;
    protected float currentMoveSpeed;
    
    private Coroutine _flashCoroutine;

    protected virtual void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    protected virtual void Start()
    {
        spawnPoint = transform.position;
        
        _leftBoundary = spawnPoint.x - wanderRange;
        _rightBoundary = spawnPoint.x + wanderRange;
        
        if (data != null) InitMonster();
    }
    
    protected virtual void Update()
    {
        if (isDead) return;
        
        CheckDistanceForRespawn();
        
        HandleAI();
    }

    protected virtual void InitMonster()
    {
        isDead = false;
        currentHealth = data.maxHealth;
        gameObject.SetActive(true);
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
        
        if (rb != null) 
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.simulated = true;
            rb.gravityScale = 1;
        }
        
        if (sr != null) sr.color = Color.white;
    }

    private void CheckDistanceForRespawn()
    {
        if (player == null) return;
        float dist = Vector2.Distance((Vector2)transform.position, (Vector2)player.position);
        
        // 멀어지면 원위치로 리스폰
        if (dist > respawnDistance) InstantRespawn();
    }

    public virtual void InstantRespawn()
    {
        transform.position = new Vector3(spawnPoint.x, spawnPoint.y, transform.position.z);
        InitMonster();
    }
    protected void Move(float direction, float speed)
    {
        if (isDead) return;

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
            
            if (animator != null)
            {
                float moveSpeed = Mathf.Abs(rb.linearVelocity.x);
                animator.SetFloat("Speed", moveSpeed);
            }
        }
    }
    
    protected virtual void HandleAI()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // 감지 범위
        if (dist <= _chaseDistance)
        {
            currentMoveSpeed = data.moveSpeed * 1.3f; 
            ChasePlayer();
        }
        else
        {
            currentMoveSpeed = data.moveSpeed * 0.6f; 
            Wander();
        }
        
    }
    protected virtual void ChasePlayer()
    {
        if (isDead) return;
        float moveDir = (player.position.x > transform.position.x) ? 1 : -1;
        Move(moveDir, currentMoveSpeed);
        SetFlip(player.position.x < transform.position.x);
    }
    protected virtual void Wander()
    {
        if (isDead) return;
        Move(_wanderDirection, currentMoveSpeed);
        
        if (transform.position.x >= _rightBoundary)
        {
            _wanderDirection = -1;
            SetFlip(true); 
        }
        else if (transform.position.x <= _leftBoundary)
        {
            _wanderDirection = 1;
            SetFlip(false); 
        }
    }
    private void SetFlip(bool shouldFlip)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.flipX = shouldFlip;
    }
    
    public virtual void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;
        
        if (currentHealth > 0)
        {
            if (animator != null) animator.SetTrigger("Hurt");
        
            if (_flashCoroutine != null) StopCoroutine(_flashCoroutine);
            _flashCoroutine = StartCoroutine(HurtFlashRoutine());
        }
        else
        {
            Die(); 
        }
    }
    
    private IEnumerator HurtFlashRoutine()
    {
        if (sr == null) yield break;
        sr.color = new Color(1f, 0.5f, 0.5f);
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    protected virtual void Die()
    {
        if (isDead) return;
        isDead = true;
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
            rb.simulated = false;
        }
        
        
        if (animator != null)
        {
            animator.SetFloat("Speed", 0f); 
            animator.ResetTrigger("Hurt"); 
            animator.SetTrigger("Dead");
        }

        // 몬스터가 죽으면 관리하기 위해 필요
        if (mySpawner != null) mySpawner.OnMonsterRemoved();
        StartCoroutine(DeathRoutine());
    }
    
    private IEnumerator DeathRoutine()
    {
        if (rb != null) 
        {
            rb.bodyType = RigidbodyType2D.Static; 
        }
        
        yield return new WaitForSeconds(1.5f);
        
        if (MonsterManager.Instance != null)
            MonsterManager.Instance.RegisterDeadMonster(this, deathRespawnTime);
    
        gameObject.SetActive(false);
    }

    public virtual void Heal(int amount)
    {
        if (isDead) return;
        
        currentHealth += amount;
        
        if (currentHealth > data.maxHealth)
        {
            currentHealth = data.maxHealth;
        }

    }
}