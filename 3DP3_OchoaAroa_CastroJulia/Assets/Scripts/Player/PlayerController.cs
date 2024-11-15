using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IStartGameElement
{
	[Header("Settings")]
	[SerializeField] private float m_WalkSpeed = 5f;
	[SerializeField] private float m_RunSpeed = 10f;
	
	[Header("Components")]
	[SerializeField] private Camera m_Camera;
	
	private PlayerHealthSystem m_PlayerHealthSystem;
	private CharacterController m_CharacterController;
	private Animator m_Animator;
	
	private Vector3 m_InitialPosition;
	private Quaternion m_InitialRotation;

    private void Awake()
    {
	    m_CharacterController = GetComponent<CharacterController>();
	    m_PlayerHealthSystem = GetComponent<PlayerHealthSystem>();
	    m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
	    GameManager.GetInstance().SetPlayer(this);
	    GameManager.GetInstance().RegisterGameElement(this);
    }

    private void Update()
    {
        Vector3 l_forward = m_Camera.transform.forward;
        Vector3 l_right = m_Camera.transform.right;
        
        l_forward.y = 0f;
        l_right.y = 0f;
        
        l_forward.Normalize();
        l_right.Normalize();

        bool l_hasMovement = false;
        Vector3 l_movement = Vector3.zero;

        if (InputManager.Instance.Right.Hold)
        {
			l_movement = l_right;
			l_hasMovement = true;
        }
        
        if (InputManager.Instance.Left.Hold)
		{
			l_movement = -l_right;
			l_hasMovement = true;
		}

		if (InputManager.Instance.Up.Hold)
		{
			l_movement = l_forward;
			l_hasMovement = true;
		}

		if (InputManager.Instance.Down.Hold)
		{
			l_movement = -l_forward;
			l_hasMovement = true;
		}
		
		l_movement.Normalize();

		float l_speed = 0.0f;
		if (l_hasMovement)
		{
			if (InputManager.Instance.Shift.Hold)
			{
				l_speed = m_RunSpeed;
				m_Animator.SetFloat("Speed", 0.0f);
			}
			else
			{
				l_speed = m_WalkSpeed;
				m_Animator.SetFloat("Speed", 0.0f);
			}
		}
		else
		{
			m_Animator.SetFloat("Speed", 0.0f);
		}
		
		l_movement *= l_speed * Time.deltaTime;
		
		m_CharacterController.Move(l_movement);
    }

    public void TakeLive()
    {
	    m_PlayerHealthSystem.TakeLive();
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
