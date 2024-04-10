using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileDataManager : MonoBehaviour
{
    public static TileDataManager instance = null;        
    public int[,] tileArray = new int[103, 103];

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
        if (!isRange(x, y))
            return;
        tileArray[x, y] = type;
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
}
