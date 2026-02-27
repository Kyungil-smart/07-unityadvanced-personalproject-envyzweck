[System.Serializable]
public class CharacterStats
{
    public int level;
    public int maxHealth;
    public int currentHealth;
    public int maxMana;
    public int currentMana;
    public int attack;
    public int defense;
    public float speed;

    public CharacterStats(CharacterData data)
    {
        level = 1;
        maxHealth = data.baseHealth;
        currentHealth = maxHealth;
        maxMana = data.baseMana;
        currentMana = maxMana;
        attack = data.baseAttack;
        defense = data.baseDefense;
        speed = data.baseSpeed;
    }

    public void LevelUp()
    {
        level++;
        attack += 1;
        defense += 1;
        currentHealth += 30;
        speed += 0.1f;
    }
}