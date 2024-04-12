using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
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
   
    public List<Tuple<int[,], int[,]>> AttachedRoad { get; set; }
   
    public int sizeX, sizeY;

    enum TileType
    {
        None = 0,
        Building = 1, 
        Cradle = 2, 
        Road = 3,
        Resource = 4,
        Mark = 5,
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
                    Quaternion tileRotation = tilemap.GetTransformMatrix(tilePosition).rotation;
                    nodes[i, j].nodeSprite = tileSprite;
                    nodes[i, j].rotation = tileRotation;
                    nodes[i, j].isWalk = true;
                    if (tileSprite == targetSprite[1] || tileSprite == targetSprite[2] || tileSprite == targetSprite[3] || tileSprite == targetSprite[4] || tileSprite == targetSprite[5] || tileSprite == targetSprite[6] || tileSprite == targetSprite[7] || tileSprite == targetSprite[8])
                    {
                        nodes[i, j].isSignal = true;
                    }
                }
            }
        }
    }

}
  /*  static bool IsPositive(int[] array)
    {
        int left = 0;
        int right = array.Length - 1;

        if(right < 0)
            return false;

        while(left <= right)
        {
            int mid = (right - left ) / 2;

            if (array[mid] > 0)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }
        return left == array.Length;
    }    
}
  */