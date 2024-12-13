using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoombaController : StateMachine<EGommbaState>, IRestartGameElement
{
    [Header("General Settings")]
    public float m_RotationSpeed = 100.0f;
    public float m_MaxDistanceToAttack = 15.0f;
    
    [Header("Patrol State Settings")]
    public List<Transform> m_PatrolPositions;
    public float m_MinDistanceToAlert = 20.0f;
    
    [Header("Alert State Settings")]
    public float m_ConeAngle = 60.0f;
    public float m_AlertDuration = 5.0f;
    public LayerMask m_SightLayerMask;
    
    [HideInInspector]
    public Animator m_Animator;
    private NavMeshAgent m_NavMeshAgent;
    private Vector3 m_InitialPosition;
    private Quaternion m_InitialRotation;
    
    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponentInChildren<Animator>();
    }

    protected override BState<EGommbaState> CreateInitialState()
    {
        return new GPatrolState(this, m_NavMeshAgent, m_PatrolPositions, m_MinDistanceToAlert);
    }

    protected override void Start()
    {
        base.Start();
        
        m_InitialPosition = transform.position;
        m_InitialRotation = transform.rotation;
        
        GameManager.GetInstance().RegisterGameElement(this);
    }
    
    public void RotateToFindPlayer()
    {
        PlayerController l_player = GameManager.GetInstance().GetPlayer();

        if (l_player == null) return;

        Vector3 l_playerDirection = l_player.transform.position - transform.position;
        l_playerDirection.y = 0f;

        Quaternion l_targetRotation = Quaternion.LookRotation(l_playerDirection);
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            l_targetRotation,
            m_RotationSpeed * Time.deltaTime
        );
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //other.GetComponent<PlayerController>().TakeLive();
        }
    }   
    
    public void Kill()
    {
        gameObject.SetActive(false);
    }
    
    public void RestartGame()
    {
        gameObject.SetActive(true);
        m_NavMeshAgent.enabled = false;
        transform.position = m_InitialPosition;
        transform.rotation = m_InitialRotation;
        m_NavMeshAgent.enabled = true;
    }
    
    private void OnDrawGizmos()
    {
        DrawVisionCone();
    }

    private void DrawVisionCone()
    {
        Vector3 l_enemyPosition = transform.position + Vector3.up * 1.6f;  // Adjust position to eye height

        // Define the forward direction
        Vector3 l_forward = transform.forward;

        // Set the number of segments to draw a smooth cone
        int l_coneSegments = 30;

        // Calculate the angle of each segment in radians
        float l_halfConeAngleRad = m_ConeAngle * Mathf.Deg2Rad / 2.0f;
        float l_segmentAngle = m_ConeAngle * Mathf.Deg2Rad / l_coneSegments;

        // Draw the cone with rays extending to the max attack distance
        for (int i = 0; i <= l_coneSegments; i++)
        {
            // Calculate the current segment angle
            float currentAngle = -l_halfConeAngleRad + i * l_segmentAngle;

            // Rotate the forward direction to get the current segment direction
            Vector3 direction = Quaternion.Euler(0, currentAngle * Mathf.Rad2Deg, 0) * l_forward;

            // Draw a ray for each segment reaching max attack distance
            Gizmos.color = Color.green;  // Set color to green
            Gizmos.DrawRay(l_enemyPosition, direction * m_MaxDistanceToAttack);  // Ray represents vision cone
        }

        // Draw a circle at max attack distance to show the cone's far edge
        Gizmos.color = Color.blue;  // Change color for the circle
        Gizmos.DrawWireSphere(l_enemyPosition + l_forward * m_MaxDistanceToAttack, m_MaxDistanceToAttack * Mathf.Sin(l_halfConeAngleRad));
    }
}
