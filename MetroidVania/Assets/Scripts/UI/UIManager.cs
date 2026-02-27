using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Player UI")]
    public Slider hpBar;
    public TextMeshProUGUI hpText;
    public Slider manaBar;
    public TextMeshProUGUI manaText;
    public Image playerPortrait;

    public PlayerController player;

    private void Start()
    {
        // 이벤트 구독
        player.HealthChanged += UpdateHealthUI;
        player.ManaChanged += UpdateManaUI;

        // 초기값 불러오기
        UpdateHealthUI(player.stats.currentHealth, player.stats.maxHealth);
        UpdateManaUI(player.stats.currentMana, player.stats.maxMana);

        if (playerPortrait != null && player.characterData != null)
        {
            playerPortrait.sprite = player.characterData.portrait;
        }
    }

    private void UpdateHealthUI(int current, int max)
    {
        if (hpBar != null)
        {
            hpBar.maxValue = max;
            hpBar.value = current;
        }

        if (hpText != null)
        {
            hpText.text = $"{current} / {max}";
        }
    }

    private void UpdateManaUI(int current, int max)
    {
        if (manaBar != null)
        {
            manaBar.maxValue = max;
            manaBar.value = current;
        }

        if (manaText != null)
        {
            manaText.text = $"{current} / {max}";
        }
    }
}