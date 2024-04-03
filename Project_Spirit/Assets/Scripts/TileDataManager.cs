using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDataManager : MonoBehaviour
{
    public static TileDataManager instance = null;        
    public int[,] tileArray = new int[103, 103];

    // 각 타일의 정보를 담을 2차원 배열
   public Node[,] nodes;   
    public Sprite tileSprite;
    public int x;
    public int y;
    public Vector2Int position;
    public TileDataManager(Vector2Int _pos, Sprite _sprite) 
    {
        position = _pos; tileSprite = _sprite;
    }

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

    public void SetTileType(int x, int y, int type)
    {
        tileArray[x, y] = type;
    }

    public int GetTileType(int x, int y)
    {
        return tileArray[x, y];
    }
    public void SetTileSprite(Sprite _sprite)
    {
        tileSprite = _sprite;
    }
    public Sprite GetTileSprit()
    {
        return tileSprite;
    }
}
