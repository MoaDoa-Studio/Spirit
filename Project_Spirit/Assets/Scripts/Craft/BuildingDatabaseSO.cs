using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildingDatabaseSO : ScriptableObject
{
    public List<BuildingData> buildingsData;
}

[Serializable]
public class BuildingData
{
    [field: SerializeField]
    public string name { get; private set; }
    [field: SerializeField]
    public int ID { get; private set; }
    [field: SerializeField]
    public string type { get; private set; }
    [field: SerializeField]
    public string stoneRequirement { get; private set; }
    [field: SerializeField]
    public string woodRequirement { get; private set; }
    [field: SerializeField]
    public string essenceRequirement { get; private set; }
    [field: SerializeField]
    public Vector2Int size { get; private set; } = Vector2Int.zero;
    [field: SerializeField]
    public GameObject prefab { get; private set; }    
}
