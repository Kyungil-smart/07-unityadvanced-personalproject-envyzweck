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
        float targetXVelocity = _player.moveInput.x * _player.stats.speed;
        _player.rb.linearVelocity = new Vector2(targetXVelocity, _player.rb.linearVelocity.y);

        _player.animator.SetFloat("MoveSpeed", Mathf.Abs(_player.rb.linearVelocity.x));

        if (Mathf.Abs(_player.moveInput.x) < 0.01f)
            _player.StateMachine.ChangeState(_player.IdleState);

        if (_player.bufferedAttack)
        {
            _player.bufferedAttack = false;
            _player.StateMachine.ChangeState(_player.AttackState);
        }

        if (_player.jumpBufferCounter > 0 && _player.IsGrounded())
        {
            _player.StateMachine.ChangeState(_player.JumpState);
            _player.jumpBufferCounter = 0;
        }
    }
}