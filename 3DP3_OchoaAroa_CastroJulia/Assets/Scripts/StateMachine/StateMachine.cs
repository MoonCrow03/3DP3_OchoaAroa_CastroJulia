using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class StateMachine<EState> : MonoBehaviour where EState : Enum
{
    protected BState<EState> m_CurrentState;
    private BState<EState> m_QueuedState;
    protected bool m_IsTransitioningState = false;
    
    protected abstract BState<EState> CreateInitialState();

    void Start()
    {
        m_CurrentState = CreateInitialState();
        if (m_CurrentState == null)
        {
            Debug.LogError("<color=red>Initial state not set for StateMachine.</color>");
            return;
        }
        m_CurrentState.OnEnter();
    }

    void Update()
    {
        if (m_CurrentState == null) return;

        if (m_QueuedState != null && !m_IsTransitioningState)
        {
            TransitionToState(m_QueuedState);
            m_QueuedState = null;
        }
        else
        {
            m_CurrentState.OnUpdate();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        m_CurrentState.OnTriggerEnter(other);
    }

    void OnTriggerStay(Collider other)
    {
        m_CurrentState.OnTriggerStay(other);
    }

    void OnTriggerExit(Collider other)
    {
        m_CurrentState.OnTriggerExit(other);
    }
    
    public void QueueNextState(BState<EState> nextState)
    {
        if (nextState == null)
        {
            Debug.LogWarning("<color=red>Attempted to queue a null state.</color>");
            return;
        }
        
        m_QueuedState = nextState;
    }
    
    public void TransitionToState(BState<EState> nextState)
    {
        m_IsTransitioningState = true;
        
        m_CurrentState.OnExit();
        m_CurrentState = nextState;
        m_CurrentState.OnEnter();
        
        m_IsTransitioningState = false;
    }
}
