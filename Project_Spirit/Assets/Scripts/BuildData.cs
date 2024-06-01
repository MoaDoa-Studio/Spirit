using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Build Item", menuName = "Build/Item", order = 1)]

public class BuildData : ScriptableObject
{
    public float structureID;
    public string structureName = "New Item";
    public int KindOfStructure = 0;
    public float stoneRequirement = 0;
    public float woodRequirement = 0;
    public float essenceRequirement = 0;
    public int UniqueProperties = 0;
    public int StructureEffect = 0;
    public float WorkingTime = 0;
    public int Capacity = 0;
    public float HcostOfUse = 0;

    //길의 것을 만들어야하는데 언제 이것을 불러일으키냐의 문제이다.
    public GameObject prefab;

    //길의 테마타입
    public pathType pathtype;

    //길에 대한 설명
    public string pathdescription;

    //길의 타입 정하기
    public enum pathType
    {
        BabyRoom,
        OnTable,
        PlayGround

    }
}