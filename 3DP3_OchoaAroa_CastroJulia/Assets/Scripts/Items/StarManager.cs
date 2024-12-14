using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StarManager : MonoBehaviour
{
    public int m_StarsCount = 5;
    
    private void Start()
    {
        GameEvents.TriggerUpdateLives(m_StarsCount);
    }

    public void AddStar()
    {
        m_StarsCount++;
        GameEvents.TriggerUpdateLives(m_StarsCount);
    }
}
