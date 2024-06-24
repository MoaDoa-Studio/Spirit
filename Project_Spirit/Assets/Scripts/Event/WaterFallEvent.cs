using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFallEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject RainDropEventUI;
  
    public void RainDropEventTrigger()
    {
        RainDropEventUI.SetActive(true);
    }
}
