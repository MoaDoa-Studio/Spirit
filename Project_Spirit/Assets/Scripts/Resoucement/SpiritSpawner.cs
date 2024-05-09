using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SpiritSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] allPrefabs;
    [SerializeField]
    GameObject SpawnUI;
    GameObject Spiritprefab;
    
    public bool Fire;
    public bool Water;
    public bool Ground;
    public bool Air;
    
    public string SpawnerName;
    [HideInInspector]
    public float sliderValue = 0;
    public int elementNum =0;

    int[,] Area = new int[103,103];
    float[] Spawn = new float[] { 2f, 1.5f, 1f };   // 24, 18, 12
    float SpawnDuration = 0f;
    
    float gameTimer = 0f;
    float realTimeToGameTimeRatio = 720f;
    int spLv = 1;
    int spwLv = 2;

    Vector2 bottomLeft;
    Vector2 topRight;
    [HideInInspector]
    Text textComp;

    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    private void Start()
    {
        SetSpawnerType();
        SpawnDuration = Spawn[0];
        SpawnUI.GetComponent<SpawnerUI>().ReceiveDefaultSpiritSpawnInfo(SetUIInfo());
    }

    private void Update()
    {
       // float spawnTime = slider.value * 1440f;
        // 현실 1초 => 12분 계산 24분 => 
        gameTimer += Time.deltaTime * realTimeToGameTimeRatio;

        float spawnTime = sliderValue * 720f;
        if (gameTimer >= realTimeToGameTimeRatio * SpawnDuration + spawnTime)
        {
            SpawnSpirit();
           
            gameTimer = 0f;
        }
       
        
    }
    void SpawnSpirit()
    {
        Vector2Int? OneRoad = isOneRoadAttached();
        
        if( OneRoad.HasValue)
        {
            Debug.Log("한길잇음");
            Vector2Int road = OneRoad.Value;
            int row = road.x;
            int col = road.y;
            
            Vector3 newPosition = new Vector3(row, col, 0);
            GameObject SpiritObject = Instantiate(Spiritprefab, new Vector3(newPosition.x + 0.5f, newPosition.y + 0.5f, 0), Quaternion.identity);
            SpiritObject.GetComponent<DetectMove>().CurposX = row;
            SpiritObject.GetComponent<DetectMove>().CurposY = col;
            SpiritObject.GetComponent<DetectMove>()._dir = Redirection(row, col);
            
            
        }
        else
        {
           // Debug.Log("Oneroad is null");
        }
    }

    #region 정령 생산소 세팅
    void SetSpawnerType()
    {
        if(Fire) { SpawnerName = "불"; }
        if (Water) { SpawnerName = "물"; }
        if(Ground) { SpawnerName = "흙"; }
        if(Air){ SpawnerName = "공기"; };
        SetRangeOfBuilding(SpawnerName);
    }

    void SetRangeOfBuilding(string name)
    {
        switch(name)
        {
            case "불":
                SetFireMap();
                break;
            case "물":
                SetWaterMAp();
                break;
            case "흙":
                SetGroundMap();
                break;
            case "공기":
                SetAirMap();
                break;
        }
    }

    void SetAirMap()
    {
        for(int i = 0; i < 6; i++)
        {
            for(int j = 50; j < 54; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[3];
                elementNum = 3;
                //Debug.Log("공기영역");
            }
        }
        bottomLeft = new Vector2(0, 50);
        topRight = new Vector2(6, 54);
    }

    void SetWaterMAp()
    {
        for (int i = 50; i < 56; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[1];
                //Debug.Log("물영역");
                elementNum = 1;
            }
        }
        bottomLeft = new Vector2(50, 0);
        topRight = new Vector2(56, 4);
    }

    void SetGroundMap()
    {
        for (int i = 97; i <= 102; i++)
        {
            for (int j = 50; j < 54; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[2];
                //Debug.Log("땅영역");
                elementNum = 2;
            }
        }
        bottomLeft = new Vector2(97, 50);
        topRight = new Vector2(102, 54);
    }

    void SetFireMap()
    {
        for (int i = 49; i < 55; i++)
        {
            for (int j = 99; j <= 102; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[0];
                //Debug.Log("불영역");
                elementNum = 0;
            }
        }
        bottomLeft = new Vector2(49, 99);
        topRight = new Vector2(55, 102);
    }
    #endregion

    Vector2Int? isOneRoadAttached()
    {
        List<Vector2Int> resultList = new List<Vector2Int>();
        for (int i = (int)bottomLeft.x; i < topRight.x; i++)
        {
            for(int j = (int)bottomLeft.y; j <topRight.y; j++)
            {
                if (Area[i, j] == 1)
                {
                    int[] SpawndirX = { 0, 0, 1, -1 };
                    int[] SpawndirY = { 1, -1, 0, 0 };
                    for(int k = 0; k < 4; k++)
                    {
                        int newX = i + SpawndirX[k];
                        int newY = j + SpawndirY[k];
                    
                        // 새로운 인덱스가 배열 범위 내에 있는지 확인합니다.
                        if (newX >= 0 && newX < Area.GetLength(0) && newY >= 0 && newY < Area.GetLength(1))
                        {
                            // 상하좌우에 해당하는 인덱스의 값이 3인지 확인하고, 맞다면 리스트에 추가합니다.
                            if (!resultList.Contains(new Vector2Int(newX,newY)) &&TileDataManager.instance.GetTileType(newX, newY) == 3)
                            {
                                Vector2Int newvec = new Vector2Int(newX,newY);
                                resultList.Add(newvec);
                            }
                        }
                    }
                }
            }
        }
       
        if (resultList.Count == 1)
            return resultList[0];
        else
            return null;
        
    }

    private int Redirection(int _row, int _col)
    {
        
        int[] dirX = { 0, 0, 1, -1 };
        int[] dirY = { 1, -1, 0, 0 };

        int resultDir = 0;
        for (int i = 0; i < 4; i++)
        {
            if (TileDataManager.instance.GetTileType(_row + dirX[i], _col + dirY[i]) == 1)
            {
                if (i == 0)
                {
                    resultDir = (int)Dir.Down;
                    //Debug.Log(resultDir);
                    break;
                }
                else if (i == 1)
                {
                    resultDir = (int)Dir.Up;
                    //Debug.Log(resultDir);
                    break;
                }
                else if (i == 2)
                {
                    resultDir = (int)Dir.Left;
                    //Debug.Log(resultDir);
                    break;
                }
                else
                {
                    resultDir = (int)Dir.Right;
                    //Debug.Log(resultDir);
                    break;
                }
            }
        }
            return resultDir;
    }

    private void OnMouseDown()
    {
        SpawnUI.SetActive(true);
       
        // UI 최초 데이터 sync.
        SpawnUI.GetComponent<SpawnerUI>().ReceiveSpiritSpawnInfo(SetUIInfo(), this.gameObject);

    }

    SpiritSpawnInfo SetUIInfo()
    {
        // SpiritSpawnInfo 인스턴스 생성 및 정보 채우기
        SpiritSpawnInfo spawnInfo = new SpiritSpawnInfo();
        spawnInfo.SpawnerName = SpawnerName;
        spawnInfo.spawnDuration = SpawnDuration;
        spawnInfo.SpiritLv = spLv;
        spawnInfo.SpwnLv = spwLv;
        spawnInfo.elementNum = elementNum;
        
        return spawnInfo;
    }
    
}

public class SpiritSpawnInfo
{
    public float spawnDuration;
    public string SpawnerName;
    public int SpwnLv;
    public int SpiritLv;
    public int elementNum;
    public float sliderValue;

    public void UpdateFrom(SpiritSpawnInfo other)
    {
        this.spawnDuration = other.spawnDuration;
        this.SpawnerName = other.SpawnerName;
        this.SpwnLv = other.SpwnLv;
        this.SpiritLv = other.SpiritLv;
        this.sliderValue = other.sliderValue;
        this.elementNum = other.elementNum;
        // slider는 참조형이므로 직접 값을 복사할 필요 없음
    }
}