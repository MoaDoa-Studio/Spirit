using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DetectMove : MonoBehaviour
{
    Node[,] NodeArray;
    
    int[,] intArray, StartNode, CurNode;
    
   
    [SerializeField]
    Button button;
    [SerializeField]
    Sprite[] sprites;
    [SerializeField]
    Vector2Int bottomLeft, topRight, startPos, targetPos;
    
    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    enum Detect
    {
        None,
        Normal,
        Sign
    }
    Detect detection;

    private void Start()
    {
       
    }
    private void Update()
    {
        switch(detection)
        {
            case Detect.None:
                break;
            case Detect.Normal:
                break;
            case Detect.Sign:
                break;
        }
    }
    public void checkDirection()
    {
        Debug.Log(TileDataManager.instance.nodes[1, 1].x);
        int startX = (int)startPos.x - (int)bottomLeft.x;
        int startY = (int)startPos.y - (int)bottomLeft.y;
        
        Debug.Log(startX+" " +startY);

        // 바라보는 방향 기준으로 좌표 변화를 나타냄.  
        int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
        int[] frontY = { 1, 0, -1, 0 };

        int[] leftX = { -1, 0, 1, 0 }; // 좌회전 변경 위치점
        int[] leftY = { 0, -1, 0, 1 }; // 좌회전 이후에 현재 바라보는 방향이 반시계방향으로 90도 회전

        int[] rightX = { 1, 0, -1, 0 };  // 우회전 변경 위치점
        int[] rightY = { 0, 1, 0, -1 };

        int _dir = (int)Dir.Up;

        int CurposX = 2;
        int CurposY = 1;

          Debug.Log(TileDataManager.instance.nodes[startX + leftX[_dir], startY + leftY[_dir]].x);  // x 좌표
          Debug.Log(TileDataManager.instance.nodes[startX + leftX[_dir], startY + leftY[_dir]].y);  // y 좌표
        //  1. 현재 바라보는 방향으로 기준으로 왼쪽으로 갈 수 있는지 확인.
         if (TileDataManager.instance.nodes[CurposX + leftX[_dir], CurposY + leftY[_dir]].nodeSprite == GetdetectSprite(0))
         {
            Debug.Log("왼쪽에 Ground가 있습니다.");
         }
        
        // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
        if (TileDataManager.instance.nodes[(CurposX + frontX[_dir]), (CurposY + frontY[_dir])].nodeSprite == GetdetectSprite(0))
        {
           Debug.Log("앞쪽에 Ground가 있습니다.");
        }
        // else
        //    return;
    }

    private Sprite GetdetectSprite(int _num)
    {
        if (_num == 0)
            return sprites[0];
        else
            return null;
    }
}
