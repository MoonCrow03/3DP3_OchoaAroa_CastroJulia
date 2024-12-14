using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour
{
    public int m_CoinCount = 0;

    private void Start()
    {
        GameEvents.TriggerUpdateCoins(m_CoinCount);
    }

    public void AddCoin()
    {
        m_CoinCount++;
        GameEvents.TriggerUpdateCoins(m_CoinCount);
    }
}
