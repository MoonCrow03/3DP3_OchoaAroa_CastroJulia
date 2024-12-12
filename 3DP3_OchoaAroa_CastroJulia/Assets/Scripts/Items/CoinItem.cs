using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : Item
{
   

    public override bool CanPick()
    {
        return true;
    }

    public override void Pick()
    { 
        // AudioManager._Instance.PlaySound()
        base.Pick();
        
    }

}
