using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Lives")]
    [SerializeField] private Text m_LivesText;
    
    [Header("Coins")]
    [SerializeField] private Text m_CoinText;
    
    [Header("Health Bar")]
    [SerializeField] private Image m_HealthBar;
    [SerializeField] private float m_LerpSpeed = 3.0f;
    
    private void UpdateLives(int m_StarsCount)
    {
        m_LivesText.text = m_StarsCount.ToString();
    }

    private void UpdateCoins(int m_CoinCount)
    {
        m_CoinText.text = m_CoinCount.ToString();
    }
    
    private void UpdateHealthBar(int healthPercentage, int maxHealth)
    {
        float amount = (float) healthPercentage / maxHealth;
        amount = Mathf.Clamp(amount, 0, 1);
        m_HealthBar.fillAmount = amount;
        ColorChanger();
    }

    private void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, m_HealthBar.fillAmount);
        m_HealthBar.color = healthColor;
    }
    
    private void OnEnable()
    {
        GameEvents.OnUpdateHealthBar += UpdateHealthBar;
        GameEvents.OnUpdateCoins += UpdateCoins;
        GameEvents.OnUpdateLives += UpdateLives;
    }
    
    private void OnDisable()
    {
        GameEvents.OnUpdateHealthBar -= UpdateHealthBar;
        GameEvents.OnUpdateCoins -= UpdateCoins;
        GameEvents.OnUpdateLives -= UpdateLives;
    }
}
