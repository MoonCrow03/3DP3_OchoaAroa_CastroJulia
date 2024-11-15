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
    
    private float m_Yaw;
    private float m_Pitch;
    
    private void Start()
    {
        Cursor.visible = false;
    }
    
    private void LateUpdate()
    {
        float l_horizontalAxis = InputManager.Instance.MouseDelta.x;
        float l_verticalAxis = InputManager.Instance.MouseDelta.y;
        
        Vector3 l_lookDirection = m_FollowObject.position - transform.position;
        float l_distToPlayer = l_lookDirection.magnitude;
        l_lookDirection.y = 0.0f;
        l_lookDirection.Normalize();
        
        m_Yaw = Mathf.Atan2(l_lookDirection.x, l_lookDirection.z) * Mathf.Rad2Deg;
        
        m_Yaw += l_horizontalAxis * m_YawSpeed * Time.deltaTime;
        m_Pitch += l_verticalAxis * m_PitchSpeed * Time.deltaTime;
        
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
        
        if(Physics.Raycast(l_ray, out RaycastHit l_hit, l_distToPlayer, m_LayerMask.value))
        {
            l_desiredPos = l_hit.point + l_cameraForward * m_OffsetHit;
        }
        
        transform.position = l_desiredPos;
        transform.LookAt(m_FollowObject.position);
    }
}
