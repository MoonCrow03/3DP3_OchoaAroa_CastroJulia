using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StarManager : MonoBehaviour
{
    public int starsCount = 0;
    public Text starsText;

    private void Start()
    {
        UpdateUI();
    }

    public void AddStar()
    {
        starsCount ++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        starsText.text = starsCount.ToString();
    }
}
