using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using UnityEngine.PlayerLoop;
using Quaternion = System.Numerics.Quaternion;

public class Elevator : MonoBehaviour
{
    //Poner un colider en el elev

    //2 coldiers uno para la parte de arriba  y otro para abajo

    //desactivar los colider cuando el colider este mirando hacia arriba
    //comprobramos el producto escalar para saber si el mario esta en paralelo a la plataforma


    //Erores menores
    //el player no se destruye entre escenas, al emparentar pierde la prioridad de dont destroy
    
    //si la plataforma esta girando y entras en el trigger y luego baja, ya no te enganchara (recomendable bool de inside).
    
    
    //tigger mario
    [Header("Elevator")] private float m_MaxAngleToAttackElevator = 0.8f;
    private Collider m_CurrentElevator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Elevator"))
        {
            if (CanAttackElevator(other))
                AttachElevator(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Elevator") && other == m_CurrentElevator)
            DetatchElevator();
    }
    
    private bool CanAttackElevator(Collider elevator)
    {
        if (m_CurrentElevator != null)
            return false;
        return IsAttachableElevator(elevator);
    }

    //poducto escalar con un margen de 0.8 grados
    bool IsAttachableElevator(Collider elevator)
    {
        float l_DotAngle = Vector3.Dot(elevator.transform.forward, Vector3.up);
        if (l_DotAngle >= Mathf.Cos(m_MaxAngleToAttackElevator * Mathf.Deg2Rad))
            return true;
        return false;
    }

    private void AttachElevator(Collider elevator)
    {
        transform.SetParent(elevator.transform.parent);
        m_CurrentElevator = elevator;
    }

    void UpdateElevator()
    {
        if (m_CurrentElevator == null)
            return;
        
        if (!IsAttachableElevator(m_CurrentElevator))
            DetatchElevator();
    }

    private void DetatchElevator()
    {
        m_CurrentElevator = null;
        transform.SetParent(null);
    }

    private void LateUpdate()
    {
        Vector3 l_Angle = transform.rotation.eulerAngles;
        transform.rotation = UnityEngine.Quaternion.Euler(0.0f, l_Angle.y, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateElevator();
    }
}