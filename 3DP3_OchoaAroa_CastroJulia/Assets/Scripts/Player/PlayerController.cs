using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour, IRestartGameElement
{
	private static readonly int _speed = Animator.StringToHash("Speed");
	private static readonly int _jump = Animator.StringToHash("Jump");
	private static readonly int _longJump = Animator.StringToHash("LongJump");
	private static readonly int _wallJump = Animator.StringToHash("WallJump");
	private static readonly int _wallSlide = Animator.StringToHash("WallSlide");
	private static readonly int _falling = Animator.StringToHash("Falling");
	private static readonly int _jumpCombo = Animator.StringToHash("JumpCombo");
	private static readonly int _idleBreak = Animator.StringToHash("IdleBreak");
	private static readonly int _restart = Animator.StringToHash("Restart");

	private static int MAX_JUMPS = 3;

	[Header("Movement Settings")] [SerializeField]
	private float m_WalkSpeed = 5f;

	[SerializeField] private float m_RunSpeed = 10f;
	[SerializeField] private float m_LerpRotation = 16.0f;

	[Header("Components")] [SerializeField]
	private Camera m_Camera;

	[Header("Collider References")] [SerializeField]
	private GameObject m_LeftHandPunchHitCollider;

	[SerializeField] private GameObject m_RightHandPunchCollider;
	[SerializeField] private GameObject m_RightFootKickCollider;

	[Header("Jump Parameters")] [SerializeField]
	private float m_JumpVerticalSpeed = 5.0f;

	[SerializeField] private float m_LongJumpVerticalSpeed = 7.0f;
	[SerializeField] private float m_KillJumpVerticalSpeed = 8.0f;
	[SerializeField] private float m_MinVerticalSpeedToKillGoomba = -1.0f;

	[Header("Other Parameters")] [SerializeField]
	private float m_BridgeForce = 100.0f;

	[SerializeField] private float m_GroundedThreshold = 0.005f;

	[Header("Animation Parameters")] [SerializeField]
	private float m_IdleBreakTime = 10f;
	private float m_GroundedTheresold = 0.005f;

	private float m_VerticalSpeed;
	private int m_CurrentJumpId;
	private float m_InactivityTimer;
	private bool m_IsTouchingWall;
	private Vector3 m_WallNormal;

	private PlayerHealthSystem m_PlayerHealthSystem;
	private CharacterController m_CharacterController;
	private Animator m_Animator;

	private Vector3 m_InitialPosition;
	private Quaternion m_InitialRotation;

	public Vector3 GetMovementVelocity() => m_CharacterController.velocity;
	public bool IsGrounded() => m_CharacterController.isGrounded;
	public bool IsDead => m_PlayerHealthSystem.IsDead;

	private void Awake()
	{
		m_PlayerHealthSystem = GetComponent<PlayerHealthSystem>();
		m_CharacterController = GetComponent<CharacterController>();
		m_Animator = GetComponentInChildren<Animator>();
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

		Vector3 l_movement = Vector3.zero;
		bool l_hasMovement = GetMovementInput(ref l_movement, l_right, l_forward);

		l_movement.Normalize();

		if (l_hasMovement)
		{
			HandleMovement(ref l_movement);
			m_InactivityTimer = 0f;
			m_Animator.SetBool(_idleBreak, false);
		}

		else
		{
			m_Animator.SetFloat(_speed, 0.0f);
			m_InactivityTimer += Time.deltaTime;

			if (m_InactivityTimer >= m_IdleBreakTime)
			{
				m_Animator.SetBool(_idleBreak, true);

			}
		}

		HandleJump();

		ApplyMovement(ref l_movement);
	}

	private bool GetMovementInput(ref Vector3 l_movement, Vector3 l_right, Vector3 l_forward)
	{
		bool l_hasMovement = false;

		if (InputManager.Instance.Right.Hold)
		{
			l_movement = l_right;
			l_hasMovement = true;
		}
		else if (InputManager.Instance.Left.Hold)
		{
			l_movement = -l_right;
			l_hasMovement = true;
		}

		if (InputManager.Instance.Up.Hold)
		{
			l_movement += l_forward;
			l_hasMovement = true;
		}
		else if (InputManager.Instance.Down.Hold)
		{
			l_movement -= l_forward;
			l_hasMovement = true;
		}

		return l_hasMovement;
	}

	private void HandleMovement(ref Vector3 l_movement)
	{
		if (InputManager.Instance.Shift.Hold)
		{
			l_movement *= m_RunSpeed;
			m_Animator.SetFloat(_speed, 1.0f);
		}
		else
		{
			l_movement *= m_WalkSpeed;
			m_Animator.SetFloat(_speed, 0.5f);
		}

		Quaternion l_targetRotation = Quaternion.LookRotation(l_movement);
		transform.rotation = Quaternion.Lerp(transform.rotation, l_targetRotation, m_LerpRotation * Time.deltaTime);
	}

	private void ApplyMovement(ref Vector3 l_movement)
	{
		l_movement *= m_WalkSpeed * Time.deltaTime;
		m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
		l_movement.y = m_VerticalSpeed * Time.deltaTime;

		CollisionFlags l_collisionFlags = m_CharacterController.Move(l_movement);

		bool l_isGrounded = (l_collisionFlags & CollisionFlags.Below) != 0;
		bool l_isRoof = (l_collisionFlags & CollisionFlags.Above) != 0;

		if ((l_isGrounded && m_VerticalSpeed < 0.0f) || (l_isRoof && m_VerticalSpeed > 0.0f))
			m_VerticalSpeed = 0.0f;

		if (l_isGrounded && m_CurrentJumpId != 0)
			m_CurrentJumpId = 0;
		
		m_Animator.SetBool(_falling, !l_isGrounded);
	}

	private void HandleJump()
	{
		if (InputManager.Instance.Space.Tap)
		{
			if (m_IsTouchingWall && CanJump())
			{
				Debug.Log("Wall Jump");
				ExecuteWallJump();
			}
			else if (CanJump())
			{
				ExecuteJump();
			}
		}
	}

	private bool CanJump()
	{
		return m_CurrentJumpId < MAX_JUMPS;
	}

	private void ExecuteJump()
	{
		if (InputManager.Instance.Shift.Hold && m_CurrentJumpId == 0)
		{
			m_VerticalSpeed = m_LongJumpVerticalSpeed;
			m_Animator.SetTrigger(_longJump);
		}
		else
		{
			m_VerticalSpeed = m_JumpVerticalSpeed;
			m_Animator.SetTrigger(_jump);
		}

		m_CurrentJumpId++;
		m_Animator.SetInteger(_jumpCombo, m_CurrentJumpId);
	}

	private void ExecuteWallJump()
	{
		m_VerticalSpeed = m_JumpVerticalSpeed;

		Vector3 wallJumpDirection = -m_WallNormal;
		wallJumpDirection.y = 0;
		m_CharacterController.Move(wallJumpDirection * Time.deltaTime * m_RunSpeed);

		m_IsTouchingWall = false;

		m_Animator.SetTrigger(_wallJump);
		m_CurrentJumpId++;
	}

	private void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.gameObject.CompareTag("Bridge"))
		{
			hit.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(-hit.normal * m_BridgeForce, hit.point);
		}

		if (Mathf.Abs(hit.normal.y) < 0.1f)
		{
			m_IsTouchingWall = true;
			m_WallNormal = hit.normal;
		}
		else
		{
			m_IsTouchingWall = false;
		}

		m_Animator.SetBool(_wallSlide, m_IsTouchingWall);
	}

	public void BounceUp()
	{
		m_VerticalSpeed = m_KillJumpVerticalSpeed;
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

	public void TakeDamage(int damage)
	{
		m_PlayerHealthSystem.TakeDamage(damage);
	}

	public void TakeLive()
	{
		m_PlayerHealthSystem.TakeLive();
	}

	public void GiveLive()
	{
		m_PlayerHealthSystem.GiveLive();
	}

	public void SetCheckPoint(Vector3 position, Quaternion rotation)
	{
		m_InitialPosition = position;
		m_InitialRotation = rotation;
	}

	public void HardRestartGame()
	{
		RestartGame();
	}

	public void RestartGame()
	{
		m_CharacterController.enabled = false;

		transform.position = m_InitialPosition;
		transform.rotation = m_InitialRotation;

		m_CharacterController.enabled = true;

		m_Animator.SetTrigger(_restart);
	}

	public void PauseGame()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("DeadZone"))
		{
			m_PlayerHealthSystem.Die();
		}
	}

	private void OnEnable()
	{
		GameEvents.OnSetCheckpoint += SetCheckPoint;
	}
	
	private void OnDisable()
	{
		GameEvents.OnSetCheckpoint -= SetCheckPoint;
	}
}
