using UnityEngine;

public class RunState : IState
{
    private PlayerController _player;

    public RunState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("Run 상태 진입");
    }

    public void Exit()
    {
        Debug.Log("Run 상태 종료");
    }

    public void Update()
    {
        // 이동 처리 (linearVelocity 사용)
        _player.rb.linearVelocity = new Vector2(
            _player.moveInput.x * _player.moveSpeed,
            _player.rb.linearVelocity.y
        );

        // 입력이 없으면 Idle로 전환
        if (_player.moveInput.x == 0)
        {
            _player.StateMachine.ChangeState(new IdleState(_player));
        }
    }
}