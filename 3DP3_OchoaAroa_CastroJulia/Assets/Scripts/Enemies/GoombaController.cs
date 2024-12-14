using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoombaController : StateMachine<EGommbaState>, IRestartGameElement
{
    [Header("General Settings")]
    public float m_RotationSpeed = 100.0f;
    public float m_MaxDistanceToAttack = 15.0f;
    
    [Header("Player Bounce Settings")]
    [SerializeField] private float m_MaxAngleToKillGoomba = 15.0f;
    [SerializeField] private float m_BounceDuration = 0.25f;
    [SerializeField] private float m_GoombaBounceDistance = 5.0f;
    [SerializeField] private float m_PlayerBounceDistance = 5.0f;
    [SerializeField] private float m_PlayerBounceVerticalSpeed = 3.0f;
    [SerializeField] private int m_PlayerDamage = 2;
    
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
            if (IsUpperHit(other.transform))
            {
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                playerController.BounceUp();
                Kill();
            }
            else
            {
                Vector3 impactDirection = (transform.position - other.transform.position).normalized;
                Vector3 goombaBounceDirection = impactDirection  * m_GoombaBounceDistance;
                Vector3 playerBounceDirection = -impactDirection  * m_PlayerBounceDistance + Vector3.up * m_PlayerBounceVerticalSpeed;
                
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                playerController.TakeDamage(m_PlayerDamage);

                StartCoroutine(BounceObject(transform, goombaBounceDirection, m_BounceDuration));
                StartCoroutine(BounceObject(other.transform, playerBounceDirection, m_BounceDuration));
            }
        }
    }
    
    private bool IsUpperHit(Transform playerTransform)
    {
        Vector3 goombaToPlayer = playerTransform.position - transform.position;
        goombaToPlayer.Normalize();

        float dotAngle = Vector3.Dot(goombaToPlayer, Vector3.up);
        return dotAngle >= Mathf.Cos(m_MaxAngleToKillGoomba * Mathf.Deg2Rad);
    }
    
    private IEnumerator BounceObject(Transform obj, Vector3 bounceDirection, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = obj.position;
        Vector3 targetPosition = startPosition + bounceDirection;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            obj.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        obj.position = targetPosition;
    }
    
    private void Kill()
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
        
        Vector3 l_forward = transform.forward;

        int l_coneSegments = 30;

        float l_halfConeAngleRad = m_ConeAngle * Mathf.Deg2Rad / 2.0f;
        float l_segmentAngle = m_ConeAngle * Mathf.Deg2Rad / l_coneSegments;

        for (int i = 0; i <= l_coneSegments; i++)
        {
            float currentAngle = -l_halfConeAngleRad + i * l_segmentAngle;

            Vector3 direction = Quaternion.Euler(0, currentAngle * Mathf.Rad2Deg, 0) * l_forward;

            Gizmos.color = Color.green;  // Set color to green
            Gizmos.DrawRay(l_enemyPosition, direction * m_MaxDistanceToAttack);  // Ray represents vision cone
        }

        Gizmos.color = Color.blue;  // Change color for the circle
        Gizmos.DrawWireSphere(l_enemyPosition + l_forward * m_MaxDistanceToAttack, m_MaxDistanceToAttack * Mathf.Sin(l_halfConeAngleRad));
    }
}
