using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents
{
    public delegate void SetCheckpoint(Vector3 p_position, Quaternion p_rotation);
    public static event SetCheckpoint OnSetCheckpoint;

    public static void TriggerSetCheckpoint(Vector3 p_position, Quaternion p_rotation)
    {
        if (OnSetCheckpoint != null)
        {
            OnSetCheckpoint(p_position, p_rotation);
        }
    }

    #region UI Events

    public delegate void ShowGameOverUI(bool show);
    public static event ShowGameOverUI OnShowGameOverUI;

    public static void TriggerShowGameOverUI(bool show)
    {
        if (OnShowGameOverUI != null)
        {
            OnShowGameOverUI(show);
        }
    }
    
    public delegate void ShowHardGameOverUI(bool show);
    public static event ShowHardGameOverUI OnShowHardGameOverUI;
    
    public static void TriggerShowHardGameOverUI(bool show)
    {
        if (OnShowHardGameOverUI != null)
        {
            OnShowHardGameOverUI(show);
        }
    }
    
    public delegate void UpdateHealthBar(int healthPercentage, int maxHealth);
    
    public static event UpdateHealthBar OnUpdateHealthBar;
    
    public static void TriggerUpdateHealthBar(int healthPercentage, int maxHealth)
    {
        if (OnUpdateHealthBar != null)
        {
            OnUpdateHealthBar(healthPercentage, maxHealth);
        }
    }
    
    public delegate void UpdateCoins(int coinCount);
    public static event UpdateCoins OnUpdateCoins;
    
    public static void TriggerUpdateCoins(int coinCount)
    {
        if (OnUpdateCoins != null)
        {
            OnUpdateCoins(coinCount);
        }
    }
    
    public delegate void UpdateLives(int starCount);
    public static event UpdateLives OnUpdateLives;
    
    public static void TriggerUpdateLives(int starCount)
    {
        if (OnUpdateLives != null)
        {
            OnUpdateLives(starCount);
        }
    }
    
    #endregion
    
    
}
