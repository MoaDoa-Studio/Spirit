using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class SpiritSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] allPrefabs;
    [SerializeField]
    GameObject SpawnUI;
    [SerializeField]
    Transform SpawnParent;

    GameObject Spiritprefab;
    [SerializeField]
    List<GameObject> setPrefab = new List<GameObject>();

    public bool PathtoCradle = false;
    public bool Fire;
    public bool Water;
    public bool Ground;
    public bool Air;

    // 요람까지 도달 체크 배열
    private bool[,] visited;
    public string SpawnerName;
    [HideInInspector]
    public float sliderValue = 0;
    [HideInInspector]
    public int elementNum = 0;

    public GameObject noteTilePrefab; // 프리팹을 지정합니다.
    private GameObject[] NoteTile = new GameObject[2];

    int[,] Area = new int[103, 103];
    float[] Spawn = new float[] {8f, 8f, 6f, 4f };   // 정령 스폰 시간 1,2,3 단계  96, 72, 48
    float[] sliderBar = new float[] { 96, 96, 72, 48f };    // 슬라이더바 값 조정
    float SpawnDuration = 0f;

    float gameTimer = 0f;
    float realTimeToGameTimeRatio = 720f;
    public int spLv { get; set; } = 1;
    public int spwLv { get; set; } = 1;

    TimeManager timeManager;
    int termcnt = 0;
    Vector2 bottomLeft;
    Vector2 topRight;
    [SerializeField]
    Vector2Int road;
    [HideInInspector]
    Text textComp;
    Node[,] nodes;

    Vector2Int spawnPos1;
    Vector2Int spawnPos2;

    private float alphaMin = 67f / 255f;
    private float alphaMax = 180f / 255f;
    private float duration = 1.0f;

 
    // 정령 생산 속도 가중치.
    public float spawnWeight = 1f;
    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    private void Start()
    {
        nodes = TileDataManager.instance.GetNodes();
        SetSpawnerType();
        SpawnDuration = Spawn[0];
        SpawnUI.GetComponent<SpawnerUI>().ReceiveDefaultSpiritSpawnInfo(SetUIInfo());
        timeManager = GameObject.Find("TimeNTemperatureManager").GetComponent<TimeManager>();
    }

    private void Update()
    {
        // float spawnTime = slider.value * 1440f;

        // 현실 1초 => 12분 계산 24분 => 

        if(isTwoRoadAttached())
        {
           NoteTile[0].gameObject.SetActive(false);
           NoteTile[1].gameObject.SetActive(false);

            if (CanReachCradle(road))
            {
                //Debug.Log("길이 정령까지 존재합니다.");

                gameTimer += Time.deltaTime * realTimeToGameTimeRatio *timeManager.timeSpeed;

                float spawnTime = (sliderValue / sliderBar[spwLv]) * 720f;
                if (gameTimer >= realTimeToGameTimeRatio * Spawn[spwLv] + spawnTime / spawnWeight)
                {
                    SpawnSpirit();

                    gameTimer = 0f;
                }
            }
            else
            {
                Debug.Log("정령왕의 요람까지 가는길이 존재하지 않습니다.");
            }

        }
        else
        {
            // 알림 타일맵 알림 부여
            NoticeTileMap(); 
            //Debug.Log("Oneroad is null");
        }
       

    }
    void SpawnSpirit()
    { 
        if(isTwoRoadAttached())
        {
            CheckSpawnAttachedRoad();
        }
       
    }

    #region 정령 생산소 세팅
    void SetSpawnerType()
    {
        if (Fire) { SpawnerName = "불"; }
        if (Water) { SpawnerName = "물"; }
        if (Ground) { SpawnerName = "흙"; }
        if (Air) { SpawnerName = "공기"; };
        SetRangeOfBuilding(SpawnerName);
    }

    void SetRangeOfBuilding(string name)
    {
        switch (name)
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
        for (int i = 51; i < 55; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[3];
                elementNum = 4;
                SetGamePrefabForSpawner();
                spawnPos1 = new Vector2Int(52, 6);
                spawnPos2 = new Vector2Int(53, 6);
                //Debug.Log("공기영역");
            }
        }
        bottomLeft = new Vector2(51, 0);
        topRight = new Vector2(55, 6);
    }

    void SetWaterMAp()
    {
        for (int i = 51; i < 55; i++)
        {
            for (int j = 97; j < 103; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[1];
                //Debug.Log("물영역");
                elementNum = 2;
                SetGamePrefabForSpawner();
                spawnPos1 = new Vector2Int(52, 96);
                spawnPos2 = new Vector2Int(53, 96);
            }
        }
        bottomLeft = new Vector2(51, 97);
        topRight = new Vector2(55, 103);
    }

    void SetGroundMap()
    {
        for (int i = 97; i < 103; i++)
        {
            for (int j = 49; j < 53; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[2];
                //Debug.Log("땅영역");
                elementNum = 3;
                SetGamePrefabForSpawner();
                spawnPos1 = new Vector2Int(96, 49);
                spawnPos2 = new Vector2Int(96, 50);
            }
        }
        bottomLeft = new Vector2(97, 49);
        topRight = new Vector2(103, 53);
    }

    void SetFireMap()
    {
        for (int i = 0; i < 6; i++)
        {
            for (int j = 49; j < 53; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 1);
                Area[i, j] = 1;
                Spiritprefab = allPrefabs[0];
                //Debug.Log("불영역");
                elementNum = 1;
                SetGamePrefabForSpawner();
                spawnPos1 = new Vector2Int(6, 49);
                spawnPos2 = new Vector2Int(6, 50);
            }
        }
        bottomLeft = new Vector2(0, 49);
        topRight = new Vector2(6, 53);
    }
    #endregion

    private bool isTwoRoadAttached()
    {
        if (TileDataManager.instance.GetTileType(spawnPos1.x, spawnPos1.y) == 3 || TileDataManager.instance.GetTileType(spawnPos2.x, spawnPos2.y) == 3)
        {
            if(TileDataManager.instance.GetTileType(spawnPos1.x, spawnPos1.y) == 3)
            { road = spawnPos1; }
            else if(TileDataManager.instance.GetTileType(spawnPos2.x, spawnPos2.y) == 3)
            {   road = spawnPos2;}
            return true;
        }
        else
        {
            road = Vector2Int.zero;
            return false;
        }
        
    }

    private void CheckSpawnAttachedRoad()
    {
        // 두 곳 모두 봉인되었을때
        if (TileDataManager.instance.GetTileType(spawnPos1.x, spawnPos1.y) == 3 && TileDataManager.instance.GetTileType(spawnPos2.x, spawnPos2.y) == 3)
        {
            if (nodes[spawnPos1.x, spawnPos1.y].spiritElement == elementNum && nodes[spawnPos2.x, spawnPos2.y].spiritElement == elementNum) return;
            
            if (termcnt % 2 == 0)
            {
                termcnt++;
                Vector3 newPosition = new Vector3(spawnPos1.x, spawnPos1.y, 0);
                GameObject SpiritObject = Instantiate(Spiritprefab, new Vector3(newPosition.x + 0.5f, newPosition.y + 0.5f, 0), Quaternion.identity, SpawnParent);
                SpiritObject.GetComponent<DetectMove>().CurposX = spawnPos1.x + 0.5f;
                SpiritObject.GetComponent<DetectMove>().CurposY = spawnPos1.y + 0.5f;
                SpiritObject.GetComponent<DetectMove>()._dir = Redirection(spawnPos1.x, spawnPos1.y);
                return;
            }
            else
            {
                termcnt = 0;
                Vector3 newPosition = new Vector3(spawnPos2.x, spawnPos2.y, 0);
                GameObject SpiritObject = Instantiate(Spiritprefab, new Vector3(newPosition.x + 0.5f, newPosition.y + 0.5f, 0), Quaternion.identity, SpawnParent);
                SpiritObject.GetComponent<DetectMove>().CurposX = spawnPos2.x + 0.5f;
                SpiritObject.GetComponent<DetectMove>().CurposY = spawnPos2.y + 0.5f;
                SpiritObject.GetComponent<DetectMove>()._dir = Redirection(spawnPos2.x, spawnPos2.y);
                return;
            }

        }
        if (TileDataManager.instance.GetTileType(spawnPos1.x, spawnPos1.y) == 3 && TileDataManager.instance.GetTileType(spawnPos2.x, spawnPos2.y) != 3)
        {
            if (nodes[spawnPos1.x, spawnPos1.y].spiritElement == elementNum) return;
            Vector3 newPosition = new Vector3(spawnPos1.x, spawnPos1.y, 0);
            GameObject SpiritObject = Instantiate(Spiritprefab, new Vector3(newPosition.x + 0.5f, newPosition.y + 0.5f, 0), Quaternion.identity, SpawnParent);
            SpiritObject.GetComponent<DetectMove>().CurposX = spawnPos1.x + 0.5f;
            SpiritObject.GetComponent<DetectMove>().CurposY = spawnPos1.y + 0.5f;
            SpiritObject.GetComponent<DetectMove>()._dir = Redirection(spawnPos1.x, spawnPos1.y);

        }
        else if (TileDataManager.instance.GetTileType(spawnPos1.x, spawnPos1.y) != 3 && TileDataManager.instance.GetTileType(spawnPos2.x, spawnPos2.y) == 3)
        {
            if (nodes[spawnPos2.x, spawnPos2.y].spiritElement == elementNum) return;
            Vector3 newPosition = new Vector3(spawnPos2.x, spawnPos2.y, 0);
            GameObject SpiritObject = Instantiate(Spiritprefab, new Vector3(newPosition.x + 0.5f, newPosition.y + 0.5f, 0), Quaternion.identity, SpawnParent);
            SpiritObject.GetComponent<DetectMove>().CurposX = spawnPos2.x + 0.5f;
            SpiritObject.GetComponent<DetectMove>().CurposY = spawnPos2.y + 0.5f;
            SpiritObject.GetComponent<DetectMove>()._dir = Redirection(spawnPos2.x, spawnPos2.y);
        }
    }

    Vector2Int? isOneRoadAttached()
    {
        List<Vector2Int> resultList = new List<Vector2Int>();
        for (int i = (int)bottomLeft.x; i < topRight.x; i++)
        {
            for (int j = (int)bottomLeft.y; j < topRight.y; j++)
            {
                if (Area[i, j] == 1)
                {
                    int[] SpawndirX = { 0, 0, 1, -1 };
                    int[] SpawndirY = { 1, -1, 0, 0 };
                    for (int k = 0; k < 4; k++)
                    {
                        int newX = i + SpawndirX[k];
                        int newY = j + SpawndirY[k];

                        // 새로운 인덱스가 배열 범위 내에 있는지 확인합니다.
                        if (newX >= 0 && newX < Area.GetLength(0) && newY >= 0 && newY < Area.GetLength(1))
                        {
                            // 상하좌우에 해당하는 인덱스의 값이 3인지 확인하고, 맞다면 리스트에 추가합니다.
                            if (!resultList.Contains(new Vector2Int(newX, newY)) && TileDataManager.instance.GetTileType(newX, newY) == 3)
                            {
                                Vector2Int newvec = new Vector2Int(newX, newY);
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

    // 정령 단계 실질적인 업그레이드를 구현하는 메서드.
    public void UpgradeByUIButton()
    {
        if(spLv == 2)
        {
            Spiritprefab = setPrefab[0];
        }
        else if(spLv == 3)
        {
            Spiritprefab = setPrefab[1];
        }
    }

    // 업그레이드에 필요한 prefab 저장하는 메서드.
    private void SetGamePrefabForSpawner()
    {
        GameObject tempLv2Prefab = null;
        GameObject tempLv3Prefab = null;

        if(Spiritprefab.name == "Spirit_Fire")
        {
            tempLv2Prefab = Resources.Load<GameObject>("SpiritObject/Spirit_Fire2");
            tempLv3Prefab = Resources.Load<GameObject>("SpiritObject/Spirit_Fire3");

        }
        else if(Spiritprefab.name == "Spirit_Water")
        {
            tempLv2Prefab = Resources.Load<GameObject>("SpiritObject/Spirit_Water2");
            tempLv3Prefab = Resources.Load<GameObject>("SpiritObject/Spirit_Water3");
        }
        else if(Spiritprefab.name == "Spirit_Soil")
        {
            tempLv2Prefab = Resources.Load<GameObject>("SpiritObject/Spirit_Soil2");
            tempLv3Prefab = Resources.Load<GameObject>("SpiritObject/Spirit_Soil3");
        }
        else
        {
            tempLv2Prefab = Resources.Load<GameObject>("SpiritObject/Spirit_Air2");
            tempLv3Prefab = Resources.Load<GameObject>("SpiritObject/Spirit_Air3");
        }

        if (tempLv2Prefab != null && !IsPrefabAlreadyAdded(setPrefab, tempLv2Prefab))
        {
            setPrefab.Add(tempLv2Prefab);
        }

        if (tempLv3Prefab != null && !IsPrefabAlreadyAdded(setPrefab, tempLv3Prefab))
        {
            setPrefab.Add(tempLv3Prefab);
        }


    }

    // 리스트에 이미 같은 이름을 가진 프리팹이 이미 추가되어 있는지 확인하는 메서드.
    bool IsPrefabAlreadyAdded(List<GameObject> list, GameObject prefab)
    {
        foreach (GameObject obj in list)
        {
            if (obj.name == prefab.name)
            {
                // 이미 추가된 프리팹이 있는 경우
                //Debug.LogWarning("Prefab '" + prefab.name + "' is already added to the list.");
                return true;
            }
        }
        // 추가된 프리팹이 없는 경우
        return false;
    }


    bool CanReachCradle(Vector2Int startPoint)
    {
        nodes = TileDataManager.instance.GetNodes();
        visited = new bool[103, 103];

        return DFS(startPoint.x, startPoint.y);
    }

    bool DFS(int x, int y)
    {
        if (x < 0 || y < 0 || x >= 102 || y >= 102 || visited[x, y])
        {
            return false;
        }
        visited[x, y] = true;

        // 요람
        if(TileDataManager.instance.GetTileType(x,y) == 2)
        {
            //Debug.Log(" 정령까지 가는 요람길이 존재합니다.");
            return true;
        }
        if (TileDataManager.instance.GetTileType(x, y) != 1 && TileDataManager.instance.GetTileType(x, y) != 3 && TileDataManager.instance.GetTileType(x, y) != 4
            && TileDataManager.instance.GetTileType(x, y) != 5 && TileDataManager.instance.GetTileType(x, y) != 6 && TileDataManager.instance.GetTileType(x, y) != 7)
        {
            return false;
        }

        // 상하좌우 탐색
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1), // 상
            new Vector2Int(0, -1), // 하
            new Vector2Int(1, 0), // 우
            new Vector2Int(-1, 0) // 좌
        };

        foreach (var direction in directions)
        {
            int newX = x + direction.x;
            int newY = y + direction.y;

            if (DFS(newX, newY))
            {
                return true;
            }
        }

        return false;
    }

    private void NoticeTileMap()
    {
        // NoteTile[0]이 null이면 프리팹을 생성합니다.
        if (NoteTile[0] == null)
        {
            NoteTile[0] = Instantiate(noteTilePrefab, new Vector3(spawnPos1.x + 0.5f, spawnPos1.y + 0.5f, 0), Quaternion.identity);
        }
        else
        {
            NoteTile[0].transform.position = new Vector3(spawnPos1.x + 0.5f, spawnPos1.y + 0.5f, 0);
            NoteTile[0].SetActive(true); // 이미 존재하는 프리팹을 활성화합니다.
        }

        // NoteTile[1]이 null이면 프리팹을 생성합니다.
        if (NoteTile[1] == null)
        {
            NoteTile[1] = Instantiate(noteTilePrefab, new Vector3(spawnPos2.x + 0.5f, spawnPos2.y + 0.5f, 0), Quaternion.identity);
        }
        else
        {
            NoteTile[1].transform.position = new Vector3(spawnPos2.x + 0.5f, spawnPos2.y + 0.5f, 0);
            NoteTile[1].SetActive(true); // 이미 존재하는 프리팹을 활성화합니다.
        }


        StartCoroutine(ChangeTileAlpha());
    }

    private IEnumerator ChangeTileAlpha()
    {
        while (true)
        {
            yield return StartCoroutine(FadeAlpha(alphaMin, alphaMax, duration / 2)); // duration/2로 설정하여 전체 사이클이 1초가 되도록 합니다.
            yield return StartCoroutine(FadeAlpha(alphaMax, alphaMin, duration / 2));
        }
    }

    private IEnumerator FadeAlpha(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            SetTileAlpha(NoteTile[0], newAlpha);
            SetTileAlpha(NoteTile[1], newAlpha);
            yield return null;
        }
    }

    private void SetTileAlpha(GameObject tile, float alpha)
    {
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
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