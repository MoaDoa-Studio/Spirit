using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;
using TMPro;
public partial class CraftManager : MonoBehaviour
{
    // 건물 변수 저장.
    [SerializeField]
    private Grid grid; // 그리드.
    [SerializeField]
    private GameObject craftGrid; // 건축 모드 시 격자 표시.
    [SerializeField]
    private GameObject craftMenuUI; // 하단 빌딩 선택 UI
    [SerializeField]
    private GameObject buildingParent; // 생성될 빌딩의 부모 오브젝트.
    [SerializeField]
    private Tilemap gameTilemap;    

    private GameObject mouseIndicator;
    private List<Vector3Int> roadBufferList = new List<Vector3Int>();
    private Tile selectedRoad;
    
    // For Debug.
    public GameObject building_Prefab;
    public LayerMask placement_LayerMask;

    
    enum CraftMode
    {
        None,
        Craft,
        PlaceBuilding,
        PlaceRoad,
    }

    CraftMode craftMode;
    private void Start()
    {
        craftMode = CraftMode.None;
        mouseIndicator = null;
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
                    mouseIndicator = null;
                    craftMode = CraftMode.Craft;
                }
                break;
            case CraftMode.PlaceRoad:
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Vector3Int gridPos = grid.WorldToCell(TrackMousePosition());                     
                    if (!roadBufferList.Contains(gridPos))
                    {                        
                        SetRoadTile(gridPos);
                        roadBufferList.Add(gridPos);
                        if (roadBufferList.Count == 1)
                            return;

                        // 대각선 중간에 타일 비지 않도록.
                        Vector3Int prevRoad = roadBufferList[roadBufferList.Count - 2];                        
                        if ((prevRoad.x != gridPos.x) && (prevRoad.y != gridPos.y))                                                    
                            SetRoadTile(new Vector3Int(prevRoad.x, gridPos.y, 0));
                    }                                                         
                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    roadBufferList.Clear();

                    // ToDo. 길 건설 가능 판정 로직 실시.                     
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
        // 마우스 따라다닐 건물 정보 불러와서 마우스에 부착.
        mouseIndicator = Instantiate(building, buildingParent.transform); // Todo. transform과 위치 정보 매개변수 추가.

        // 선택한 건물 버튼 외 다른 버튼 흑백 처리 로직도 들어가야 함.

        craftMode = CraftMode.PlaceBuilding;
    }

    // 선택한 건물의 정보를 데이터 셋에서 불러오는 함수.

    // 건물을 마우스 위치에 따라 다니도록 하는 함수.

    // 건물을 배치할 때 사용할 함수.     

    // 길 설치 관련.

    // 길 선택 버튼에 부착할 함수.
    public void OnClickRoadSelectButton(Tile tile)
    {
        selectedRoad = tile;

        craftMode = CraftMode.PlaceRoad;
        roadBufferList.Clear();
        // 선택한 건물 버튼 외 다른 버튼 흑백 처리 로직도 들어가야 함.
        
    }

    void SetRoadTile(Vector3Int pos)
    {
        gameTilemap.SetTile(pos, selectedRoad);
        TileDataManager.instance.SetTileType(pos.x, pos.y, 3);
    }    
    
    // 마우스를 떼는 순간, 건설이 가능하게 길을 깔았을 경우, 건축 모드 해제.
    // 건설 가능 여부 판정 로직도 필요.    
}