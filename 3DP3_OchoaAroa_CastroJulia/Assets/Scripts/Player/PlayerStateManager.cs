using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerStateManager : MonoBehaviour, IStartGameElement
{
    [Header("States")]
    public IdleState m_IdleState;

    private State m_CurrentState;
    private State m_PreviousState;

    private NavMeshAgent m_NavMeshAgent;

    private Vector3 m_StartPosition;
    private Quaternion m_StartRotation;

    private void Start()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();

        GameManager.GetInstance().RegisterGameElement(this);

        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;

        m_IdleState.SetUp(m_NavMeshAgent);

        m_CurrentState = m_IdleState;
        m_CurrentState.Enter(this);
    }

    private void Update()
    {
        m_CurrentState.Execute(this);
    }

    public void SwitchState(State p_state)
    {
        m_PreviousState = m_CurrentState;
        m_CurrentState = p_state;

        p_state.Initialize(m_PreviousState);
        p_state.Enter(this);
    }

    public void SwitchToPreviousState()
    {
        SwitchState(m_PreviousState);
    }

    public void RestartGame()
    {
        gameObject.SetActive(true);
        m_NavMeshAgent.isStopped = true;

        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;

        m_NavMeshAgent.isStopped = false;

        m_CurrentState = m_IdleState;
        m_CurrentState.Enter(this);
    }
}
