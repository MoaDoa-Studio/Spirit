using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectMove : MonoBehaviour
{
    TileDataManager[,] tileArray;
    TileDataManager StartNode, TargetNode, CurNode;
    List<TileDataManager> OpenList, ClosedList;

    public Sprite[] sprites;
    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    int sizeX = 20, sizeY = 20;


    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    private void Start()
    {
        tileArray = new TileDataManager[sizeX, sizeY];

        // tileArray의 각 요소를 초기화합니다.
        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
               // tileArray[i, j] = new TileDataManager(i,j,null); // TileDataManager의 기본 생성자를 호출하여 객체를 초기화합니다.
            }
        }

        //check();
    }
    public void check()
    {   
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;

        StartNode = tileArray[startPos.x - bottomLeft.x, startPos.y - bottomLeft.y];
        TargetNode = tileArray[targetPos.x - bottomLeft.x, targetPos.y - bottomLeft.y];

        OpenList = new List<TileDataManager> { StartNode };
        ClosedList = new List<TileDataManager>();
        CurNode = OpenList[0];

        // 바라보는 방향 기준으로 좌표 변화를 나타냄.  
        int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
        int[] frontY = { 1, 0, -1, 0 };

        int[] leftX = { -1, 0, 1, 0 }; // 좌회전 변경 위치점
        int[] leftY = { 0, -1, 0, 1 }; // 좌회전 이후에 현재 바라보는 방향이 반시계방향으로 90도 회전

        int[] rightX = { 1, 0, -1, 0 };  // 우회전 변경 위치점
        int[] rightY = { 0, 1, 0, -1 };

        int _dir = (int)Dir.Up;

        Debug.Log(tileArray[CurNode.x + leftX[_dir], CurNode.y + leftY[_dir]]);
        Debug.Log(tileArray[CurNode.x + leftX[_dir], CurNode.y + leftY[_dir]].tileSprite);
        // 1. 현재 바라보는 방향으로 기준으로 왼쪽으로 갈 수 있는지 확인.
        if (tileArray[CurNode.x + leftX[_dir], CurNode.y + leftY[_dir]].GetTileSprit() == GetdetectTileSprite(0))
        {
            Debug.Log("왼쪽에 Ground가 있습니다.");
        }
        // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
        if (tileArray[(CurNode.x + frontX[_dir]), (CurNode.y + frontY[_dir])].GetTileSprit() == GetdetectTileSprite(0))
        {
            Debug.Log("앞쪽에 Ground가 있습니다.");
        }
        else
            return;
    }

    private Sprite GetdetectTileSprite(int _num)
    {
        if(_num == 0)
        {
            Sprite sprite = sprites[0];
            return sprite;
        }
        else
        {
            return null;
        }

    }
}
