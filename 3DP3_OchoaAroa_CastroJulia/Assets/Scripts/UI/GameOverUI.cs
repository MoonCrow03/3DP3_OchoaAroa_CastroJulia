using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour, IRestartGameElement
{
    [SerializeField] private float m_FadeDuration = 1.5f;
    [SerializeField] private float m_WaitTimeAfterFade = 3.0f;
    private CanvasGroup m_CanvasGroup;

    private void Awake()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        GameManager.GetInstance().RegisterGameElement(this);
        RestartGame();
    }

    private void OnEnable()
    {
        GameEvents.OnShowGameOverUI += ShowGameOverUI;
        GameEvents.OnShowHardGameOverUI += ShowHardGameOverUI;
    }

    private void OnDisable()
    {
        GameEvents.OnShowGameOverUI -= ShowGameOverUI;
        GameEvents.OnShowHardGameOverUI -= ShowHardGameOverUI;
    }
    
    private void ShowHardGameOverUI(bool show)
    {
        StartCoroutine(AfterGameOverUI(show));
    }

    private void ShowGameOverUI(bool show)
    {
        StartCoroutine(AfterGameOverUI(show));
    }

    private IEnumerator AfterGameOverUI(bool show)
    {
        float l_elapseTime = 0.0f;

        while (l_elapseTime < m_FadeDuration)
        {
            l_elapseTime += Time.deltaTime;
            m_CanvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, l_elapseTime / m_FadeDuration);
            yield return null;
        }

        m_CanvasGroup.alpha = 1.0f;
        
        yield return new WaitForSeconds(m_WaitTimeAfterFade);
        
        GameManager.GetInstance().RestartGameElements();
    }
    
    private IEnumerator AfterHardGameOverUI(bool show)
    {
        float l_elapseTime = 0.0f;

        while (l_elapseTime < m_FadeDuration)
        {
            l_elapseTime += Time.deltaTime;
            m_CanvasGroup.alpha = Mathf.Lerp(0.0f, 1.0f, l_elapseTime / m_FadeDuration);
            yield return null;
        }

        m_CanvasGroup.alpha = 1.0f;
        
        yield return new WaitForSeconds(m_WaitTimeAfterFade);
        
        GameManager.GetInstance().HardRestartGame();
    }

    public void HardRestartGame()
    {
        RestartGame();
    }

    public void RestartGame()
    {
        m_CanvasGroup.alpha = 0.0f;
    }

    public void PauseGame() { }
}
