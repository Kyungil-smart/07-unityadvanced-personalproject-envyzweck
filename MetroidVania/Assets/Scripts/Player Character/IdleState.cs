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
    }

    public void Exit()
    {
        Debug.Log("Idle 상태 종료");
    }

    public void Update()
    {
        if (_player.moveInput.x != 0)
        {
            _player.StateMachine.ChangeState(new RunState(_player));
        }
    }
}