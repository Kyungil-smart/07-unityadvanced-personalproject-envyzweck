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

        // 착지 직후 y속도 초기화
        _player.rb.linearVelocity = new Vector2(
            _player.rb.linearVelocity.x,
            0f
        );

        // 점프 힘 적용
        _player.rb.AddForce(Vector2.up * _player.jumpForce, ForceMode2D.Impulse);
    }

    public void Exit()
    {
        Debug.Log("Jump 상태 종료");
    }

    public void Update()
    {
        if (_player.IsGrounded())
        {
            if (_player.moveInput.x != 0)
                _player.StateMachine.ChangeState(new RunState(_player));
            else
                _player.StateMachine.ChangeState(new IdleState(_player));
        }
    }
}