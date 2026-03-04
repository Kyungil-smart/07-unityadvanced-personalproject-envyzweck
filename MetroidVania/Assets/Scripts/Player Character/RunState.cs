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
        // 캐릭터 스탯 적용 (지상 이동)
        _player.rb.linearVelocity = new Vector2(
            _player.moveInput.x * _player.stats.speed,
            _player.rb.linearVelocity.y
        );

        // Animator에 이동 속도 전달
        _player.animator.SetFloat("MoveSpeed", Mathf.Abs(_player.rb.linearVelocity.x));

        // Idle 전환 조건
        if (Mathf.Abs(_player.moveInput.x) < 0.01f)
        {
            _player.StateMachine.ChangeState(_player.IdleState);
        }

        // 공격 입력 처리
        if (_player.bufferedAttack)
        {
            _player.bufferedAttack = false;
            _player.StateMachine.ChangeState(_player.AttackState);
        }

        // 점프 입력 처리
        if (_player.jumpBufferCounter > 0 && _player.IsGrounded())
        {
            _player.StateMachine.ChangeState(_player.JumpState);
            _player.jumpBufferCounter = 0;
        }
    }
}