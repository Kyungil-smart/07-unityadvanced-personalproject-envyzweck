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
        // y축 초기화
        _player.rb.linearVelocity = new Vector2(_player.rb.linearVelocity.x, 0f);

        _player.rb.AddForce(Vector2.up * _player.jumpForce, ForceMode2D.Impulse);

        _player.animator.SetTrigger("Jump");
    }

    public void Exit()
    {
        Debug.Log("Jump 상태 종료");
    }

    public void Update()
    {
        float targetXVelocity = _player.moveInput.x * _player.stats.speed;
        _player.rb.linearVelocity = new Vector2(targetXVelocity, _player.rb.linearVelocity.y);

        _player.animator.SetFloat("MoveSpeed", Mathf.Abs(_player.rb.linearVelocity.x));

        // 공격 입력
        if (_player.bufferedAttack)
        {
            _player.bufferedAttack = false;
            _player.StateMachine.ChangeState(_player.AttackState);
        }

        // 점프 중일때 착지판정 안하기
        if (_player.rb.linearVelocity.y <= 0.01f && _player.IsGrounded())
        {
            if (Mathf.Abs(_player.moveInput.x) > 0.01f)
                _player.StateMachine.ChangeState(_player.RunState);
            else
                _player.StateMachine.ChangeState(_player.IdleState);
        }
    }
}