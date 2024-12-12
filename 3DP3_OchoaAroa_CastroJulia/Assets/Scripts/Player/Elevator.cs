using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    [Header("Elevator Parameters")]
    [SerializeField] private float m_ElevatorDotAngle = 0.95f;
    
    private Collider m_CurrentElevator;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            Debug.Log("Enter elevator");
            if(CanAttachElevator(other))
                AttachElevator(other);
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Elevator") && CanAttachElevator(other) && Vector3.Dot(other.transform.up, transform.up) < m_ElevatorDotAngle)
            AttachElevator(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            if (other == m_CurrentElevator)
            {
                Debug.Log("Deattach elevator");
                DeattachElevator();
            }
        }
    }

    private void AttachElevator(Collider elevator)
    {
        transform.SetParent(elevator.transform);
        m_CurrentElevator = elevator;
    }
    
    private bool CanAttachElevator(Collider other)
    {
        Debug.Log("Dot: " + Vector3.Dot(other.transform.up, transform.up));
        return m_CurrentElevator == null && Vector3.Dot(other.transform.up, transform.up) >= m_ElevatorDotAngle;
    }

    private void DeattachElevator()
    {
        m_CurrentElevator = null;
        transform.SetParent(null);
    }
}
