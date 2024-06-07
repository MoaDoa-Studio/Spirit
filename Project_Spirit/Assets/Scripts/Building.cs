using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField]
    private int BuildID;
    [SerializeField]
    GameObject sliderUI;
    [SerializeField]
    public int GameObjectCount;
    [SerializeField]
    private float cellsize = 1f;
    [SerializeField]
    private GameObject cellPrefab;

    [HideInInspector]   
    public Vector2Int upperRight;
    [HideInInspector]   
    public Vector2Int bottomLeft;
    public Tuple<Vector2Int, Vector2Int> connectedRoads;
    // 정령 담는 리스트
    
    public List<GameObject> gameObjectList;
    BuildingDataManager buildingDataManager;
    List<BuildData> buildDataList;
    List<StructUniqueData> structUniqueDataList;
    BuildData buildData;
    StructUniqueData structUniqueData;

    GameObject gameManager;
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

    // 자원 한개를 생산하는데 필요한 갯수의 합
    [SerializeField]
    private float EarnWoodResourceAmount = 0;
    [SerializeField]
    private float EarnRockResourceAmount = 0;
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
        gameManager = GameObject.Find("GameManager");
        // 빌딩 데이터 초기화.
        buildDataList = GameObject.Find("GameManager").GetComponent<BuildingDataManager>().buildDataList;
        structUniqueDataList = GameObject.Find("GameManager").GetComponent<BuildingDataManager>().structUniqueDataList;
        buildData = FindDataFromBuildData(buildDataList, BuildID);
       
        if (buildData == null)
        {
            Debug.LogError("BuildData not found for BuildID: " + BuildID);
            return;
        }
        structUniqueData = FindDataFromStructUnique(structUniqueDataList, buildData.UniqueProperties);
        SycnXMLDataToBuilding(buildData, structUniqueData);
    }
    private void Update()
    {
        gameObjectList.RemoveAll(item => item == null);
        BuildOperation();
        BuildStater();

        // 단발성 건물 효과

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
                if(UniqueProperties != 107)
                ShowBuildSlideBarToUI();

                if (constructionAmount > 10)
                {
                    buildOperator = BuildOperator.Done; 
                    break;
                }
                break;
            case BuildOperator.Done:
                if(UniqueProperties != 107)
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
                if (gameObjectList.Count == 0)
                {
                    buildState = BuildState.Rest;
                    break;
                }
                // 건축 지어지기 전 공사를 진행하는 상태.
                if (buildOperator == BuildOperator.Construct)
                {
                    // 건축물 애니메이션
                }

                // 건축이 지어졌고, 해당 건물을 사용하는 상태.
                if (buildOperator == BuildOperator.Done)
                {
                  

                        // 건축물 애니메이션 보여주기
                    
                }

                break;
        }
    }
    public bool AskPermissionOfUse(GameObject _gameObject)
    {
        // 정령 접근사용 제어 메서드.
        if(!RestrictAccessToBuilding())
        {  return false; }

        // 공사중엔 사용접근 제한 정령이 존재하지 않음
        if(buildOperator != BuildOperator.Done)
        {
            if(CheckForAccesibleBeforeBuilt(_gameObject))
            { return true; }
        }
        else
        {
            if (CheckForAccesibleWhenBuilt(_gameObject))
                return true;
        }
        return false;
    }

    public bool CheckForAccesibleBeforeBuilt(GameObject _gameObject)
    {
        if(connectedRoads == null) return false;

        if (gameObjectList.Count >= 0 && gameObjectList.Count <= Capacity)
        {
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

        if (gameObjectList.Count >= 0 && gameObjectList.Count <= Capacity)
        {
            //_gameObject.GetComponent<DetectMove>().TimeforWorking = WorkingTime;
            // 생산소를 사용한다면...!
            if(UniqueProperties == 101)
            {
                // 돌 생산소
                if(structureID >= 1001 && structureID  < 1004)
                {
                    // 기술자가 돌 생산소를 이용할 시  / 돌 한개를 만드는데 필요한 기술자 수 만큼 나무 증가
                    EarnRockResourceAmount +=_gameObject.GetComponent<Spirit>().Work_Efficienty;
                    if (EarnRockResourceAmount > 5)
                    {
                        gameManager.GetComponentInChildren<ResouceManager>().Rock_reserves += 1;
                        EarnRockResourceAmount = 0;

                    }
                }
                // 나무 생산소
                else if(structureID >= 1004 && structureID < 1007)
                {
                    EarnWoodResourceAmount +=_gameObject.GetComponent<Spirit>().Work_Efficienty;
                    if (EarnWoodResourceAmount < 5)
                    {
                        gameManager.GetComponentInChildren<ResouceManager>().Timber_reserves += 1;
                        EarnWoodResourceAmount = 0;
                    }
                }
                // 연구소
                else if(structureID == 1007)
                {

                }
                // 기술자 훈련소
                else if(structureID == 1008)
                {

                }
                // 학자 훈련소
                else if (structureID == 1009)
                {

                }
                // 기사 훈련소
                else if (structureID == 1010)
                {

                }
                // 전사 훈련소
                else if (structureID == 1011)
                {

                }
                // 귀족 훈련소
                else if (structureID == 1012)
                {

                }
                // 치유사 훈련소
                else if (structureID == 1013)
                {

                }
            }
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


    public void AddWorkingSprit(GameObject gameObject)
    {
        gameObjectList.Add(gameObject);
        constructionAmount++;

        if (buildOperator == BuildOperator.Done)
        {
            if (UniqueProperties == 101)
            {
                // 돌 생산소
                if (structureID == 1001)
                {
                    // 기술자가 돌 생산소를 이용할 시  / 돌 한개를 만드는데 필요한 기술자 수 만큼 나무 증가
                    EarnRockResourceAmount += 0.5f;
                    gameManager.GetComponentInChildren<ResouceManager>().Rock_reserves += 0.5f;
                }
                else if (structureID == 1002)
                {
                    EarnRockResourceAmount += 1f;
                    gameManager.GetComponentInChildren<ResouceManager>().Rock_reserves += 1f;
                }
                else if (structureID == 1003)
                {
                    EarnRockResourceAmount += 1.5f;
                    gameManager.GetComponentInChildren<ResouceManager>().Rock_reserves += 1.5f;
                }
                // 나무 생산소
                else if (structureID >= 1004) 
                {
                    EarnWoodResourceAmount += 0.5f;
                    gameManager.GetComponentInChildren<ResouceManager>().Timber_reserves += 0.5f;
                }
                else if (structureID >= 1005) 
                {
                    EarnWoodResourceAmount += 1f;
                    gameManager.GetComponentInChildren<ResouceManager>().Timber_reserves += 1f;
                }
                else if (structureID >= 1004)
                {
                    EarnWoodResourceAmount += 1.5f;
                    gameManager.GetComponentInChildren<ResouceManager>().Timber_reserves += 1.5f;
                }
                    // 연구소
                else if (structureID == 1007)
                {

                }
                // 기술자 훈련소
                else if (structureID == 1008)
                {

                }
                // 학자 훈련소
                else if (structureID == 1009)
                {

                }
                // 기사 훈련소
                else if (structureID == 1010)
                {

                }
                // 전사 훈련소
                else if (structureID == 1011)
                {

                }
                // 귀족 훈련소
                else if (structureID == 1012)
                {

                }
                // 치유사 훈련소
                else if (structureID == 1013)
                {

                }
                    
                

            } 
        } 
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

    bool RestrictAccessToBuilding()
    {
        if(UniqueProperties == 107)
        {
            //Debug.Log("이용하지 못합니다");
        return false;
        }
        
        else
            return true;
    
    }

    public void BuildingExpense(GameObject _targetObject)
    {
        // 정령 체력 감소 적용
        _targetObject.GetComponent<Spirit>().SDefaultLife -= HCostOfUse;
        gameManager.GetComponentInChildren<ResouceManager>().Rock_reserves -= CostOfStone;
        gameManager.GetComponentInChildren<ResouceManager>().Rock_reserves -= CostUseWood;
        gameManager.GetComponentInChildren<ResouceManager>().Essence_reserves -= essenceRequirement;
        
        // 추가적으로 돈이 부족했을떄에는 건물을 이용하지 못하게 해야함
    }

    private void OnEnable()
    {
        isActivateCondition();
    }

    // 설치했을때 효과를 부여해야하는 건물일때 해당 특성 발현
    void isActivateCondition()
    {
        // 건축 효과가 적용되는 UniqueProperties 107가 발현됨
        if(UniqueProperties == 107)
        {
            if(StructureEffect == 214)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Rock_reserves += 300;
            }
            else if(StructureEffect == 215)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Rock_reserves += 500;
            }
            else if (StructureEffect == 216)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Rock_reserves += 1000;
            }
            else if (StructureEffect == 217)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Timber_reserves += 300;
            }
            else if (StructureEffect == 218)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Timber_reserves += 500;
            }
            else if (StructureEffect == 219)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Timber_reserves += 500;
            }
            // 소박한 마법의 동상
            else if(StructureEffect == 220)
            {

            }
            // 고급스런 마법의 동상
            else if(StructureEffect == 221)
            {

            }
        }

    }

    // 비활성화 될때 상태 특성
    private void OnDisable()
    {
        DeactivateCondition();
    }
    void DeactivateCondition()
    {
        // 건축 효과가 적용되는 UniqueProperties 107가 발현됨
        if (UniqueProperties == 107)
        {
            if (StructureEffect == 214)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Rock_reserves -= 300;
            }
            else if (StructureEffect == 215)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Rock_reserves -= 500;
            }
            else if (StructureEffect == 216)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Rock_reserves -= 1000;
            }
            else if (StructureEffect == 217)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Timber_reserves -= 300;
            }
            else if (StructureEffect == 218)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Timber_reserves -= 500;
            }
            else if (StructureEffect == 219)
            {
                gameManager.GetComponentInChildren<ResouceManager>().Max_Timber_reserves -= 500;
            }
            // 소박한 마법의 동상
            else if(StructureEffect == 220)
            {
                GenerateHealGrid(bottomLeft, 7, 7, cellsize);
            }
            // 고급스런 마법의 동상
            else if(StructureEffect == 221)
            {
                GenerateHealGrid(bottomLeft, 9, 9, cellsize);
            }
        }
    }

    // 소박 => 60분 쿨 / 고급 => 25분 쿨
    void GenerateHealGrid(Vector2 center, int rows, int cols, float cellsize)
    {
        int halfRow = rows / 2;
        int halfCol = cols / 2;

        for(int i = -halfRow; i <= halfCol; i++)
        {
            for(int j = -halfCol; i <= halfCol; j++)
            {
                // 외곽 셀 생성
                Vector2 position = new Vector2(center.x + j * cellsize , center.y + i * cellsize);
                Instantiate(cellPrefab, position, quaternion.identity);
            }
        }
    }

 
}
