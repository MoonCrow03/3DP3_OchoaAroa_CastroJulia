using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private Transform m_SpawnPoint;

    private void OnTriggerEnter(Collider p_collider)
    {
        if (p_collider.CompareTag("Player"))
        {
            GameEvents.TriggerSetCheckpoint(m_SpawnPoint.position, m_SpawnPoint.rotation);
        }
    }
}
