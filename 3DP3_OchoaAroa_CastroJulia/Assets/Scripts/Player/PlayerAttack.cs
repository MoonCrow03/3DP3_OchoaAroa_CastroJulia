using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private static readonly int _punch = Animator.StringToHash("Punch");
    private static readonly int _punchCombo = Animator.StringToHash("PunchCombo");
    
    [Header("Punch Settings")]
    [SerializeField] private float m_PunchComboAvailableTime = 1.3f;
    
    private float m_LastPunchTime;
    private int m_CurrentPunchId;
    
    private Animator m_Animator;
    private PlayerController m_PlayerController;

    private void Awake()
    {
        m_Animator = GetComponentInChildren<Animator>();
        m_PlayerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (InputManager.Instance.LeftClick.Tap && CanPunch())
        {
            ExecutePunchCombo();
        }
    }

    private bool CanPunch()
    {
        return m_PlayerController.IsGrounded();
    }
    
    private void ExecutePunchCombo()
    {
        m_Animator.SetTrigger(_punch);
        float diffTime = Time.time - m_LastPunchTime;
        if (diffTime <= m_PunchComboAvailableTime)
        {
            m_CurrentPunchId = (m_CurrentPunchId + 1) % 3;
        }
        else
        {
            m_CurrentPunchId = 0;
        }
        m_LastPunchTime = Time.time;
        m_Animator.SetInteger(_punchCombo, m_CurrentPunchId);
    }
}
