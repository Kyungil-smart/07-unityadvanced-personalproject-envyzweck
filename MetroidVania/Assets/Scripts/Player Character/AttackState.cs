using UnityEngine;

public class AttackState : IState
{
    private PlayerController _player;
    private Animator _animator;
    private Rigidbody2D _rb;

    private float _attackDuration = 0.5f;
    private float _timer;
    private bool _comboWindowOpen;
    private bool _hasDealtDamage;

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
        _hasDealtDamage = false;
    }

    public void Update()
    {
        _timer += Time.deltaTime;

        // 이동 허용 (공격 중 이동 가능 구조)
        float moveSpeed = _player.moveInput.x * (_player.moveSpeed * 0.5f); 
        _rb.linearVelocity = new Vector2(moveSpeed, _rb.linearVelocity.y);

        // 공격 판정 시간
        float startTime = _player.attackStartTime;
        float endTime = startTime + _player.attackActiveTime;
        
        if (_timer >= startTime && _timer <= endTime)
        {
            // 기즈모 빨간불
            _player.IsAttackingFrame = true; 
            
            if (!_hasDealtDamage)
            {
                PerformAttackDetection();
            }
        }
        else
        {
            _player.IsAttackingFrame = false; 
        }
        
        // 콤보 입력 가능 구간
        if (_timer >= endTime)
            _comboWindowOpen = true;

        // 콤보 입력
        if (_comboWindowOpen && _player.bufferedAttack)
        {
            _player.bufferedAttack = false;
            
            if (_player.ComboStep < 3)
            {
                _player.ComboStep++;
                _player.IsAttackingFrame = false;
                _player.StateMachine.ChangeState(_player.AttackState);
                return;
            }
        }

        // 공격 종료 (3타 끝나면 초기화)
        if (_timer >= _attackDuration)
        {
            _player.ComboStep = 0;
            _player.IsAttackingFrame = false;
            _player.StateMachine.ChangeState(_player.IdleState);
        }
    }

    private void PerformAttackDetection()
    {
        _hasDealtDamage = true;

        
        _hasDealtDamage = true;
        
        float direction = _player.GetComponent<SpriteRenderer>().flipX ? -1f : 1f;
        Vector2 attackDirection = new Vector2(direction, 0);
        
        Vector2 attackPos = (Vector2)_player.transform.position + (attackDirection * _player.attackOffset);

        // 범위 내 적 레이어 찾기
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPos, _player.attackRange, _player.enemyLayer);

        // 데미지 주기
        foreach (Collider2D enemy in hitEnemies)
        {
            var damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(_player.stats.attack);
            }
        }
    }
    
    public void Exit()
    {
        // _animator.ResetTrigger("Attack");
    }
}