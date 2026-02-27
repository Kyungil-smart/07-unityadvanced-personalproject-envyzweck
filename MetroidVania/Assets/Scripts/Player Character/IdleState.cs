using UnityEngine;

public class IdleState : IState
{
    private PlayerController _player;

    public IdleState(PlayerController player)
    {
        _player = player;
    }

    public void Enter()
    {
        Debug.Log("Idle 상태 진입");
        _player.animator.SetFloat("MoveSpeed", 0f);
    }

    public void Exit()
    {
        Debug.Log("Idle 상태 종료");
    }

    public void Update()
    {
        if (Mathf.Abs(_player.moveInput.x) > 0.01f)
        {
            _player.StateMachine.ChangeState(_player.RunState);
        }
        if (_player.bufferedAttack)
        {
            _player.bufferedAttack = false;
            _player.StateMachine.ChangeState(_player.AttackState);
        }
    }
}