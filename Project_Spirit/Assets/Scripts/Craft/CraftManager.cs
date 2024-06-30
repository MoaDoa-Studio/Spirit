using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using TMPro;
public partial class CraftManager : MonoBehaviour
{
    [Header("건축 모드")]
    public Grid grid; // 그리드.    
    public GameObject craftGrid; // 건축 모드 시 격자 표시.
    public GameObject craftMenuUI; // 하단 빌딩 선택 UI
    public Transform BuildingSlot; // 생성될 빌딩의 부모 오브젝트.
    public Transform SignSlot; // 생성될 사인의 부모 오브젝트.
    public BuildingDatabaseSO database;

    [Header("타일 맵")]
    public Tilemap GameTilemap;
    public Tilemap GridTilemap;
    public Tile defaultTile, greenTile, orangeTile, redTile;

    private GameObject mouseIndicator;
    private Tile selectedRoad;
    private Tile selectedSign;
    Node[,] nodes;
    private int[,] copyArray = new int[103, 103];
    private List<Vector3Int> roadBufferList = new List<Vector3Int>();
    private Vector3Int signBuffer = new Vector3Int();
    private Vector3Int deleteStart;

    public bool IsPointerOverUI()
    => EventSystem.current.IsPointerOverGameObject();
        
    enum CraftMode
    {
        None,
        Default,
        PlaceBuilding,
        PlaceRoad,
        PlaceSign,
        DeleteBuilding,
        DeleteRoad,
        DeleteSign,
    }

    CraftMode craftMode;
    private void Start()
    {
        craftMode = CraftMode.None;
        mouseIndicator = null; 
        deleteStart = Vector3Int.back;        
    }

    void ChangeCraftMode(CraftMode mode)
    {
        craftMode = mode;
        switch (mode)
        {
            case CraftMode.None:
                break;
            case CraftMode.Default:
                craftGrid.SetActive(true);
                craftMenuUI.SetActive(true);
                UpdateFieldStatus(); // 필드 갱신. 건물 도로 등등.
                break;
            case CraftMode.PlaceBuilding:
            case CraftMode.DeleteBuilding:                
            case CraftMode.PlaceRoad:
            case CraftMode.DeleteRoad:
            case CraftMode.PlaceSign:
            case CraftMode.DeleteSign:
                craftMenuUI.SetActive(false);
                break;                
        }
    }
    public void Update()
    {
        Vector3Int mousePos = grid.WorldToCell(ProcessingMousePosition());
        switch (craftMode)
        {
            case CraftMode.PlaceBuilding:                
                mouseIndicator.transform.position = mousePos;
                PlaceBuildingBuffer();
                if (Input.GetKeyDown(KeyCode.Mouse0))                 
                    PlaceBuilding();
                if (Input.GetKeyDown(KeyCode.Q))
                    RotateObject(true);
                if (Input.GetKeyDown(KeyCode.E))
                    RotateObject(false);
                break;

            case CraftMode.PlaceRoad:                
                if (Input.GetKey(KeyCode.Mouse0))                
                    PlaceRoadTileBuffer(mousePos);
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (roadBufferList.Count == 0)                    
                        return;                    
                    PlaceRoadTile();
                }
                break;
            
            case CraftMode.PlaceSign:                
                mouseIndicator.transform.position = mousePos;
                if (signBuffer != mousePos)
                {
                    signBuffer = mousePos;
                    ResetGridTile();
                    PlaceSignTileBuffer(signBuffer);
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    PlaceSignTile(mousePos);
                if (Input.GetKeyDown(KeyCode.Q))
                    RotateObject(true);
                if (Input.GetKeyDown(KeyCode.E))
                    RotateObject(false);
                break;
            
            case CraftMode.DeleteBuilding:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    deleteStart = mousePos;
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (deleteStart == Vector3Int.back)
                        return;
                    DeleteBuilding();
                }
                break;
            
            case CraftMode.DeleteRoad:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    deleteStart = mousePos;
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (deleteStart == Vector3Int.back)
                        return;
                    DeleteRoad();
                }
                break;

            case CraftMode.DeleteSign:
                if (Input.GetKeyDown(KeyCode.Mouse0))
                    deleteStart = mousePos;
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    if (deleteStart == Vector3Int.back)
                        return;
                    DeleteSign();
                }
                break;
        }

        // For Debug.
        if (Input.GetKeyDown(KeyCode.W))
        {
            ExitCraftMode();                        
        }
    }
    // Craft 모드 진입.
    public void EnterCraftMode()
    {
        craftGrid.SetActive(true);
        craftMenuUI.SetActive(true);
    }
    public void ExitCraftMode()
    {
        craftMode = CraftMode.None;
        mouseIndicator = null;
        deleteStart = Vector3Int.back;
        
        craftGrid.SetActive(false);
        craftMenuUI.SetActive(false);        
    }
    public void EnterDeleteBuildingMode()
    {
        ChangeCraftMode(CraftMode.DeleteBuilding);
    }
    public void EnterDeleteRoadMode()
    {
        ChangeCraftMode(CraftMode.DeleteRoad);
    }
    private Vector3 ProcessingMousePosition()
    {
        // 마우스 좌표를 월드 좌표로 가공.
        Vector3 mousePos = Input.mousePosition;                
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);        
        return worldPos;
    }    
    public void ResetGridTile()
    {        
        for(int i = 0; i < 103; i++)        
            for(int j = 0; j < 103; j++)            
                GridTilemap.SetTile(new Vector3Int(i, j, 0), defaultTile);            
    }
}

partial class CraftManager
{
    #region 건물 배치 관련
    public void OnClickBuildingSelectButton(GameObject building)
    {
        mouseIndicator = Instantiate(building, BuildingSlot);        
        ChangeCraftMode(CraftMode.PlaceBuilding);
        // 선택한 건물 버튼 외 다른 버튼 흑백 처리 로직도 들어가야 함.        
    }
    public void PlaceBuildingBuffer()
    {
        ResetGridTile();

        Vector2Int upperRight = new Vector2Int(Mathf.RoundToInt(mouseIndicator.transform.position.x), Mathf.RoundToInt(mouseIndicator.transform.position.y));
        var angles = mouseIndicator.transform.GetChild(0).rotation.eulerAngles;
        int x = 0, y = 0;
        if (angles.z % 180 == 0)
        {
            x = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.x;
            y = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.y;
        }
        else
        {
            x = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.y;
            y = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.x;
        }
        Vector2Int bottomLeft = new Vector2Int(upperRight.x - x + 1, upperRight.y - y + 1);

        for (int i = upperRight.y; i >= bottomLeft.y; i--)
        {
            for (int j = upperRight.x; j >= bottomLeft.x; j--)
            {
                if (TileDataManager.instance.GetTileType(j, i) == 1)                
                    GridTilemap.SetTile(new Vector3Int(j, i, 0), redTile);
                else
                    GridTilemap.SetTile(new Vector3Int(j, i, 0), greenTile);                
            }
        }        
    }
    public void PlaceBuilding()
    {        
        var angles = mouseIndicator.transform.GetChild(0).rotation.eulerAngles;
        int x = 0, y = 0;
        if (angles.z % 180 == 0)
        {
            x = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.x;
            y = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.y;
        }
        else
        {
            x = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.y;
            y = (int)mouseIndicator.transform.GetComponent<BoxCollider2D>().size.x;
        }
        
        Vector2Int upperRight = new Vector2Int(Mathf.RoundToInt(mouseIndicator.transform.position.x), Mathf.RoundToInt(mouseIndicator.transform.position.y));
        Vector2Int bottomLeft = new Vector2Int(upperRight.x - x + 1, upperRight.y - y + 1);
        ResetGridTile();

        if (isBuildingOvelapBuilding(upperRight, bottomLeft))
        {            
            Destroy(mouseIndicator);
            ChangeCraftMode(CraftMode.Default);
            return;
        }

        for (int i = upperRight.y; i >= bottomLeft.y; i--)        
            for (int j = upperRight.x; j >= bottomLeft.x; j--)            
                TileDataManager.instance.SetTileType(j, i, 1);
        
        mouseIndicator.GetComponent<Building>().SetBuildingPos(upperRight, bottomLeft);
        BuildingDataManager.instance.AddBuilding(mouseIndicator.GetComponent<Building>());        
        
        mouseIndicator = null;        
        ChangeCraftMode(CraftMode.Default);        
    }
    bool isBuildingOvelapBuilding(Vector2Int upperRight, Vector2Int bottomLeft)
    {
        for (int i = upperRight.y; i >= bottomLeft.y; i--)
        {
            for (int j = upperRight.x; j >= bottomLeft.x; j--)
            {                
                if (TileDataManager.instance.GetTileType(j, i) == 1)
                {
                    Debug.Log("겹치는 건물 있다");
                    return true;
                }
            }
        }
        return false;
    }
    public void RotateObject(bool isRight)
    {
        var angles = mouseIndicator.transform.GetChild(0).rotation.eulerAngles;
        angles.z += isRight == true ? 90 : -90;

        if (angles.z >= 360)
            angles.z -= 360;
        else if (angles.z < 0)
            angles.z += 360;
        mouseIndicator.transform.GetChild(0).rotation = Quaternion.Euler(angles);
        
        float x = 0, y = 0;
        if (angles.z % 180 == 0)
        {
            x = mouseIndicator.transform.GetComponent<BoxCollider2D>().size.x;
            y = mouseIndicator.transform.GetComponent<BoxCollider2D>().size.y;
        }
        else
        {
            x = mouseIndicator.transform.GetComponent<BoxCollider2D>().size.y;
            y = mouseIndicator.transform.GetComponent<BoxCollider2D>().size.x;
        }
        Vector2 mouseIndicatorPos = new Vector2(-(x / 2 - 1), -(y / 2 - 1));
        mouseIndicator.transform.GetChild(0).localPosition = mouseIndicatorPos;
        mouseIndicator.transform.GetChild(0).rotation = Quaternion.Euler(angles);                       
    }
    #endregion

    #region 건물 삭제 관련
    void DeleteBuilding()
    {
        Vector3Int deleteEnd = grid.WorldToCell(ProcessingMousePosition());
        Vector2Int deleteUpperRight = new Vector2Int();
        Vector2Int deleteBottomLeft = new Vector2Int();
        deleteUpperRight.x = deleteStart.x > deleteEnd.x ? deleteStart.x : deleteEnd.x;
        deleteUpperRight.y = deleteStart.y > deleteEnd.y ? deleteStart.y : deleteEnd.y;
        deleteBottomLeft.x = deleteStart.x < deleteEnd.x ? deleteStart.x : deleteEnd.x;
        deleteBottomLeft.y = deleteStart.y < deleteEnd.y ? deleteStart.y : deleteEnd.y;
        
        Queue<Building> DeleteBuildingQueue = FindBuildingToBeDeletedByRange(deleteUpperRight, deleteBottomLeft);
        while (DeleteBuildingQueue.Count != 0)
        {
            Building building = DeleteBuildingQueue.Dequeue();
            BuildingDataManager.instance.DestroyObjectByScript(building);
        }

        deleteStart = Vector3Int.back;
        ChangeCraftMode(CraftMode.Default);
    }

    Queue<Building> FindBuildingToBeDeletedByRange(Vector2Int deleteUpperRight, Vector2Int deleteBottomLeft)
    {        
        Queue<Building> result = new Queue<Building>();
        foreach (Building building in BuildingDataManager.instance.GetBuildingList())
        {
            Tuple<Vector2Int, Vector2Int> buildingPos = building.GetBuildingPos();
            Vector2Int upperRight = buildingPos.Item1;
            Vector2Int bottomLeft = buildingPos.Item2;
            if (upperRight.x <= deleteUpperRight.x && upperRight.y <= deleteUpperRight.y
                && bottomLeft.x >= deleteBottomLeft.x && bottomLeft.y >= deleteBottomLeft.y)
            {                
                TileDataManager.instance.ChangeTileTypeByRange(upperRight, bottomLeft, 1, 0); // 타일 타입 초기화.
                result.Enqueue(building);
            }
        }
        return result;
    }
    #endregion
}
partial class CraftManager
{
    #region 길 배치 관련
    public void OnClickRoadSelectButton(Tile tile)
    {
        selectedRoad = tile;
        CopyTileArray();
        ChangeCraftMode(CraftMode.PlaceRoad);

        // Todo. 선택한 건물 버튼 외 다른 버튼 흑백 처리 로직도 들어가야 함.
    }
    bool isOverlapRoad(Vector3Int pos)
    {
        if (TileDataManager.instance.GetTileType(pos.x, pos.y) == 3)
            return true;
        return false;
    }
    bool isOverlapBuilding(Vector3Int pos)
    {
        if (TileDataManager.instance.GetTileType(pos.x, pos.y) == 1)
            return true;
        return false;
    }
    bool isOverlapResource(Vector3Int pos)
    {
        int type = TileDataManager.instance.GetTileType(pos.x, pos.y);
        if (type == 6 || type == 7)
            return true;
        return false;
    }
    bool canPlaceRoad()
    {
        foreach (Vector3Int roadBuffer in roadBufferList)
        {
            if (TileDataManager.instance.GetTileType(roadBuffer.x, roadBuffer.y) == 1)
            {
                Debug.Log("건물과 도로가 겹칩니다.");
                return false;
            }
        }
        Debug.Log("설치 가능");
        return true;
    }
    public void PlaceRoadTileBuffer(Vector3Int pos)
    {        
        // 배치 가능한 타일인지 체크.
        if (!roadBufferList.Contains(pos))
        {            
            if (isOverlapBuilding(pos) || isOverlapResource(pos))
            {
                GridTilemap.SetTile(pos, redTile);
                return;
            }
            else if (isOverlapRoad(pos))
                GridTilemap.SetTile(pos, orangeTile);
            else
                GridTilemap.SetTile(pos, greenTile);

            GameTilemap.SetTile(pos, selectedRoad);
            if (TileDataManager.instance.isRange(pos.x, pos.y))
            {
                copyArray[pos.x, pos.y] = 3;
                roadBufferList.Add(pos);
            }
        }
    }
    public void PlaceRoadTile()
    {        
        if (canPlaceRoad())        
            PasteTileArray();        
        else
        {
            foreach (Vector3Int roadBuffer in roadBufferList)
            {
                if (TileDataManager.instance.GetTileType(roadBuffer.x, roadBuffer.y) != 3)                    
                    GameTilemap.SetTile(roadBuffer, null);
            }
        }
        ResetGridTile();
        roadBufferList.Clear();
        ChangeCraftMode(CraftMode.Default);
    }
    #endregion
    
    #region 길 삭제 관련
    void DeleteRoad()
    {
        Vector3Int deleteEnd = grid.WorldToCell(ProcessingMousePosition());
        Vector2Int deleteUpperRight = new Vector2Int();
        Vector2Int deleteBottomLeft = new Vector2Int();
        deleteUpperRight.x = deleteStart.x > deleteEnd.x ? deleteStart.x : deleteEnd.x;
        deleteUpperRight.y = deleteStart.y > deleteEnd.y ? deleteStart.y : deleteEnd.y;
        deleteBottomLeft.x = deleteStart.x < deleteEnd.x ? deleteStart.x : deleteEnd.x;
        deleteBottomLeft.y = deleteStart.y < deleteEnd.y ? deleteStart.y : deleteEnd.y;

        TileDataManager.instance.ChangeTileTypeByRange(deleteUpperRight, deleteBottomLeft, 3, 0); // 타일 타입이 3인 경우에만 초기화.
        // 타일 맵 초기화.
        for (int i = deleteBottomLeft.y; i <= deleteUpperRight.y; i++)        
            for (int j = deleteBottomLeft.x; j <= deleteUpperRight.x; j++)            
                GameTilemap.SetTile(new Vector3Int(j, i, 0), null);         

        deleteStart = Vector3Int.back;
        ChangeCraftMode(CraftMode.Default);
    }
    #endregion
}
partial class CraftManager
{
    #region 표식 배치 관련
    public void OnClickSignSelectButton(GameObject sign)
    {
        mouseIndicator = Instantiate(sign, SignSlot);
        ChangeCraftMode(CraftMode.PlaceSign);
    }
    public int GetNumberOfNearbyRoad(Vector3Int pos)
    {
        int result = 0;
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            if (TileDataManager.instance.GetTileType(pos.x + dx[i], pos.y + dy[i]) == 3)
                result++;
        }
        return result;
    }
    public bool CanPlaceSignTile(Vector3Int pos)
    {
        if (!isOverlapRoad(pos))
        {
            Debug.Log("길이 아닙니다.");
            return false;
        }

        string TileName = mouseIndicator.name.Split("(")[0];        
        switch (TileName)
        {
            case "Sign_0":
            case "Sign_7":
                break;
            default:
                if (GetNumberOfNearbyRoad(pos) < 3)
                {
                    Debug.Log("주변의 길 개수가 3개 미만입니다.");
                    return false;
                }
                break;
        }
        return true;
    }
    public void PlaceSignTileBuffer(Vector3Int pos)
    {
        if (CanPlaceSignTile(pos))
            GridTilemap.SetTile(pos, greenTile);
        else
            GridTilemap.SetTile(pos, redTile);
    }
    public void PlaceSignTile(Vector3Int pos)
    {
        if (CanPlaceSignTile(pos))                    
            TileDataManager.instance.SetTileType(pos.x, pos.y, 5);        
        else        
            Destroy(mouseIndicator.gameObject);
        
        mouseIndicator = null;
        ResetGridTile();
        ChangeCraftMode(CraftMode.Default);
    }
    #endregion

    #region 표식 삭제 관련
    void DeleteSign()
    {
        Vector3Int deleteEnd = grid.WorldToCell(ProcessingMousePosition());
        Vector2Int deleteUpperRight = new Vector2Int();
        Vector2Int deleteBottomLeft = new Vector2Int();
        deleteUpperRight.x = deleteStart.x > deleteEnd.x ? deleteStart.x : deleteEnd.x;
        deleteUpperRight.y = deleteStart.y > deleteEnd.y ? deleteStart.y : deleteEnd.y;
        deleteBottomLeft.x = deleteStart.x < deleteEnd.x ? deleteStart.x : deleteEnd.x;
        deleteBottomLeft.y = deleteStart.y < deleteEnd.y ? deleteStart.y : deleteEnd.y;

        Queue<GameObject> DeleteSignQueue = FindSignToBeDeletedByRange(deleteUpperRight, deleteBottomLeft);
        while (DeleteSignQueue.Count != 0)
        {
            GameObject sign = DeleteSignQueue.Dequeue();
            Destroy(sign);
        }

        deleteStart = Vector3Int.back;
        ChangeCraftMode(CraftMode.Default);
    }

    Queue<GameObject> FindSignToBeDeletedByRange(Vector2Int deleteUpperRight, Vector2Int deleteBottomLeft)
    {
        Queue<GameObject> result = new Queue<GameObject>();        
        for(int i = 0; i < SignSlot.transform.childCount; i++)
        {
            Vector3 signPos = SignSlot.transform.GetChild(i).position;            
            if (signPos.x <= deleteUpperRight.x && signPos.y <= deleteUpperRight.y
                && signPos.x >= deleteBottomLeft.x && signPos.y >= deleteBottomLeft.y)
            {                
                TileDataManager.instance.SetTileType((int)signPos.x, (int)signPos.y, 0);
            }
        }
        return result;
    }
    #endregion
}
partial class CraftManager
{    
    void UpdateFieldStatus()
    {
        CopyTileArray();
        foreach (Building building in BuildingDataManager.instance.GetBuildingList())
        {
            Tuple<Vector2Int, Vector2Int> buildingPos = building.GetBuildingPos();
            Tuple<Vector2Int, Vector2Int> TwoRoads = isTwoRoadsAttackedBuilding(buildingPos.Item1, buildingPos.Item2);
            building.SetConnectedRoad(TwoRoads);
            TileDataManager.instance.CheckBuildings();
        }
    }
    Tuple<Vector2Int, Vector2Int> isTwoRoadsAttackedBuilding(Vector2Int upperRight, Vector2Int bottomLeft)
    {
        List<Vector2Int> result = new List<Vector2Int>();

        // 건물이 1칸인 경우.
        if (upperRight == bottomLeft)
        {
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { 1, -1, 0, 0 };
            for(int i = 0; i < 4; i++)
            {
                if (copyArray[upperRight.x + dx[i], upperRight.y + dy[i]] == 3 || nodes[upperRight.x + dx[i], upperRight.y + dy[i]].isWalk)
                    result.Add(new Vector2Int(upperRight.x + dx[i], upperRight.y + dy[i]));
            }
        }
        else
        {
            // 우상단
            if (copyArray[upperRight.x, upperRight.y + 1] == 3) result.Add(new Vector2Int(upperRight.x, upperRight.y + 1));
            if (copyArray[upperRight.x + 1, upperRight.y] == 3) result.Add(new Vector2Int(upperRight.x + 1, upperRight.y));

            // 우하단
            if (copyArray[upperRight.x + 1, bottomLeft.y] == 3) result.Add(new Vector2Int(upperRight.x + 1, bottomLeft.y));
            if (copyArray[upperRight.x, bottomLeft.y - 1] == 3) result.Add(new Vector2Int(upperRight.x, bottomLeft.y - 1));

            // 좌상단
            if (copyArray[bottomLeft.x, upperRight.y + 1] == 3) result.Add(new Vector2Int(bottomLeft.x, upperRight.y + 1));
            if (copyArray[bottomLeft.x - 1, upperRight.y] == 3) result.Add(new Vector2Int(bottomLeft.x - 1, upperRight.y));

            // 좌하단
            if (copyArray[bottomLeft.x - 1, bottomLeft.y] == 3) result.Add(new Vector2Int(bottomLeft.x - 1, bottomLeft.y));
            if (copyArray[bottomLeft.x, bottomLeft.y - 1] == 3) result.Add(new Vector2Int(bottomLeft.x, bottomLeft.y - 1));

            // 모서리
            for (int i = upperRight.x - 1; i > bottomLeft.x; i--)
            {
                if (copyArray[i, upperRight.y + 1] == 3) result.Add(new Vector2Int(i, upperRight.y + 1));
                if (copyArray[i, bottomLeft.y - 1] == 3) result.Add(new Vector2Int(i, bottomLeft.y - 1));
            }

            for (int i = upperRight.y - 1; i > bottomLeft.y; i--)
            {
                if (copyArray[upperRight.x + 1, i] == 3) result.Add(new Vector2Int(upperRight.x + 1, i));
                if (copyArray[bottomLeft.x - 1, i] == 3) result.Add(new Vector2Int(bottomLeft.x - 1, i));
            }
        }
        
        if (result.Count == 2)
            return new Tuple<Vector2Int, Vector2Int>(result[0], result[1]);
        return null;
    }
    void CopyTileArray()
    {
        for (int i = 0; i < 103; i++)
        {
            for (int j = 0; j < 103; j++)
            {
                copyArray[j, i] = TileDataManager.instance.GetTileType(j, i);
            }
        }
    }
    void PasteTileArray()
    {
        for (int i = 0; i < 103; i++)
        {
            for (int j = 0; j < 103; j++)
            {
                TileDataManager.instance.SetTileType(j, i, copyArray[j, i]);
            }
        }
    }
}