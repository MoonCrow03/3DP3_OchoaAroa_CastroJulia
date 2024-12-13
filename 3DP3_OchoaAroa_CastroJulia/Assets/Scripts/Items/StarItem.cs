using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarItem : Item
{
    [SerializeField] private float _heath; 
    public override bool CanPick()
    {
        return true;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Pick();
        }
    }

    public override void Pick()
    { 
        // AudioManager._Instance.PlaySound()
        GameManager.GetInstance().PickStar();
        base.Pick();
        
    }
}
