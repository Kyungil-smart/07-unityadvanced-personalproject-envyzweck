using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public StateMachine StateMachine { get; private set; }
    public Vector2 moveInput;
    public Rigidbody2D rb;
    [SerializeField]public float moveSpeed = 5f;
    [SerializeField]public float jumpForce = 7f;

    private PlayerInput _playerInput;
    
    public Transform groundCheck;
    [SerializeField]public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    
    [SerializeField]private float jumpBufferTime = 0.15f; 
    private float jumpBufferCounter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        StateMachine = new StateMachine();
        StateMachine.ChangeState(new IdleState(this));

        _playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        moveInput = _playerInput.actions["Move"].ReadValue<Vector2>();

        // 점프 입력 저장
        if (_playerInput.actions["Jump"].triggered)
        {
            jumpBufferCounter = jumpBufferTime;
        }

        // 버퍼 시간 내에 착지하면 점프 실행
        if (jumpBufferCounter > 0 && IsGrounded())
        {
            StateMachine.ChangeState(new JumpState(this));
            jumpBufferCounter = 0;
        }

        // 버퍼 시간 감소
        jumpBufferCounter -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        StateMachine.Update();
    }

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        return hit.collider != null;
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