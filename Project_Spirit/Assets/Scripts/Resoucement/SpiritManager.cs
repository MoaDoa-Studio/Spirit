using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiritManager : MonoBehaviour
{
    static public SpiritManager instance = null;
    [SerializeField]
    GameObject SpawnSlot;

    public float spiritMoveSpeed = 1f;
    public float resourceBuildingDamagePercent = 0.8f;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void ChangeSpiritSpeed(float speed)
    {
        for(int i = 0; i < SpawnSlot.transform.childCount; i++)
        {
            SpawnSlot.transform.GetChild(i).GetComponent<DetectMove>().moveSpeed = speed;
        }
    }
}
