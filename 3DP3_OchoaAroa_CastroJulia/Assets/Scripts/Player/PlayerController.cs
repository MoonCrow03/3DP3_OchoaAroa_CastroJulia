using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerController : MonoBehaviour, IStartGameElement
{
    [Header("Movement Settings")]
    [SerializeField] private float m_YawSpeed = 360f;
    [SerializeField] private float m_PitchSpeed = 180f;
    [SerializeField] private float m_MinPitch = -60f;
    [SerializeField] private float m_MaxPitch = 80;
    [SerializeField] private float m_Speed = 5;
    [SerializeField] private float m_FastSpeedMultiplier = 2f;
    [SerializeField] private float m_JumpSpeed = 2f;

    [Header("Other Elements")]
    [SerializeField] private Transform m_PitchController;
    [SerializeField] private Camera m_Camera;

    private float m_Yaw;
    private float m_Pitch;
    private float m_VerticalSpeed;

    private bool m_LockAngle;
    private bool m_HasKey;

    private CharacterController m_CharacterController;
    private PlayerHealthSystem m_HealthSystem;
    
    private Vector3 m_StartPosition;
    private Quaternion m_StartRotation;
    private Vector3 m_MovementDirection;
    
    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_HealthSystem = GetComponent<PlayerHealthSystem>();
    }

    private void Start()
    {
        GameManager l_manager = GameManager.GetInstance();

        if (l_manager.GetPlayer() != null)
        {
            l_manager.GetPlayer().InitLevel(this);
            Destroy(gameObject);
        }

        l_manager.SetPlayer(this);
        l_manager.RegisterGameElement(this);

        m_StartPosition = transform.position;
        m_StartRotation = transform.rotation;

        DontDestroyOnLoad(gameObject);

        m_Yaw = transform.eulerAngles.y;
        m_Pitch = transform.eulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        m_LockAngle = false;
    }

    private void Update()
    {
        // Mouse Movement
        float l_HorizontalValue = Input.GetAxis("Mouse X");
        float l_VerticalValue = Input.GetAxis("Mouse Y");
        
        if (!m_LockAngle)
        {
            m_Yaw += l_HorizontalValue * m_YawSpeed * Time.deltaTime;
            m_Pitch -= l_VerticalValue * m_PitchSpeed * Time.deltaTime;
            m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
        }

        transform.rotation = Quaternion.Euler(0f, m_Yaw, 0f);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0f, 0f);
        
        // Movement
        float l_ForwardAngleRadians = m_Yaw * Mathf.Deg2Rad;
        float l_RightAngleRadians = (m_Yaw + 90f) * Mathf.Deg2Rad;
        
        Vector3 l_Forward = new Vector3(Mathf.Sin(l_ForwardAngleRadians), 0f, Mathf.Cos(l_ForwardAngleRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_RightAngleRadians), 0f, Mathf.Cos(l_RightAngleRadians));
        
        m_MovementDirection= Vector3.zero;

        if (InputManager.Instance.Right.Hold)
            m_MovementDirection += l_Right;
        else if (InputManager.Instance.Left.Hold)
            m_MovementDirection -= l_Right;
        
        if (InputManager.Instance.Up.Hold)
            m_MovementDirection += l_Forward;
        else if (InputManager.Instance.Down.Hold)
            m_MovementDirection -= l_Forward;
        
        m_MovementDirection.Normalize();
        
        // Jump
        if (m_CharacterController.isGrounded && InputManager.Instance.Space.Hold)
            m_VerticalSpeed = m_JumpSpeed;
        
        // Fall
        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        
        // Run
        float l_SpeedMultiplier = 1f;

        if (InputManager.Instance.Shift.Hold)
        {
            l_SpeedMultiplier = m_FastSpeedMultiplier;
            
        }
        
        // Movement
        Vector3 l_Movement = m_MovementDirection * m_Speed * l_SpeedMultiplier * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;
        m_CharacterController.Move(l_Movement);
        
        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);
        
        // When to stop moving vertically
        // This is a binary operation. & = &&
        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
            m_VerticalSpeed = 0f;
        else if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0f)
            m_VerticalSpeed = 0f;
    }

    private void OnEnable()
    {
        GameEvents.OnSetCheckpoint += SetCheckPoint;
    }

    private void OnDisable()
    {
        GameEvents.OnSetCheckpoint -= SetCheckPoint;
    }

    private void SetCheckPoint(Vector3 p_position, Quaternion p_rotation)
    {
        m_StartPosition = p_position;
        m_StartRotation = p_rotation;
    }

    private void InitLevel(PlayerController p_player)
    {
        m_CharacterController.enabled = false;

        transform.position = p_player.transform.position;
        transform.rotation = p_player.transform.rotation;

        m_CharacterController.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Item"))
        {
            Item l_Item = other.GetComponent<Item>();

            if (l_Item.CanPick())
            {
                l_Item.Pick();
            }
        }

        if (other.CompareTag("DeadZone"))
        {
            m_HealthSystem.TakeLive();
        }
    }

    public void RestartGame()
    {
        m_CharacterController.enabled = false;
        transform.position = m_StartPosition;
        transform.rotation = m_StartRotation;
        m_CharacterController.enabled = true;
    }
}