using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public struct MonsterSpawnInfo
{
    public string monsterName;      
    public GameObject monsterPrefab; 
    [Range(0, 100)]
    public int spawnChance;         
 }

public class MonsterSpawner : MonoBehaviour
{
    [Header("Spawn Configuration")]
    public List<MonsterSpawnInfo> monsterList = new List<MonsterSpawnInfo>();
    
    [Header("Spawner Settings")]
    public float spawnRadius = 5f;    
    public int maxMonsterCount = 5;   
    public float spawnDelay = 3f;     
    public float spawnHeight = 2.5f;
    
    // 생성된 몬스터를 찾기위한 변수 (청크로 구현시 소환된 몬스터는 안사라져서)
    private List<GameObject> _activeMonsters = new List<GameObject>();
    
    private int _currentMonsterCount = 0;
    private float _timer = 0f;

    private void Update()
    {
        if (_currentMonsterCount < maxMonsterCount)
        {
            _timer += Time.deltaTime;
            if (_timer >= spawnDelay)
            {
                SpawnRandomMonster();
                _timer = 0f;
            }
        }
    }

    private void SpawnRandomMonster()
    {
        if (monsterList == null || monsterList.Count == 0) return;

        GameObject selectedPrefab = GetWeightedRandomMonster();
        if (selectedPrefab == null) return;
        
        float randomX = Random.Range(-spawnRadius, spawnRadius);
        Vector2 spawnOrigin = new Vector2(transform.position.x + randomX, transform.position.y);

        // 바닥감지
        RaycastHit2D hit = Physics2D.Raycast(spawnOrigin, Vector2.down, 15f, LayerMask.GetMask("Ground"));

        Vector2 finalSpawnPos;

        if (hit.collider != null)
        {
            finalSpawnPos = hit.point + new Vector2(0, spawnHeight);
        }
        else
        {
            return; 
        }

        // 몬스터 생성 (this.transform으로 부모로 선언)
        GameObject obj = Instantiate(selectedPrefab, finalSpawnPos, Quaternion.identity, this.transform);
        
        BaseMonster monster = obj.GetComponent<BaseMonster>();
        if (monster != null)
        {
            monster.mySpawner = this;
            _currentMonsterCount++;
            
            // 관리 리스트에 추가
            _activeMonsters.Add(obj);
        }
    }
    
    private GameObject GetWeightedRandomMonster()
    {
        int totalWeight = 0;
        foreach (var info in monsterList)
        {
            totalWeight += info.spawnChance;
        }

        if (totalWeight <= 0) return null;

        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;

        foreach (var info in monsterList)
        {
            currentWeight += info.spawnChance;
            if (randomValue < currentWeight)
            {
                return info.monsterPrefab;
            }
        }

        return null;
    }
    
    public void OnMonsterRemoved()
    {
        // 청크 비활성화 시 리스트 에서도 제거
        _activeMonsters.RemoveAll(m => m == null || !m.activeInHierarchy);
        _currentMonsterCount--;
        // 마이너스가 되지 않게 유지
        if (_currentMonsterCount < 0) _currentMonsterCount = 0;
    }
    
    private void OnDisable()
    {
        // 소환된 모든 몬스터를 비활성화
        foreach (var monster in _activeMonsters)
        {
            if (monster != null)
            {
                monster.SetActive(false); 
            }
        }
        _activeMonsters.Clear();
        _currentMonsterCount = 0;
        _timer = 0f;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 1f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}