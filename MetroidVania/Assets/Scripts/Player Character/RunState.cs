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
        _player.rb.linearVelocity = new Vector2(
            _player.moveInput.x * _player.moveSpeed,
            _player.rb.linearVelocity.y
        );
        
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