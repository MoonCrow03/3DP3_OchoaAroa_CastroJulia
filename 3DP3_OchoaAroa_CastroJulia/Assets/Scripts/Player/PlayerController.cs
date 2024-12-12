using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IRestartGameElement
{
	private static readonly int Speed = Animator.StringToHash("Speed");
	private static readonly int Jump = Animator.StringToHash("Jump");
	private static readonly int Punch = Animator.StringToHash("Punch");
	private static readonly int PunchCombo = Animator.StringToHash("PunchCombo");
	private static readonly int Falling = Animator.StringToHash("Falling");

	[Header("Movement Settings")]
	[SerializeField] private float m_WalkSpeed = 5f;
	[SerializeField] private float m_RunSpeed = 10f;
	[SerializeField] private float m_LerpRotation = 16.0f;
	
	[Header("Punch Settings")]
	[SerializeField] private float m_PunchComboAvailableTime = 1.3f;
	
	[Header("Components")]
	[SerializeField] private Camera m_Camera;
	
	[Header("Collider References")]
	[SerializeField] private GameObject m_LeftHandPunchHitCollider;
	[SerializeField] private GameObject m_RightHandPunchCollider;
	[SerializeField] private GameObject m_RightFootKickCollider;
	
	[Header("Jump Parameters")]
	[SerializeField] private float m_WaitStartJumpTime = 0.12f;
	[SerializeField] private float m_JumpVerticalSpeed = 5.0f;
	[SerializeField] private float m_KillJumpVerticalSpeed = 8.0f;
	[SerializeField] private float m_MaxAngleToKillGoomba = 15.0f;
	[SerializeField] private float m_MinVerticalSpeedToKillGoomba = -1.0f;
	
	[Header("Bridge Parameters")]
	[SerializeField] private float m_BridgeForce = 100.0f;
	
	private float m_VerticalSpeed;
	private float m_LastPunchTime;
	private int m_CurrentPunchId;
	
	private PlayerHealthSystem m_PlayerHealthSystem;
	private CharacterController m_CharacterController;
	private Animator m_Animator;
	
	private Vector3 m_InitialPosition;
	private Quaternion m_InitialRotation;
	
	public Vector3 GetMovementVelocity() => m_CharacterController.velocity;

    private void Awake()
    {
	    m_PlayerHealthSystem = GetComponent<PlayerHealthSystem>();
	    m_CharacterController = GetComponent<CharacterController>();
	    m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
	    GameManager.GetInstance().SetPlayer(this);
	    GameManager.GetInstance().RegisterGameElement(this);

	    SetCheckPoint(transform.position, transform.rotation);
	    
	    m_LeftHandPunchHitCollider.SetActive(false);
	    m_RightHandPunchCollider.SetActive(false);
	    m_RightFootKickCollider.SetActive(false);
	    
	    m_Animator.applyRootMotion = false;
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
			l_movement += l_forward;
			l_hasMovement = true;
		}

		if (InputManager.Instance.Down.Hold)
		{
			l_movement -= l_forward;
			l_hasMovement = true;
		}
		
		l_movement.Normalize();

		if (l_hasMovement)
		{
			if (InputManager.Instance.Shift.Hold)
			{
				l_movement *= m_RunSpeed;
				m_Animator.SetFloat(Speed, 1.0f);
			}
			else
			{
				l_movement *= m_WalkSpeed;
				m_Animator.SetFloat(Speed, 0.5f);
			}
			
			Quaternion l_targetRotation = Quaternion.LookRotation(l_movement);
			transform.rotation = Quaternion.Lerp(transform.rotation, l_targetRotation, 
				m_LerpRotation * Time.deltaTime);
		}
		else
			m_Animator.SetFloat(Speed, 0.0f);

		if (CanJump() && InputManager.Instance.Space.Tap)
		{
			JumpMethod();
		}
		
		l_movement *= m_WalkSpeed * Time.deltaTime;
		
		m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
		
		l_movement.y = m_VerticalSpeed * Time.deltaTime;
		
		CollisionFlags l_collisionFlags = m_CharacterController.Move(l_movement);
		
		bool l_isGrounded = (l_collisionFlags & CollisionFlags.Below) != 0;
		bool l_isRoof = (l_collisionFlags & CollisionFlags.Above) != 0;

		if ((l_isGrounded && m_VerticalSpeed < 0.0f) || (l_isRoof && m_VerticalSpeed > 0.0f))
			m_VerticalSpeed = 0.0f;
		
		m_Animator.SetBool(Falling, !l_isGrounded);
		
		UpdateHit();
    }
    
    // TODO: Implement CanJump method
    private bool CanJump()
    {
	    return true;
    }

    private void JumpMethod()
    {
	    m_Animator.SetTrigger(Jump);
	    StartCoroutine(ExecuteJump());
    }
    
    IEnumerator ExecuteJump()
    {
	    yield return new WaitForSeconds(m_WaitStartJumpTime);
	    m_VerticalSpeed = m_JumpVerticalSpeed;
	    m_Animator.SetTrigger(Jump);
    }

    public void TakeLive()
    {
	    m_PlayerHealthSystem.TakeLive();
    }
    
    private void UpdateHit()
    {
	    if (InputManager.Instance.LeftClick.Tap && CanPunch())
	    {
		    Debug.Log("Punch");
		    ExecutePunchCombo();
	    }
    }
    
    private bool CanPunch()
    {
	    return true;
    }

    private void ExecutePunchCombo()
    {
	    m_Animator.SetTrigger(Punch);
	    float l_DiffTime = Time.time - m_LastPunchTime;
	    if (l_DiffTime <= m_PunchComboAvailableTime)
	    {
		    m_CurrentPunchId = (m_CurrentPunchId + 1) % 3;
	    }
	    else
	    {
		    m_CurrentPunchId = 0;
	    }
	    m_LastPunchTime = Time.time;
	    m_Animator.SetInteger(PunchCombo, m_CurrentPunchId);
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
	    if (hit.gameObject.CompareTag("Bridge"))
	    {
		    hit.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * m_BridgeForce, hit.point);
	    }
	    if (hit.gameObject.CompareTag("Goomba"))
	    {
		    if (IsUpperHit(hit.transform))
		    {
			    hit.gameObject.GetComponent<GoombaController>().Kill();
			    m_VerticalSpeed = m_KillJumpVerticalSpeed;
		    }
		    else
		    {
			    //TODO: Que el goomba y el player salgan rebotados
		    }
	    }
    }

    private bool IsUpperHit(Transform GoombaTransform)
    {
	    Vector3 l_GoombaDirection = transform.position - GoombaTransform.position;
	    l_GoombaDirection.Normalize();
	    float l_DotAngle = Vector3.Dot(l_GoombaDirection, Vector3.up);
	    return l_DotAngle >= Mathf.Cos(m_MaxAngleToKillGoomba * Mathf.Deg2Rad) && m_VerticalSpeed <= m_MinVerticalSpeedToKillGoomba;
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

    public void Heal()
    {
	    m_PlayerHealthSystem.GiveLive();
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
