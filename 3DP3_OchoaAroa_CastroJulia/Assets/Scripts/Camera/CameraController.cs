using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private float m_MinCameraDist = 5.0f;
    [SerializeField] private float m_MaxCameraDist = 15.0f;
    
    [Header("Camera Rotation Settings")]
    [SerializeField] private float m_YawSpeed = 30.0f;
    [SerializeField] private float m_PitchSpeed = 15.0f;
    [SerializeField] private float m_MinPitch = 4f;
    [SerializeField] private float m_MaxPitch = 8f;
    
    [Header("Camera Collision Settings")]
    [SerializeField] private float m_OffsetHit = 0.1f;
    [SerializeField] private LayerMask m_LayerMask;
    
    [Header("Components")]
    [SerializeField] private Transform m_FollowObject;

    [Header("Smooth Movement")] [SerializeField]
    private float m_SmoothTime = 0.2f; 
    
    private float m_Yaw;
    private float m_Pitch;
    private float m_ResetTime = 5f;
    private float m_InactivityTimer;
    private Vector3 m_CurrentVel; 
    
    private void Start()
    {
        Cursor.visible = false;
    }
    
    private void LateUpdate()
    {
        float l_horizontalAxis = InputManager.Instance.Look.x;
        float l_verticalAxis = InputManager.Instance.Look.y;
        
        if (Mathf.Abs(l_horizontalAxis) > 0.01f || Mathf.Abs(l_verticalAxis) > 0.01f)
        {
            m_InactivityTimer = 0f;
        }
        else
        {
            m_InactivityTimer += Time.deltaTime;
        }
        
        Vector3 l_lookDirection = m_FollowObject.position - transform.position;
        float l_distToPlayer = l_lookDirection.magnitude;
        l_lookDirection.y = 0.0f;
        l_lookDirection.Normalize();
        
        m_Yaw = Mathf.Atan2(l_lookDirection.x, l_lookDirection.z) * Mathf.Rad2Deg;
        
        if (m_InactivityTimer >= m_ResetTime)
        {
            float targetYaw = m_FollowObject.eulerAngles.y;
            float targetPitch = (m_MinPitch + m_MaxPitch) / 2f;

            m_Yaw = Mathf.LerpAngle(m_Yaw, targetYaw, Time.deltaTime / m_SmoothTime); 
            m_Pitch = Mathf.Lerp(m_Pitch, targetPitch, Time.deltaTime / m_SmoothTime); 
        }
        else { 
            m_Yaw += l_horizontalAxis * m_YawSpeed * Time.deltaTime;
            m_Pitch += l_verticalAxis * m_PitchSpeed * Time.deltaTime;
            
        }
       
        
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
        
        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_PitchInRadians = m_Pitch * Mathf.Deg2Rad;
        
        Vector3 l_cameraForward = new Vector3(
            Mathf.Sin(l_YawInRadians) * Mathf.Cos(l_PitchInRadians),
            Mathf.Sin(l_PitchInRadians),
            Mathf.Cos(l_YawInRadians) * Mathf.Cos(l_PitchInRadians));
        
        l_distToPlayer = Mathf.Clamp(l_distToPlayer, m_MinCameraDist, m_MaxCameraDist);
        
        Vector3 l_desiredPos = m_FollowObject.position - l_cameraForward * l_distToPlayer;
        
        Ray l_ray = new Ray(m_FollowObject.position, -l_cameraForward);
        
        if (Physics.Raycast(l_ray, out RaycastHit l_hit, m_MaxCameraDist, m_LayerMask.value))
        {
            // Calculate the new distance based on the hit point, but move the camera away
            // Increase the distance from the object (camera goes over the obstacle)
            float l_adjustedDist = Mathf.Max(l_hit.distance + m_OffsetHit, m_MinCameraDist);  // Ensure the camera doesn't get closer than the minimum distance
            l_desiredPos = m_FollowObject.position - l_cameraForward * l_adjustedDist;
        }
        else
        {
            // Default position when no collision occurs
            l_desiredPos = m_FollowObject.position - l_cameraForward * l_distToPlayer;
        }
        
       transform.position = l_desiredPos;
       transform.LookAt(m_FollowObject.position);
      
    }
}
