using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine<EPlayerState>, IRestartGameElement
{
    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int Jump = Animator.StringToHash("Jump");
    public static readonly int LongJump = Animator.StringToHash("LongJump");
    public static readonly int JumpCombo = Animator.StringToHash("JumpCombo");
    public static readonly int Falling = Animator.StringToHash("Falling");
    public static readonly int Punch = Animator.StringToHash("Punch");
    public static readonly int PunchCombo = Animator.StringToHash("PunchCombo");
    
    [Header("Movement Settings")]
    public float m_WalkSpeed = 5f;
    public float m_RunSpeed = 10f;
    public float m_LerpRotation = 16.0f;

    [Header("Punch Settings")]
    public float m_PunchComboAvailableTime = 1.3f;
    
    [Header("Collider References")]
    public GameObject m_LeftHandPunchHitCollider;
    public GameObject m_RightHandPunchCollider;
    public GameObject m_RightFootKickCollider;

    [Header("Jump Parameters")]
    public float m_JumpVerticalSpeed = 5.0f;
    public float m_KillJumpVerticalSpeed = 8.0f;
    public float m_MaxAngleToKillGoomba = 15.0f;
    public float m_MinVerticalSpeedToKillGoomba = -1.0f;

    [Header("Bridge Parameters")]
    public float m_BridgeForce = 100.0f;
    
    [Header("Components")]
    public Camera m_Camera;
    
    [Header("Do not touch Parameters")]
    public float m_VerticalSpeed;
    
    private int m_CurrentJumpId;

    private float m_LastPunchTime;
    private int m_CurrentPunchId;
    
    private Vector3 m_InitialPosition;
    private Quaternion m_InitialRotation;
    
    public Animator m_Animator { get; private set; }
    public CharacterController m_CharacterController { get; private set; }
    public PlayerHealthSystem m_PlayerHealthSystem { get; private set; }

    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_PlayerHealthSystem = GetComponent<PlayerHealthSystem>();
        m_Animator = GetComponentInChildren<Animator>();
        
        m_LeftHandPunchHitCollider.SetActive(false);
        m_RightHandPunchCollider.SetActive(false);
        m_RightFootKickCollider.SetActive(false);
        m_Animator.applyRootMotion = false;
        
        
        
    }
    
    private void Start()
    {
        base.Start();
        
        //GameManager.GetInstance().SetPlayer(this);
        GameManager.GetInstance().RegisterGameElement(this);
        SetCheckPoint(transform.position, transform.rotation);
    }

    protected override BState<EPlayerState> CreateInitialState()
    {
        return new IdleState(this);
    }
    
    public void Move(Vector3 movement, float speedMultiplier)
    {
        movement.Normalize();
        Vector3 moveDirection = movement * speedMultiplier * Time.deltaTime;
        m_CharacterController.Move(moveDirection);
        m_Animator.SetFloat(Speed, speedMultiplier > 1.0f ? 1.0f : 0.5f);
    }
    
    public void Rotate(Vector3 movement)
    {
        if (movement.sqrMagnitude > 0.0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, m_LerpRotation * Time.deltaTime);
        }
    }
    
    public void ExecuteJump()
    {
        m_VerticalSpeed = m_JumpVerticalSpeed;
        m_CurrentJumpId++;
        m_Animator.SetTrigger(Jump);
        m_Animator.SetInteger(JumpCombo, m_CurrentJumpId);
    }
    
    public bool CanJump() => m_CurrentJumpId < 3;
    
    public void ExecutePunch()
    {
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
        m_Animator.SetTrigger(Punch);
        m_Animator.SetInteger(PunchCombo, m_CurrentPunchId);
    }
    
    public void EnableHitCollider(EPunchType punchType, bool active)
    {
        switch (punchType)
        {
            case EPunchType.LeftHand:
                m_LeftHandPunchHitCollider.SetActive(active);
                break;
            case EPunchType.RightHand:
                m_RightHandPunchCollider.SetActive(active);
                break;
            case EPunchType.RightFoot:
                m_RightFootKickCollider.SetActive(active);
                break;
        }
    }
    
    public void SetCheckPoint(Vector3 position, Quaternion rotation)
    {
        m_InitialPosition = position;
        m_InitialRotation = rotation;
    }
    
    public void RestartGame()
    {
        m_CharacterController.enabled = false;
	    
        transform.position = m_InitialPosition;
        transform.rotation = m_InitialRotation;
	    
        m_CharacterController.enabled = true;
    }
}
