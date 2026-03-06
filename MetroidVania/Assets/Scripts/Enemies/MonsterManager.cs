using UnityEngine;
using System.Collections.Generic;

public class MonsterManager : MonoBehaviour
{
    // 싱글톤
    public static MonsterManager Instance;
    
    private List<DeadMonsterInfo> _deadMonsters = new List<DeadMonsterInfo>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        for (int i = _deadMonsters.Count - 1; i >= 0; i--)
        {
            _deadMonsters[i].timer -= Time.deltaTime;

            if (_deadMonsters[i].timer <= 0)
            {
                _deadMonsters[i].monster.InstantRespawn();
                _deadMonsters.RemoveAt(i);
            }
        }
    }
    
    public void RegisterDeadMonster(BaseMonster monster, float delay)
    {
        _deadMonsters.Add(new DeadMonsterInfo(monster, delay));
    }
    
    private class DeadMonsterInfo
    {
        public BaseMonster monster;
        public float timer;

        public DeadMonsterInfo(BaseMonster m, float t)
        {
            monster = m;
            timer = t;
        }
    }
}