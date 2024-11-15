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
    
    #endregion
    
    
}
