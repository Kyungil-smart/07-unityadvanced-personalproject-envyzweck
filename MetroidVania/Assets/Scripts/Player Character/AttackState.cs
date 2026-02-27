using UnityEngine;

public class AttackState : IState
{
    private PlayerController _player;
    private Animator _animator;
    private Rigidbody2D _rb;

    private float _attackDuration = 1.2f;
    private float _timer;

    private bool _comboWindowOpen;

    public AttackState(PlayerController player)
    {
        _player = player;
        _animator = player.animator;
        _rb = player.rb;
    }

    public void Enter()
    {
        // 첫 공격이면 1부터 시작
        if (_player.ComboStep == 0)
            _player.ComboStep = 1;

        _animator.SetInteger("Combo", _player.ComboStep);
        _animator.SetTrigger("Attack");

        _timer = 0f;
        _comboWindowOpen = false;
    }

    public void Update()
    {
        _timer += Time.deltaTime;

        // 이동 허용 (공격 중 이동 가능 구조)
        float moveSpeed = _player.moveInput.x * _player.moveSpeed;
        _rb.linearVelocity = new Vector2(moveSpeed, _rb.linearVelocity.y);

        // 콤보 입력 가능 구간
        if (_timer >= 0.2f)
            _comboWindowOpen = true;

        // 콤보 입력
        if (_comboWindowOpen && _player.bufferedAttack)
        {
            _player.bufferedAttack = false;
            
            if (_player.ComboStep < 3)
            {
                _player.ComboStep++;
                _player.StateMachine.ChangeState(_player.AttackState);
                return;
            }
        }

        // 공격 종료 (3타 끝나면 초기화)
        if (_timer >= _attackDuration)
        {
            _player.ComboStep = 0;
            _player.StateMachine.ChangeState(_player.IdleState);
        }
    }

    public void Exit()
    {
        _animator.ResetTrigger("Attack");
    }
}