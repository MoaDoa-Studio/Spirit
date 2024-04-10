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
    public GameObject buildingParent; // 생성될 빌딩의 부모 오브젝트.

    [Header("타일 맵")]
    public Tilemap gameTilemap;
    public Tilemap gridTilemap;
    public Tile[] stateTile; 

    [Header("기타")]
    public LayerMask placement_LayerMask;

    private GameObject mouseIndicator;
    private List<Vector3Int> roadBufferList = new List<Vector3Int>();
    private Tile selectedRoad;
        
    public bool IsPointerOverUI()
    => EventSystem.current.IsPointerOverGameObject();

    private const int underLimit = 0;
    private const int overLimit = 103;

    // For Debug.
    public GameObject building_Prefab;
    enum CraftMode
    {
        None,
        Default,
        PlaceBuilding,
        PlaceRoad,
    }

    CraftMode craftMode;
    private void Start()
    {
        craftMode = CraftMode.None;
        mouseIndicator = null;

        // For Debug. 
        TileDataManager.instance.SetTileType(0, 0, 1);
        TileDataManager.instance.SetTileType(15, 3, 2);
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
                break;
            case CraftMode.PlaceBuilding:                
                craftMenuUI.SetActive(false);
                break;
            case CraftMode.PlaceRoad:
                craftMenuUI.SetActive(false);
                break;
        }
    }
    public void Update()
    {
        switch (craftMode)
        {
            case CraftMode.PlaceBuilding:                
                mouseIndicator.transform.position = grid.CellToWorld(grid.WorldToCell(TrackMousePosition()));

                // 클릭 입력을 받으면 해당 좌표에 설치되도록.
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    Debug.Log(mouseIndicator.transform.position);
                    mouseIndicator = null;
                    ChangeCraftMode(CraftMode.Default);                    
                }
                break;
            case CraftMode.PlaceRoad:
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Vector3Int gridPos = grid.WorldToCell(TrackMousePosition());                     
                    if (!roadBufferList.Contains(gridPos))
                    {                        
                        // 중복 설치인지 체크.
                        if (isOverlapRoad(gridPos))
                        {
                            Debug.Log("중복 설치!!");
                            gridTilemap.SetTile(gridPos, stateTile[2]);
                        }
                        else                        
                            gridTilemap.SetTile(gridPos, stateTile[1]);
                        
                        SetRoadTile(gridPos);
                        roadBufferList.Add(gridPos);
                        if (roadBufferList.Count == 1)
                            return;
                        
                        Vector3Int prevRoad = roadBufferList[roadBufferList.Count - 2];                        
                        if ((prevRoad.x != gridPos.x) && (prevRoad.y != gridPos.y))                                                    
                            SetRoadTile(new Vector3Int(prevRoad.x, gridPos.y, 0));
                    }                                                         
                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    // ToDo. 길 건설 가능 판정 로직 실시.                     
                    if (roadBufferList.Count == 0)
                        return;

                    Debug.Log("길 건설 : " + CanPlaceRoad());

                    resetGridTile();
                    roadBufferList.Clear();
                    ChangeCraftMode(CraftMode.Default);
                }
                break;
        }        
    }

    // Craft 모드 진입.
    public void EnterCraftMode()
    {
        craftGrid.SetActive(true);
        craftMenuUI.SetActive(true);
    }

    private Vector3 TrackMousePosition()
    {
        // 마우스 좌표를 화면 상 좌표로 가공.
        Vector3 mousePos = Input.mousePosition;                
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);        
        return worldPos;
    }    

    // 건물 설치 관련.

    // 건물 선택 버튼에 부착할 함수.
    public void OnClickBuildingSelectButton(GameObject building)
    {                
        mouseIndicator = Instantiate(building, buildingParent.transform);

        // 선택한 건물 버튼 외 다른 버튼 흑백 처리 로직도 들어가야 함.
        ChangeCraftMode(CraftMode.PlaceBuilding);
    }

    // 선택한 건물의 정보를 데이터 셋에서 불러오는 함수.

    // 건물을 마우스 위치에 따라 다니도록 하는 함수.

    // 건물을 배치할 때 사용할 함수.     

    // 길 설치 관련.

    // 길 선택 버튼에 부착할 함수.
    public void OnClickRoadSelectButton(Tile tile)
    {
        selectedRoad = tile;
        
        ChangeCraftMode(CraftMode.PlaceRoad);
        roadBufferList.Clear();
        // 선택한 건물 버튼 외 다른 버튼 흑백 처리 로직도 들어가야 함.
        
    }

    void SetRoadTile(Vector3Int pos)
    {
        gameTilemap.SetTile(pos, selectedRoad);
        TileDataManager.instance.SetTileType(pos.x, pos.y, 3);
    }    
    
    bool CanPlaceRoad()
    {
        Vector2Int startPos = new Vector2Int(roadBufferList[0].x, roadBufferList[0].y);
        Vector2Int endPos = new Vector2Int(roadBufferList[^1].x, roadBufferList[^1].y);

        int[] dx = new int [4]{ 0, 0, -1, 1 };
        int[] dy = new int [4]{ 1, -1, 0, 0 };

        // 출발지 연결 체크.
        for(int i = 0; i < 4; i++)
        {
            int nx = startPos.x + dx[i];
            int ny = startPos.y + dy[i];
            if (nx < underLimit || nx > overLimit || ny < underLimit || nx > overLimit)
                continue;

            int nextTileType = TileDataManager.instance.GetTileType(nx, ny);
            if (nextTileType == 1 || nextTileType == 2)
            {
                return true;
            }            
        }
        // 도착지 연결 체크.
        for (int i = 0; i < 4; i++)
        {
            int nx = endPos.x + dx[i];
            int ny = endPos.y + dy[i];
            if (nx < underLimit || nx > overLimit || ny < underLimit || nx > overLimit)
                continue;

            int nextTileType = TileDataManager.instance.GetTileType(nx, ny);
            if (nextTileType == 1 || nextTileType == 2)
            {
                return true;
            }
        }
        return false;
    }

    bool isOverlapRoad(Vector3Int pos)
    {
        if (TileDataManager.instance.GetTileType(pos.x, pos.y) == 3)
            return true;
        return false;
    }

    void resetGridTile()
    {        
        for(int i = 0; i < 103; i++)
        {
            for(int j = 0; j < 103; j++)
            {
                gridTilemap.SetTile(new Vector3Int(i, j, 0), stateTile[0]);
            }
        }
    }
}