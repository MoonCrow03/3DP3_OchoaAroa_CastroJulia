using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager m_Instance;
    
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

    public void SetPlayer(PlayerController p_Player)
    {
        m_Player = p_Player;
    }

    public void RegisterGameElement(IRestartGameElement p_Element)
    {
        m_RestartGameElements.Add(p_Element);
    }

    public void RestartGame()
    {
        GameEvents.TriggerShowGameOverUI(true);
    }

    public void RestartGameElements()
    {
        foreach (IRestartGameElement l_element in m_RestartGameElements)
        {
            l_element.RestartGame();
        }
    }
}
