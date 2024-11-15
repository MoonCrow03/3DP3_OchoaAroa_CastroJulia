using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class State : MonoBehaviour
{
    protected State m_PreviousState;
    protected State m_CurrentState;

    protected NavMeshAgent m_NavMeshAgent;
    
    public abstract void Enter(PlayerStateManager manager);

    public abstract void Execute(PlayerStateManager manager);

    public abstract void Exit(PlayerStateManager manager);

    public virtual void SetUp(NavMeshAgent navMeshAgent)
    {
        m_NavMeshAgent = navMeshAgent;
    }

    public void Initialize(State previousState)
    {
        m_PreviousState = previousState;
    }
}
