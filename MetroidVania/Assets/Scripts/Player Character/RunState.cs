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
    }

    public void Exit()
    {
    }

    public void Update()
    {
        // 캐릭터 스탯 적용
        _player.rb.linearVelocity = new Vector2(
            _player.moveInput.x * _player.stats.speed,
            _player.rb.linearVelocity.y
        );
        Debug.Log($"현재 이동속도: {_player.stats.speed}");

        _player.animator.SetFloat("MoveSpeed", Mathf.Abs(_player.rb.linearVelocity.x));

        if (Mathf.Abs(_player.moveInput.x) < 0.01f)
        {
            _player.StateMachine.ChangeState(_player.IdleState);
        }

        if (_player.bufferedAttack)
        {
            _player.bufferedAttack = false;
            _player.StateMachine.ChangeState(_player.AttackState);
        }
    }
}