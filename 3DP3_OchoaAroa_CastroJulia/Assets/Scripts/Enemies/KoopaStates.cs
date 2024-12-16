using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EKoopaState
{
    PATROL,
    ALERT,
    CHASE,
    SHEll,
    SHELL_MOVE,
}

public class KPatrolState : BState<EKoopaState>
{
    private List<Transform> m_PatrolPositions;
    private float m_MinDistanceToAlert;
    private int m_CurrentPatrolPositionId = 0;
    
    private KoopaController m_KoopaController;
    private NavMeshAgent m_NavMeshAgent;
    
    public KPatrolState(KoopaController koopaController, NavMeshAgent navMeshAgent, List<Transform> patrolPositions, float minDistanceToAlert) : base(EKoopaState.PATROL)
    {
        m_KoopaController = koopaController;
        m_NavMeshAgent = navMeshAgent;
        m_PatrolPositions = patrolPositions;
        m_MinDistanceToAlert = minDistanceToAlert;
    }

    public override void OnEnter()
    {
        m_KoopaController.m_Animator.SetBool("Walk", true);
        m_CurrentPatrolPositionId = GetTheClosestPatrolPositionId();
        MoveToNextPatrolPosition();
    }

    public override void OnExit() { }

    public override void OnUpdate()
    {
        if(!m_NavMeshAgent.enabled) return;
        
        if(!m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            m_CurrentPatrolPositionId = GetNextCurrentPatrolPositionId(m_CurrentPatrolPositionId);
            MoveToNextPatrolPosition();
        }

        if (HearsPlayer())
            m_KoopaController.QueueNextState(new KAlertState(m_KoopaController, m_NavMeshAgent, m_KoopaController.m_MaxDistanceToAttack, m_KoopaController.m_ConeAngle, m_KoopaController.m_AlertDuration, m_KoopaController.m_SightLayerMask));
    }

    public override EKoopaState OnNextState() => EKoopaState.PATROL;

    private int GetTheClosestPatrolPositionId()
    {
        Vector3 currentPosition = m_KoopaController.transform.position;

        float closestDistance = Vector3.Distance(currentPosition, m_PatrolPositions[0].position);
        int closestPatrolPositionId = 0;

        for (int i = 1; i < m_PatrolPositions.Count; i++)
        {
            float distance = Vector3.Distance(currentPosition, m_PatrolPositions[i].position);

            if (closestDistance < distance)
            {
                closestDistance = Vector3.Distance(currentPosition, m_PatrolPositions[i].position);
                closestPatrolPositionId = i;
            }
        }

        return closestPatrolPositionId;
    }
    
    private int GetNextCurrentPatrolPositionId(int currentPatrolPositionId)
    {
        ++currentPatrolPositionId;

        if (currentPatrolPositionId == m_PatrolPositions.Count)
            currentPatrolPositionId = 0;

        return currentPatrolPositionId;
    }
    
    private void MoveToNextPatrolPosition()
    {
        Vector3 nextPatrolPosition = m_PatrolPositions[m_CurrentPatrolPositionId].position;
        m_NavMeshAgent.SetDestination(nextPatrolPosition);
    }
    
    private bool HearsPlayer()
    {
        PlayerController l_player = GameManager.GetInstance().GetPlayer();

        if (l_player == null) return false;

        Vector3 l_playerPosition = l_player.transform.position;

        Vector3 l_enemyPosition = m_KoopaController.transform.position;

        return Vector3.Distance(l_playerPosition, l_enemyPosition) < m_MinDistanceToAlert;
    }
}

public class KAlertState : BState<EKoopaState>
{
    private float m_MaxDistanceToAttack;
    private float m_ConeAngle;
    private float m_AlertDuration;
    private LayerMask m_SightLayerMask;
    
    private float m_ElapseTime;
    
    private KoopaController m_KoopaController;
    private NavMeshAgent m_NavMeshAgent;
    
    public KAlertState(KoopaController koopaController, NavMeshAgent navMeshAgent, float maxDistanceToAttack, float coneAngle, float alertDuration, LayerMask sightLayerMask) : base(EKoopaState.ALERT)
    {
        m_KoopaController = koopaController;
        m_NavMeshAgent = navMeshAgent;
        m_MaxDistanceToAttack = maxDistanceToAttack;
        m_ConeAngle = coneAngle;
        m_AlertDuration = alertDuration;
        m_SightLayerMask = sightLayerMask;
    }

    public override void OnEnter()
    {
        m_ElapseTime = 0.0f;
        m_NavMeshAgent.isStopped = true;
    }

    public override void OnExit() { }

    public override void OnUpdate()
    {
        m_ElapseTime += Time.deltaTime;
        
        if(!m_NavMeshAgent.enabled) return;
        
        m_KoopaController.RotateToFindPlayer();
        
        if (SeesPlayer())
        {
            m_NavMeshAgent.isStopped = false;
            m_KoopaController.QueueNextState(new KChaseState(m_KoopaController, m_NavMeshAgent, m_MaxDistanceToAttack));
            return;
        }
        
        if (m_ElapseTime > m_AlertDuration)
        {
            m_NavMeshAgent.isStopped = false;
            m_KoopaController.QueueNextState(new KPatrolState(m_KoopaController, m_NavMeshAgent, m_KoopaController.m_PatrolPositions, m_KoopaController.m_MinDistanceToAlert));
        }
    }

    public override EKoopaState OnNextState() => EKoopaState.ALERT;
    
    private bool SeesPlayer()
    {
        PlayerController l_player = GameManager.GetInstance().GetPlayer();

        if (l_player == null) return false;

        Vector3 l_playerPosition = l_player.transform.position + Vector3.up * 1.6f;

        Vector3 l_enemyPosition = m_KoopaController.transform.position + Vector3.up * 1.6f;

        Vector3 l_direction = l_playerPosition - l_enemyPosition;
        float l_distance = l_direction.magnitude;

        l_direction /= l_distance;

        if (l_distance < m_MaxDistanceToAttack)
        {
            float l_dotAngle = Vector3.Dot(l_direction, m_KoopaController.transform.forward);

            if (l_dotAngle >= Mathf.Cos(m_ConeAngle * Mathf.Deg2Rad / 2.0f))
            {
                Ray l_ray = new Ray(l_enemyPosition, l_direction);

                Debug.DrawRay(l_enemyPosition, l_direction * 100f, Color.red);

                if (!Physics.Raycast(l_ray, l_distance, m_SightLayerMask.value))
                    return true;
            }
        }

        return false;
    }
}

public class KChaseState : BState<EKoopaState>
{
    private float m_MaxDistanceToAttack;
    
    private KoopaController m_KoopaController;
    private NavMeshAgent m_NavMeshAgent;
    
    public KChaseState(KoopaController koopaController, NavMeshAgent navMeshAgent, float maxDistanceToAttack) : base(EKoopaState.CHASE)
    {
        m_KoopaController = koopaController;
        m_NavMeshAgent = navMeshAgent;
        m_MaxDistanceToAttack = maxDistanceToAttack;
    }

    public override void OnEnter()
    {
        m_KoopaController.m_Animator.SetTrigger("Alert");
        m_KoopaController.m_Animator.SetBool("Run", true);
    }

    public override void OnExit()
    {
        m_KoopaController.m_Animator.SetBool("Run", false);
    }

    public override void OnUpdate()
    {
        if(!m_NavMeshAgent.enabled) return;
        
        PlayerController l_player = GameManager.GetInstance().GetPlayer();

        if (l_player == null) return;
        
        Vector3 l_playerPosition = l_player.transform.position;

        float l_distanceToPlayer = Vector3.Distance(l_playerPosition, m_KoopaController.transform.position);

        m_KoopaController.RotateToFindPlayer();

        if (l_distanceToPlayer > m_MaxDistanceToAttack)
        {
            Debug.Log("Player out of range. Switching to Patrol State.");
            m_KoopaController.QueueNextState(new KPatrolState(m_KoopaController, m_NavMeshAgent, m_KoopaController.m_PatrolPositions, m_KoopaController.m_MinDistanceToAlert));
            return;
        }

        m_NavMeshAgent.SetDestination(l_playerPosition);
    }

    public override EKoopaState OnNextState() => EKoopaState.CHASE;
}