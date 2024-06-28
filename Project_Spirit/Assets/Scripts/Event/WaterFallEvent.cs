using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFallEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject RainDropEventUI;

    private bool rainEventOccured;

    public void RainDropEventTrigger()
    {
        RainDropEventUI.SetActive(true);
        rainEventOccured = true;
    }

    private void Update()
    {
        if(rainEventOccured)
        {

        }
    }
}
