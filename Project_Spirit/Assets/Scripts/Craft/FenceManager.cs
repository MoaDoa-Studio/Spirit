using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FenceManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap FenceTileMap;
    [SerializeField]
    private TileBase[] Fence;
    
    int[,] copyArray;
    const int width = 103;
    const int height = 103;
    public void UpdateFence()
    {        
        for (int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                if (TileDataManager.instance.GetTileType(i, j) != 3)
                    continue;
                SetFenceTile(i, j);
            }
        }        
    }

    void SetFenceTile(int x, int y)
    {
        // 상하좌우 순으로 길이 있는 지 저장.
        bool[] isNearbyWay = { false, false, false, false };
        isNearbyWay[0] = TileDataManager.instance.GetTileType(x, y + 1) == 3 ? true : false;
        isNearbyWay[1] = TileDataManager.instance.GetTileType(x, y - 1) == 3 ? true : false;
        isNearbyWay[2] = TileDataManager.instance.GetTileType(x - 1, y) == 3 ? true : false;
        isNearbyWay[3] = TileDataManager.instance.GetTileType(x + 1, y) == 3 ? true : false;

        // 총 16가지 경우의 수.
        int NearbyWayCount = 0;
        foreach (bool temp in isNearbyWay)
        {
            if (temp == true)
                NearbyWayCount++;
        }

        // 해당 칸 초기화.
        Vector3Int pos = new Vector3Int(x, y, 0);
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
        FenceTileMap.SetTransformMatrix(pos, Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one));
        switch (NearbyWayCount)
        {
            case 1:
                FenceTileMap.SetTile(pos, Fence[3]);
                if (isNearbyWay[0])
                    matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 270f), Vector3.one);
                else if (isNearbyWay[1])
                    matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
                else if (isNearbyWay[3])
                    matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 180f), Vector3.one);
                break;
            case 2:
                if (isNearbyWay[0] && isNearbyWay[1])
                {
                    FenceTileMap.SetTile(pos, Fence[2]);
                    matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
                }
                else if (isNearbyWay[2] && isNearbyWay[3])                
                    FenceTileMap.SetTile(pos, Fence[2]);                                    
                else
                {
                    FenceTileMap.SetTile(pos, Fence[1]);
                    if (isNearbyWay[1] && isNearbyWay[3])
                        matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 180f), Vector3.one);
                    else if (isNearbyWay[1] && isNearbyWay[2])
                        matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
                    else if (isNearbyWay[0] && isNearbyWay[3])
                        matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 270f), Vector3.one);
                }
                    break;
            case 3:
                FenceTileMap.SetTile(pos, Fence[0]);
                if (!isNearbyWay[0])
                    matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 180f), Vector3.one);
                else if (!isNearbyWay[2])
                    matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 270f), Vector3.one);                
                else if (!isNearbyWay[3])
                    matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 0f, 90f), Vector3.one);
                break;
            case 4:
                FenceTileMap.SetTile(pos, null);
                break;
        }
        FenceTileMap.SetTransformMatrix(pos, matrix * FenceTileMap.GetTransformMatrix(pos));
    }    
}
