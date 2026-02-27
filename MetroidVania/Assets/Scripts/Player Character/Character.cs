using UnityEngine;

public class Character : MonoBehaviour
{
    public CharacterData characterData; 

    private int _currentHealth;

    void Start()
    {
        _currentHealth = characterData.baseHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
    }
}