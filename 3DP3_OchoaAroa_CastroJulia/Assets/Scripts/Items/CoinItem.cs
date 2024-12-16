using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinItem : Item
{
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
        SoundManager.PlaySound(SoundType.COIN);
        GameManager.GetInstance().PickCoin();
        base.Pick();
    }
}
