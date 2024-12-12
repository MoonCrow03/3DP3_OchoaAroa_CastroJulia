using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BState<EState> where EState : Enum
{
    public EState m_StateKey { get; private set; }
    
    public BState(EState key)
    {
        m_StateKey = key;
    }
    
    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract void OnUpdate();
    public abstract EState OnNextState();
    public virtual void OnTriggerEnter(Collider other) {}
    public virtual void OnTriggerStay(Collider other) {}
    public virtual void OnTriggerExit(Collider other) {}
}
