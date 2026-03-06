using UnityEngine;

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Objects/MonsterData")]
public class MonsterData : ScriptableObject
{
    [Header("Monster Basic Stats")]
    public string monsterName = "Monster";
    public int maxHealth = 100;
    public float moveSpeed = 3f;
    public float attackRange = 1.5f;
}
