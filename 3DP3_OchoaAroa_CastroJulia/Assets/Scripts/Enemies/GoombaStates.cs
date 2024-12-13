using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EGommbaState
{
    PATROL,
    ALERT,
    CHASE,
}

public class GPatrolState : BState<EGommbaState>
{
    private List<Transform> m_PatrolPositions;
    private float m_MinDistanceToAlert;
    private int m_CurrentPatrolPositionId = 0;
    
    private GoombaController m_GoombaController;
    private NavMeshAgent m_NavMeshAgent;
    
    public GPatrolState(GoombaController goombaController, NavMeshAgent navMeshAgent, List<Transform> patrolPositions, float minDistanceToAlert) : base(EGommbaState.PATROL)
    {
        m_GoombaController = goombaController;
        m_NavMeshAgent = navMeshAgent;
        m_PatrolPositions = patrolPositions;
        m_MinDistanceToAlert = minDistanceToAlert;
    }

    public override void OnEnter()
    {
        m_GoombaController.m_Animator.SetBool("Walk", true);
        m_CurrentPatrolPositionId = GetTheClosestPatrolPositionId();
        MoveToNextPatrolPosition();
    }

    public override void OnExit() { }

    public override void OnUpdate()
    {
        if(!m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            m_CurrentPatrolPositionId = GetNextCurrentPatrolPositionId(m_CurrentPatrolPositionId);
            MoveToNextPatrolPosition();
        }

        if (HearsPlayer())
            m_GoombaController.QueueNextState(new GAlertState(m_GoombaController, m_NavMeshAgent, m_GoombaController.m_MaxDistanceToAttack, m_GoombaController.m_ConeAngle, m_GoombaController.m_AlertDuration, m_GoombaController.m_SightLayerMask));
    }

    public override EGommbaState OnNextState() => EGommbaState.PATROL;

    private int GetTheClosestPatrolPositionId()
    {
        Vector3 currentPosition = m_GoombaController.transform.position;

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

        Vector3 l_enemyPosition = m_GoombaController.transform.position;

        return Vector3.Distance(l_playerPosition, l_enemyPosition) < m_MinDistanceToAlert;
    }
}

public class GAlertState : BState<EGommbaState>
{
    private float m_MaxDistanceToAttack;
    private float m_ConeAngle;
    private float m_AlertDuration;
    private LayerMask m_SightLayerMask;
    
    private float m_ElapseTime;
    
    private GoombaController m_GoombaController;
    private NavMeshAgent m_NavMeshAgent;
    
    public GAlertState(GoombaController goombaController, NavMeshAgent navMeshAgent, float maxDistanceToAttack, float coneAngle, float alertDuration, LayerMask sightLayerMask) : base(EGommbaState.ALERT)
    {
        m_GoombaController = goombaController;
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
        
        m_GoombaController.RotateToFindPlayer();
        
        if (SeesPlayer())
        {
            m_NavMeshAgent.isStopped = false;
            m_GoombaController.QueueNextState(new GChaseState(m_GoombaController, m_NavMeshAgent, m_MaxDistanceToAttack));
            return;
        }
        
        if (m_ElapseTime > m_AlertDuration)
        {
            m_NavMeshAgent.isStopped = false;
            m_GoombaController.QueueNextState(new GPatrolState(m_GoombaController, m_NavMeshAgent, m_GoombaController.m_PatrolPositions, m_GoombaController.m_MinDistanceToAlert));
        }
    }

    public override EGommbaState OnNextState() => EGommbaState.ALERT;
    
    private bool SeesPlayer()
    {
        PlayerController l_player = GameManager.GetInstance().GetPlayer();

        if (l_player == null) return false;

        Vector3 l_playerPosition = l_player.transform.position + Vector3.up * 1.6f;

        Vector3 l_enemyPosition = m_GoombaController.transform.position + Vector3.up * 1.6f;

        Vector3 l_direction = l_playerPosition - l_enemyPosition;
        float l_distance = l_direction.magnitude;

        l_direction /= l_distance;

        if (l_distance < m_MaxDistanceToAttack)
        {
            float l_dotAngle = Vector3.Dot(l_direction, m_GoombaController.transform.forward);

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

public class GChaseState : BState<EGommbaState>
{
    private float m_MaxDistanceToAttack;
    
    private GoombaController m_GoombaController;
    private NavMeshAgent m_NavMeshAgent;
    
    public GChaseState(GoombaController goombaController, NavMeshAgent navMeshAgent, float maxDistanceToAttack) : base(EGommbaState.CHASE)
    {
        m_GoombaController = goombaController;
        m_NavMeshAgent = navMeshAgent;
        m_MaxDistanceToAttack = maxDistanceToAttack;
    }

    public override void OnEnter()
    {
        m_GoombaController.m_Animator.SetTrigger("Alert");
        m_GoombaController.m_Animator.SetBool("Run", true);
    }

    public override void OnExit()
    {
        m_GoombaController.m_Animator.SetBool("Run", false);
    }

    public override void OnUpdate()
    {
        PlayerController l_player = GameManager.GetInstance().GetPlayer();

        if (l_player == null) return;
        
        Vector3 l_playerPosition = l_player.transform.position;

        float l_distanceToPlayer = Vector3.Distance(l_playerPosition, m_GoombaController.transform.position);

        m_GoombaController.RotateToFindPlayer();

        if (l_distanceToPlayer > m_MaxDistanceToAttack)
        {
            Debug.Log("Player out of range. Switching to Patrol State.");
            m_GoombaController.QueueNextState(new GPatrolState(m_GoombaController, m_NavMeshAgent, m_GoombaController.m_PatrolPositions, m_GoombaController.m_MinDistanceToAlert));
            return;
        }

        m_NavMeshAgent.SetDestination(l_playerPosition);
    }

    public override EGommbaState OnNextState() => EGommbaState.CHASE;
}
