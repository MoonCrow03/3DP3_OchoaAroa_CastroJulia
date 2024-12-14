using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHealthSystem : MonoBehaviour, IRestartGameElement
{
    private static readonly int _hit = Animator.StringToHash("Hit");
    private static readonly int _die = Animator.StringToHash("Die");

    [Header("Health Settings")]
    [SerializeField] private int m_DefaultLives = 4;
    [SerializeField] private int m_MaxHealth = 8;
    
    public bool IsDead => m_CurrentHealth < 0;

    private int m_CurrentLives;
    private int m_CurrentHealth;
    
    private Animator m_Animator;

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        GameManager.GetInstance().RegisterGameElement(this);

        m_CurrentLives = m_DefaultLives;
        m_CurrentHealth = m_MaxHealth;
        
        GameEvents.TriggerUpdateLives(m_CurrentLives);
        GameEvents.TriggerUpdateHealthBar(m_CurrentHealth, m_MaxHealth);
    }
    
    public void TakeDamage(int p_damage)
    {
        m_CurrentHealth -= p_damage;
        m_Animator.SetTrigger(_hit);
        
        GameEvents.TriggerUpdateHealthBar(m_CurrentHealth, m_MaxHealth);
        
        if (m_CurrentHealth <= 0)
        {
            m_CurrentHealth = 0;
            TakeLive();
        }
    }

    public void TakeLive()
    {
        m_CurrentLives -= 1;
        GameEvents.TriggerUpdateLives(m_CurrentLives);
        m_Animator.SetTrigger(_die);
        
        if (m_CurrentLives < 0)
        {
            m_CurrentLives = 0;
        }
        
        Die();
    }

    public void GiveLive()
    {
        m_CurrentLives += 1;
    }

    private void Die()
    {
        if(m_CurrentLives > 0)
            GameManager.GetInstance().RestartGame();
        else
            GameManager.GetInstance().GameOver();
    }

    public void HardRestartGame()
    {
        m_CurrentLives = m_DefaultLives;
        m_CurrentHealth = m_MaxHealth;
        GameEvents.TriggerUpdateHealthBar(m_CurrentHealth, m_MaxHealth);
    }

    public void RestartGame()
    {
        m_CurrentHealth = m_MaxHealth;
        GameEvents.TriggerUpdateHealthBar(m_CurrentHealth, m_MaxHealth);
    }

    public void PauseGame() { }
}
