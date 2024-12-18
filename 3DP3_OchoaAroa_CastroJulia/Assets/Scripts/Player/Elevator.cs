using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [SerializeField] private float m_MaxAttachingAngle; 
    GameObject m_Player; 

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<PlayerController>() != null)
        {
            AttachPlayer(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerController>() != null)
        {
            DeattachPlayer();
        }
    }

    private void Update()
    {
        if (Vector3.Angle(transform.up, Vector3.up) > m_MaxAttachingAngle)
        {
            DeattachPlayer();
        }
    }
    
    private void AttachPlayer(GameObject mario)
    {
        mario.transform.parent = transform;
        m_Player = mario; 
    }

    private void DeattachPlayer()
    {
        if(m_Player != null)
        {
            m_Player.transform.parent = null;
            m_Player = null; 
        }
    }
}
