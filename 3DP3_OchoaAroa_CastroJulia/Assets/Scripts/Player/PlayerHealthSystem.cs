using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerHealthSystem : MonoBehaviour, IStartGameElement
{
    [Header("Health Settings")]
    [SerializeField] private int m_DefaultLives = 4;

    private float m_CurrentLives;
    
    private void Start()
    {
        GameManager.GetInstance().RegisterGameElement(this);

        m_CurrentLives = m_DefaultLives;
    }

    public void TakeLive()
    {
        m_CurrentLives -= 1;
        
        if (m_CurrentLives <= 0)
        {
            m_CurrentLives = 0;
            Die();
        }
    }
    
    private void GiveLive()
    {
        m_CurrentLives += 1;
    }

    private void Die()
    {
        GameManager.GetInstance().RestartGame();
    }

    public void RestartGame()
    {
        m_CurrentLives = m_DefaultLives;
    }
}
