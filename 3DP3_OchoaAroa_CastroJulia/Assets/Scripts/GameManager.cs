using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance;
    private CoinManager m_CoinManager;
    private StarManager m_StarManager;
    
    private PlayerController m_Player;
    private List<IRestartGameElement> m_RestartGameElements;

    private void Awake()
    {
        if (m_Instance == null)
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        m_CoinManager = GetComponent<CoinManager>();
        m_StarManager= GetComponent<StarManager>();

        m_RestartGameElements = new List<IRestartGameElement>();
    }

    public static GameManager GetInstance()
    {
        return m_Instance;
    }

    public PlayerController GetPlayer()
    {
        return m_Player;
    }

    public void SetPlayer(PlayerController pPlayer)
    {
        m_Player = pPlayer;
    }

    public void RegisterGameElement(IRestartGameElement p_Element)
    {
        m_RestartGameElements.Add(p_Element);
    }

    public void RestartGame()
    {
        PauseGame();
        GameEvents.TriggerShowGameOverUI(true);
    }
    
    public void GameOver()
    {
        PauseGame();
        GameEvents.TriggerShowHardGameOverUI(true);
    }
    
    public void HardRestartGame()
    {
        foreach (IRestartGameElement element in m_RestartGameElements)
        {
            element.HardRestartGame();
        }
    }

    public void RestartGameElements()
    {
        foreach (IRestartGameElement element in m_RestartGameElements)
        {
            element.RestartGame();
        }
    }
    
    public void PauseGame()
    {
        foreach (IRestartGameElement element in m_RestartGameElements)
        {
            element.PauseGame();
        }
    }

    public void PickCoin()
    {
        m_CoinManager.AddCoin();
    }

    public void PickStar()
    {
        m_StarManager.AddStar();
    }
}
