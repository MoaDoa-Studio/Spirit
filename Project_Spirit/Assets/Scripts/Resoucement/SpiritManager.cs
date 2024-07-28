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

    private float saveMovespeed = 1f;
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

    // 지정해주기
    public void SetSpeed(float speed)
    { 
        saveMovespeed = spiritMoveSpeed;
        spiritMoveSpeed = speed;
        for (int i = 0; i < SpawnSlot.transform.childCount; i++)
        {
            SpawnSlot.transform.GetChild(i).GetComponent<DetectMove>().moveSpeed = speed;
        }
    }

    // 가져오는 것
    public void GetSpeed()
    {
        spiritMoveSpeed = saveMovespeed;
        
    }
}
