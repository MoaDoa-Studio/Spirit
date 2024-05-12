using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDataManager : MonoBehaviour
{
    public static TileDataManager instance = null;
    public int[,] tileArray = new int[103, 103];
    [SerializeField]
    Tilemap tilemap;
    public Node[,] nodes;
    public Sprite[] targetSprite;
    public Vector2Int position;
    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    List<Building> buildingList;

    public int sizeX, sizeY;

    enum TileType
    {
        None = 0,
        Building = 1,
        Cradle = 2,
        Road = 3,
        Resource = 4,
        Mark = 5,
        Rock = 6,
        Wood = 7,
        Elemental_Essence = 8
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            sizeX = topRight.x - bottomLeft.x + 1;
            sizeY = topRight.y - bottomLeft.y + 1;
            InstantiateTile();

        }
        else
            Destroy(this);
    }

    private void Start()
    {
        CheckEveryTile();
    }

    public void Update()
    {
        CheckEveryTile();
    }

    public void SetTileType(int x, int y, int type)
    {
        if (!isRange(x, y))
            return;
        tileArray[x, y] = type;
    }

    public void SetTileTypeByRange(Vector2Int upperRight, Vector2Int bottomLeft, int type)
    {
        for (int i = bottomLeft.y; i <= upperRight.y; i++)
        {
            for (int j = bottomLeft.x; j <= upperRight.x; j++)
            {
                if (!isRange(j, i))
                    continue;
                tileArray[j, i] = type;
            }
        }
    }

    public void ChangeTileTypeByRange(Vector2Int upperRight, Vector2Int bottomLeft, int prevType, int type)
    {
        for (int i = bottomLeft.y; i <= upperRight.y; i++)
        {
            for (int j = bottomLeft.x; j <= upperRight.x; j++)
            {
                if (!isRange(j, i))
                    continue;

                if (tileArray[j, i] == prevType)
                    tileArray[j, i] = type;
            }
        }
    }
    public int GetTileType(int x, int y)
    {
        if (!isRange(x, y))
            return -1;
        return tileArray[x, y];
    }

    public bool isRange(int x, int y)
    {
        return x >= 0 && x < 103 && y >= 0 && y < 103;
    }

    public Node[,] GetNodes() { return nodes; }

    private void InstantiateTile()
    {
        nodes = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                nodes[i, j] = new Node(i, j);
                nodes[i, j].x = i;
                nodes[i, j].y = j;
            }
        }

    }
    #region 정령 타일 초기화
    private void CheckEveryTile()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                Vector3Int tilePosition = new Vector3Int(i, j);
                TileBase tile = tilemap.GetTile(tilePosition);

                if (tile != null)
                {
                    Sprite tileSprite = (tile as Tile).sprite;
                    // 빌딩 혹은 자원 아니면
                    if (GetTileType(i, j) != 6 || GetTileType(i, j) != 7)
                    {
                        Quaternion tileRotation = tilemap.GetTransformMatrix(tilePosition).rotation;
                        nodes[i, j].rotation = tileRotation;
                        //SetTileType(i, j, 3); // 일단 걸을 수 있다!로 다 해놓으셈 타일있으면 => Craftmanager

                    }
                    else
                        //SetTileType(i, j, 4);
                   
                    nodes[i, j].nodeSprite = tileSprite;
                    nodes[i, j].isWalk = true;
                    
                    if (tileSprite == targetSprite[1] || tileSprite == targetSprite[2] || tileSprite == targetSprite[3] || tileSprite == targetSprite[4] || tileSprite == targetSprite[5] || tileSprite == targetSprite[6] || tileSprite == targetSprite[7] || tileSprite == targetSprite[8])
                    {
                        nodes[i, j].SetNodeType(5);
                        nodes[i, j].isSignal = true;
                    }
                }
            }
        }
    }
    #endregion
    #region 빌딩 - 정령타일 동기화
    public void CheckBuildings()
    {
        buildingList = BuildingDataManager.instance.GetBuildingList();
        if (buildingList != null)
        {
            for (int i = 0; i < buildingList.Count; i++)
            {
                if (buildingList[i].GetConnectedRoad() != null)
                {
                    for (int j = buildingList[i].upperRight.x; j >= buildingList[i].bottomLeft.x; j--)
                    {
                        for (int k = buildingList[i].upperRight.y; k >= buildingList[i].bottomLeft.y; k--)
                        {
                            nodes[j, k].building = buildingList[i];
                            nodes[j, k].isWalk = true;
                            nodes[j, k].isBuild = true;
                            nodes[j, k].SetNodeType(1);
                            SetTileType(j, k, 3);
                            CheckEveryTile();
                        }
                    }
                }
            }
        }
    }
    #endregion
}
