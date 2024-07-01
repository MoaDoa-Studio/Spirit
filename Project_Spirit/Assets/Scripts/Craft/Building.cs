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
    GameObject PreviewParent;
    [SerializeField]
    public int GameObjectCount;
    [SerializeField]
    private float cellsize = 1f;
    [SerializeField]
    private GameObject cellPrefab;

    [HideInInspector] 
    public Vector2Int upperRight;
    public Vector2Int bottomLeft;
    public Tuple<Vector2Int, Vector2Int> connectedRoads;
    // ���� ��� ����Ʈ
    
    public List<GameObject> gameObjectList;
    BuildingDataManager buildingDataManager;
    List<BuildData> buildDataList;
    List<StructUniqueData> structUniqueDataList;
    BuildData buildData;
    StructUniqueData structUniqueData;

    GameObject gameManager;
    Slider buildBar;

    [Header("���� �Ӽ�")]
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

    // �ڿ� �Ѱ��� �����ϴµ� �ʿ��� ������ ��
    [SerializeField]
    private float EarnWoodResourceAmount = 0;
    [SerializeField]
    private float EarnRockResourceAmount = 0;
    // ���� � ���¸� ��Ÿ��
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
        // ���� ������ �ʱ�ȭ.
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

        // �ܹ߼� �ǹ� ȿ��

    }

    void BuildOperation()
    {
        switch (buildOperator)
        {   // ���� ���� �� ����
            case BuildOperator.None:
                if (constructionAmount > 0)
                { buildOperator = BuildOperator.Construct; }
                break;
            // ���� ���� �ܰ�
            case BuildOperator.Construct:

                // ���� ������ �����̴� ǥ��
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
                // ���� �������� �� ���縦 �����ϴ� ����.
                if (buildOperator == BuildOperator.Construct)
                {
                    // ���๰ �ִϸ��̼�
                }

                // ������ ��������, �ش� �ǹ��� ����ϴ� ����.
                if (buildOperator == BuildOperator.Done)
                {
                  

                        // ���๰ �ִϸ��̼� �����ֱ�
                    
                }

                break;
        }
    }
    public bool AskPermissionOfUse(GameObject _gameObject)
    {
        // ���� ���ٻ�� ���� �޼���.
        if(!RestrictAccessToBuilding())
        {  return false; }

        // �����߿� ������� ���� ������ �������� ����
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
        if (_gameObject.GetComponent<Spirit>().SpiritID == 5) return false;
        if (gameObjectList.Count >= 0 && gameObjectList.Count <= Capacity)
        {
            return true;
        }
        else
            return false;
    }
    public bool CheckForAccesibleWhenBuilt(GameObject _gameObject)
    {   
        // ���� �ΰ� �϶��� ��밡���ϰ�..
        if (connectedRoads == null) return false;
        if (_gameObject.GetComponent<Spirit>().SpiritID == 5) return false;

        if(_gameObject.GetComponent<Spirit>().SpiritID != StructureCondition) return false;

        if (gameObjectList.Count >= 0 && gameObjectList.Count <= Capacity)
        {
            //_gameObject.GetComponent<DetectMove>().TimeforWorking = WorkingTime;
            // ����Ҹ� ����Ѵٸ�...!
            if(UniqueProperties == 101)
            {
                // �� �����
                if(structureID >= 1001 && structureID  < 1004)
                {
                    // ����ڰ� �� ����Ҹ� �̿��� ��  / �� �Ѱ��� ����µ� �ʿ��� ����� �� ��ŭ ���� ����
                    EarnRockResourceAmount +=_gameObject.GetComponent<Spirit>().Work_Efficienty;
                    if (EarnRockResourceAmount > 5)
                    {
                        gameManager.GetComponentInChildren<ResouceManager>().Rock_reserves += 1;
                        EarnRockResourceAmount = 0;

                    }
                }
                // ���� �����
                else if(structureID >= 1004 && structureID < 1007)
                {
                    EarnWoodResourceAmount +=_gameObject.GetComponent<Spirit>().Work_Efficienty;
                    if (EarnWoodResourceAmount < 5)
                    {
                        gameManager.GetComponentInChildren<ResouceManager>().Timber_reserves += 1;
                        EarnWoodResourceAmount = 0;
                    }
                }
                // ������
                else if(structureID == 1007)
                {

                }
                // ����� �Ʒü�
                else if(structureID == 1008)
                {

                }
                // ���� �Ʒü�
                else if (structureID == 1009)
                {

                }
                // ��� �Ʒü�
                else if (structureID == 1010)
                {

                }
                // ���� �Ʒü�
                else if (structureID == 1011)
                {

                }
                // ���� �Ʒü�
                else if (structureID == 1012)
                {

                }
                // ġ���� �Ʒü�
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
            Debug.Log("���ΰ� 2���� �ƴմϴ�.");
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
        if (!gameObjectList.Contains(gameObject))
        {
            gameObjectList.Add(gameObject);
            constructionAmount++;
        }

        if (buildOperator == BuildOperator.Done)
        {
            if (UniqueProperties == 101)
            {
                // �� �����
                if (structureID == 1001)
                {
                    // ����ڰ� �� ����Ҹ� �̿��� ��  / �� �Ѱ��� ����µ� �ʿ��� ����� �� ��ŭ ���� ����
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
                // ���� �����
                else if (structureID == 1004)
                {
                    EarnWoodResourceAmount += 0.5f;
                    gameManager.GetComponentInChildren<ResouceManager>().Timber_reserves += 0.5f;
                }
                else if (structureID == 1005)
                {
                    EarnWoodResourceAmount += 1f;
                    gameManager.GetComponentInChildren<ResouceManager>().Timber_reserves += 1f;
                }
                else if (structureID == 1006)
                {
                    EarnWoodResourceAmount += 1.5f;
                    gameManager.GetComponentInChildren<ResouceManager>().Timber_reserves += 1.5f;
                }
            }
            // 기술자 훈련소
            if(structureID == 1008)
            {
                gameObject.GetComponent<Spirit>().SetSpiritID(1);
                // 잔여 체력을 감소
                gameObject.GetComponent<Spirit>().TakeDamageOfBuilding();
            }
            // 학자 훈련소
            else if (structureID == 1009)
            {
                gameObject.GetComponent<Spirit>().SetSpiritID(2);
                // 잔여 체력을 감소
                gameObject.GetComponent<Spirit>().TakeDamageOfBuilding();
            }
            // 기사 훈련소
            else if (structureID == 1010)
            {
                gameObject.GetComponent<Spirit>().SetSpiritID(4);
                // 잔여 체력을 감소
                gameObject.GetComponent<Spirit>().TakeDamageOfBuilding();
            }
            // 장사 훈련소
            else if (structureID == 1011)
            {
                // 장사는 아직 구현이 안되어 있음.
                gameObject.GetComponent<Spirit>().SetSpiritID(3);
                // 잔여 체력을 감소
                gameObject.GetComponent<Spirit>().TakeDamageOfBuilding();
            }
            // 귀족 훈련소
            else if (structureID == 1012)
            {
                gameObject.GetComponent<Spirit>().SetSpiritID(5);
                // 잔여 체력을 증가시킴
                gameObject.GetComponent<Spirit>().TakeAdvantageOfBuilding();
                
            }
            // 치유 훈련소
            else if (structureID == 1013)
            {
                gameObject.GetComponent<Spirit>().SetSpiritID(6);
                // 잔여체력을 감소
                gameObject.GetComponent<Spirit>().TakeDamageOfBuilding();
            }
           
                    
                

            } 

        } 
    
    public void DeleteWorkingSprit(GameObject _gameObject) 
    {
        gameObjectList.Remove(_gameObject);
       
    }

    // ���๰ ������ ���̺� ����ȭ => ����
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

    // ���๰ ���� �Ӽ� ���̺� ����ȭ => ����
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
            //Debug.Log("�̿����� ���մϴ�");
        return false;
        }
        
        else
            return true;
    
    }

    public void BuildingExpense(GameObject _targetObject)
    {
        // ���� ü�� ���� ����
        if(_targetObject.GetComponent<Spirit>().SpiritID != 3)
        {
            _targetObject.GetComponent<Spirit>().SDefaultLife -= HCostOfUse;
        }
        gameManager.GetComponentInChildren<ResouceManager>().Rock_reserves -= CostOfStone;
        gameManager.GetComponentInChildren<ResouceManager>().Rock_reserves -= CostUseWood;
        gameManager.GetComponentInChildren<ResouceManager>().Essence_reserves -= essenceRequirement;
        
        // �߰������� ���� �������������� �ǹ��� �̿����� ���ϰ� �ؾ���
    }

    private void OnEnable()
    {
        isActivateCondition();
    }

    // ��ġ������ ȿ���� �ο��ؾ��ϴ� �ǹ��϶� �ش� Ư�� ����
    void isActivateCondition()
    {
        // ���� ȿ���� ����Ǵ� UniqueProperties 107�� ������
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
            // �ҹ��� ������ ����
            else if(StructureEffect == 220)
            {
               // GenerateHealGrid(bottomLeft, 7, 7, cellsize);
            }
            // ���޽��� ������ ����
            else if(StructureEffect == 221)
            {
                    
            }
        }

    }

    // ��Ȱ��ȭ �ɶ� ���� Ư��
    private void OnDisable()
    {
        DeactivateCondition();
    }
    void DeactivateCondition()
    {
        // ���� ȿ���� ����Ǵ� UniqueProperties 107�� ������
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
            // �ҹ��� ������ ����
            else if(StructureEffect == 220)
            {
                //GenerateHealGrid(bottomLeft, 7, 7, cellsize);
            }
            // ���޽��� ������ ����
            else if(StructureEffect == 221)
            {
               // GenerateHealGrid(bottomLeft, 9, 9, cellsize);
            }
        }
    }

    // �ҹ� => 60�� �� / ���� => 25�� ��
    void GenerateHealGrid(Vector2 center, int rows, int cols, float cellsize)
    {
        int halfRow = rows / 2;
        int halfCol = cols / 2;

        for(int i = -halfRow; i <= halfCol; i++)
        {
            for(int j = -halfCol; i <= halfCol; j++)
            {
                // �ܰ� �� ����
                Vector2 position = new Vector2(center.x + j * cellsize , center.y + i * cellsize);
                Instantiate(cellPrefab, position, quaternion.identity);
            }
        }
    }

    private void OnMouseButtonDown()
    {
        if(PreviewParent != null)
        {
            PreviewParent.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (PreviewParent != null)
        {
            PreviewParent.SetActive(false);
        }
    }
}
