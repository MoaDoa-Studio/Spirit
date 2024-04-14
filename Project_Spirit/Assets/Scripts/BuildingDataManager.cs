using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDataManager : MonoBehaviour
{
    // 건축물의 가변적인 값만 저장한다.
    // 정적인 값은 ScriptableObject에서 관리하려고 계획 중.
    public static BuildingDataManager instance = null;

    [SerializeField]
    private GameObject BuildingParent;
    public List<Building> BuildingList = new List<Building>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    

    public void AddBuilding(Building building)
    {
        BuildingList.Add(building);
    }

    public List<Building> GetBuildingList()
    {
        return BuildingList;
    }

    public GameObject FindObjectContainsScript(Building building)
    {
        for (int i = 0; i < BuildingParent.transform.childCount; i++)
        {
            GameObject obj = BuildingParent.transform.GetChild(i).gameObject;
            if (obj.GetComponent<Building>() == building)
            {                
                return obj;
            }
        }
        return null;
    }    

    public void DestroyObjectByScript(Building building)
    {
        GameObject obj = FindObjectContainsScript(building);
        if (obj == null)
        {
            Debug.Log("이 스크립트가 부착된 오브젝트를 찾을 수 없습니다.");
            return;
        }

        // 리스트에서 제거해주고 오브젝트 파괴.
        BuildingList.Remove(building);
        Destroy(obj);
    }
}
