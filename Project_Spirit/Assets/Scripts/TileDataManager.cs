using System.Collections;
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
    
    public int x;
    public int y;
    public int sizeX, sizeY;

    enum TileType
    {
        None = 0,
        Building = 1, // 건물
        Cradle = 2, // 요람
        Road = 3, // 도로
        Resource = 4, // 자원
        Mark = 5, // 표식
    }
    
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

    }

    private void Start()
    {
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;

        InstantiateTile();
        CheckTileSprite();
    }

    public void SetTileType(int x, int y, int type)
    {
        tileArray[x, y] = type;
    }

    public int GetTileType(int x, int y)
    {
        return tileArray[x, y];
    }
    public Node[,] GetNodes() { return nodes; }

    private void InstantiateTile()
    {
        nodes = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                nodes[i, j] = new Node(i,j);
                nodes[i,j].x = i;
                nodes[i,j].y = j;
            }
        }
      
    }

    private void CheckTileSprite()
    {
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                Vector3Int tilePosition = new Vector3Int(i, j);
                TileBase tile = tilemap.GetTile(tilePosition);

                if (tile != null)
                {
                    // 가져온 타일의 스프라이트를 확인한다.
                    Sprite tileSprite = (tile as Tile).sprite;
                    if (tileSprite == targetSprite[0])
                    {
                        nodes[i, j].nodeSprite = tileSprite;
                        //Debug.Log(nodes[i, j].nodeSprite);
                    }
                }
            }
        }
    }
}
