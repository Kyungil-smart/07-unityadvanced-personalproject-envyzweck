using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public Sprite portrait;
    public int baseHealth;
    public int baseMana;
    public int baseAttack;
    public int baseDefense;
    public float baseSpeed;
}