using UnityEngine;

public class JumpState : IState
{
    private PlayerController _player;

    public JumpState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("Jump 상태 진입");

        // 착지 직후 떨어지는 속도 초기화
        _player.rb.linearVelocity = new Vector2(
            _player.rb.linearVelocity.x,
            0f
        );

        // 점프 힘 적용
        _player.rb.AddForce(Vector2.up * _player.jumpForce, ForceMode2D.Impulse);

        Debug.Log($"JumpForce: {_player.jumpForce}, CurrentYVelocity: {_player.rb.linearVelocity.y}");

        _player.animator.SetTrigger("Jump");
    }

    public void Exit()
    {
        Debug.Log("Jump 상태 종료");
    }

    public void Update()
    {
        // 공중 이동 처리
        _player.rb.linearVelocity = new Vector2(
            _player.moveInput.x * _player.stats.speed,
            _player.rb.linearVelocity.y
        );

        Debug.Log($"[Jump] Applied Force: {_player.jumpForce}, Current Velocity: {_player.rb.linearVelocity}");

        _player.animator.SetFloat("MoveSpeed", Mathf.Abs(_player.rb.linearVelocity.x));

        // 공격 입력
        if (_player.bufferedAttack)
        {
            _player.bufferedAttack = false;
            _player.StateMachine.ChangeState(_player.AttackState);
        }

        // 착지 판정
        if (_player.rb.linearVelocity.y <= 0 && _player.IsGrounded())
        {
            if (Mathf.Abs(_player.moveInput.x) > 0.01f)
                _player.StateMachine.ChangeState(_player.RunState);
            else
                _player.StateMachine.ChangeState(_player.IdleState);
        }
    }
}