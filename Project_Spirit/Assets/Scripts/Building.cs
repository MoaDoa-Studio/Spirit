using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField]
    private int BuildID;
    [SerializeField]
    GameObject sliderUI;
    [HideInInspector]
    public Vector2Int upperRight;
    [HideInInspector]
    public Vector2Int bottomLeft;
    public Tuple<Vector2Int, Vector2Int> connectedRoads;
    // 정령 담는 리스트
    [SerializeField]
    List<GameObject> gameObjectList;
    BuildingDataManager buildingDataManager;
    List<BuildData> buildDataList;
    List<StructUniqueData> structUniqueDataList;
    BuildData buildData;
    StructUniqueData structUniqueData;

    Slider buildBar;

    [Header("빌딩 속성")]
    public float structureID;
    public string structureName = "New Item";
    public int KindOfStructure = 0;
    public float stoneRequirement = 0;
    public float woodRequirement = 0;
    public float essenceRequirement = 0;
    public int UniqueProperties = 0;
    public int StructureEffect = 0;
    public float WorkingTime;
    public int Capacity;
    public float HCostOfUse;
    public float CostUseWood;
    public float CostOfStone;
    public int DemandingWork;
    public int StructureCondition;

    [SerializeField]
    private float constructionAmount = 0;
    // 공장 운영 상태를 나타냄
    public enum BuildOperator
    {
        None,
        Construct,
        Done
    }

    public enum BuildState
    {
        Rest,
        isWork
    }
    [SerializeField]
    BuildOperator buildOperator = BuildOperator.None;
    [SerializeField]
    BuildState buildState = BuildState.Rest;

    public int MaxPlayer = 4;
    private void Start()
    {
        connectedRoads = null;
        gameObjectList = new List<GameObject>();
        
        // 빌딩 데이터 초기화.
        buildDataList = GameObject.Find("GameManager").GetComponent<BuildingDataManager>().buildDataList;
        structUniqueDataList = GameObject.Find("GameManager").GetComponent<BuildingDataManager>().structUniqueDataList;
        buildData = FindDataFromBuildData(buildDataList, BuildID);
        structUniqueData = FindDataFromStructUnique(structUniqueDataList, buildData.UniqueProperties);
        SycnXMLDataToBuilding(buildData, structUniqueData);
    }
    private void Update()
    {
        gameObjectList.RemoveAll(item => item == null);
        BuildOperation();
        BuildStater();
    }

    void BuildOperation()
    {
        switch (buildOperator)
        {   // 건축 짓기 전 상태
            case BuildOperator.None:
                if (constructionAmount > 0)
                { buildOperator = BuildOperator.Construct; }
                break;
            // 건축 진행 단계
            case BuildOperator.Construct:

                // 건축 진행중 슬라이더 표시
                ShowBuildSlideBarToUI();

                if (constructionAmount > 10)
                { buildOperator = BuildOperator.Done; }
                break;

            case BuildOperator.Done:
                sliderUI.SetActive(false);
                break;
        }
    }

    void BuildStater()
    {
        switch (buildState)
        {
            case BuildState.Rest:
                if(gameObjectList.Count != 0)
                    buildState = BuildState.isWork;
                break;
            case BuildState.isWork:

                // 건축 지어지기 전 공사를 진행하는 상태.
                if (buildOperator == BuildOperator.Construct)
                {
                    // 건축물 애니메이션
                }

                // 건축이 지어졌고, 해당 건물을 사용하는 상태.
                if (buildOperator == BuildOperator.Done)
                {
                    if (gameObjectList.Count > 0)
                    {
                        // 건축물 애니메이션 보여주기
                        
                    }
                }

                if (gameObjectList.Count == 0)
                    buildState = BuildState.Rest;
                break;
        }
    }
    public bool AskPermissionOfUse(GameObject _gameObject)
    {
        if (buildOperator == BuildOperator.None || buildOperator == BuildOperator.Construct)
        {
            // 어떤 정령도 이용할 수 있으며, 수용량에 따라 
            CheckForAccesibleBeforeBuilt(_gameObject);
            constructionAmount++;
            return true;
        }

        else if(buildOperator == BuildOperator.Done)
        {
            CheckForAccesibleWhenBuilt(_gameObject);
        }
        return false;
    }

    public bool CheckForAccesibleBeforeBuilt(GameObject _gameObject)
    {
        if(connectedRoads == null) return false;

        if (gameObjectList.Count >= 0 && gameObjectList.Count < 4)
        {
            gameObjectList.Add(_gameObject);
            constructionAmount++;
            return true;
        }
        else
            return false;
    }
    public bool CheckForAccesibleWhenBuilt(GameObject _gameObject)
    {   
        // 길이 두개 일때만 사용가능하게..
        if (connectedRoads == null) return false;
        if(_gameObject.GetComponent<Spirit>().SpiritID != StructureCondition) return false;

        if (gameObjectList.Count >= 0 && gameObjectList.Count < Capacity)
        {
            gameObjectList.Add(_gameObject);
            return true;
        }
        else
            return false;
    }

    public Tuple<Vector2Int, Vector2Int> GetBuildingPos()
    {
        return new Tuple<Vector2Int, Vector2Int>(upperRight, bottomLeft);
    }

    public void SetBuildingPos(Vector2Int upperRight, Vector2Int bottomLeft)
    {
        this.upperRight = upperRight;
        this.bottomLeft = bottomLeft;
    }

    public Tuple<Vector2Int, Vector2Int> GetConnectedRoad()
    {
        if (connectedRoads == null)
        {
            Debug.Log("도로가 2개가 아닙니다.");
            return null;
        }
        return connectedRoads;
    }

    public void SetConnectedRoad(Tuple<Vector2Int, Vector2Int> connectedRoads)
    {
        this.connectedRoads = connectedRoads;
    }


    public void AddWorkingSprit(GameObject _gameObject)
    {
        gameObjectList.Add(_gameObject);
        constructionAmount++;
    }
    public void DeleteWorkingSprit(GameObject _gameObject)
    {
        gameObjectList.Remove(_gameObject);
    }

    // 건축물 데이터 테이블 동기화 => 빌딩
    private BuildData FindDataFromBuildData(List<BuildData> buildDataList, int _buildID)
    {
        foreach(BuildData buildData in buildDataList)
        {
            if(buildData.structureID == _buildID)
            {
                Debug.Log(buildData);
                return buildData;
            }
        }
        return null;
    }

    // 건축물 고유 속성 테이블 동기화 => 빌딩
    private StructUniqueData FindDataFromStructUnique(List<StructUniqueData> structUniqueData, int _buildID)
    {
        foreach(StructUniqueData uniqueData in structUniqueData)
        {
            if(uniqueData.UniqueProperties == _buildID)
            { return uniqueData; }

        }
        return null;
    }

    private void SycnXMLDataToBuilding(BuildData buildData, StructUniqueData structUniqueData)
    {
        structureID = buildData.structureID;
        structureName = buildData.structureName;
        KindOfStructure = buildData.KindOfStructure;
        stoneRequirement = buildData.stoneRequirement;
        woodRequirement = buildData.woodRequirement;
        essenceRequirement = buildData.essenceRequirement;
        UniqueProperties = buildData.UniqueProperties;
        StructureEffect = buildData.StructureEffect;
        WorkingTime = structUniqueData.WorkingTime;
        Capacity = structUniqueData.Capacity;
        HCostOfUse = structUniqueData.HCostOfUse;
        CostUseWood = structUniqueData.CostUseWood;
        CostOfStone = structUniqueData.CostOfStone;
        DemandingWork = structUniqueData.DemandingWork;
        StructureCondition = structUniqueData.StructureCondition;
    }

    void ShowBuildSlideBarToUI()
    {
        buildBar = sliderUI.GetComponentInChildren<Slider>();
        sliderUI.SetActive(true);
        buildBar.value = constructionAmount / 10;
    }




}
