using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DetectMove : MonoBehaviour
{
    [SerializeField]
    Button button;
    [SerializeField]
    Sprite[] sprites;
    [SerializeField]
    Vector2Int bottomLeft, topRight, startPos, targetPos;
    [SerializeField]
    int CurposX = 2;
    [SerializeField]
    int CurposY = 1;
    
    Node[,] nodes;  // TileDataManager instance.

    // 바라보는 방향 기준 좌표 변화.  
    int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
    int[] frontY = { 1, 0, -1, 0 };

    int[] leftX = { -1, 0, 1, 0 }; // 좌회전 변경 위치점
    int[] leftY = { 0, -1, 0, 1 }; // 좌회전 이후에 현재 바라보는 방향이 반시계방향으로 90도 회전

    int[] rightX = { 1, 0, -1, 0 };  // 우회전 변경 위치점
    int[] rightY = { 0, 1, 0, -1 };

    public float moveSpeed = 1f;
    int _dir = (int)Dir.Up;
    Vector2 currentPosition;

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
        Search,
        Move,
        Sign
    }
    Detect detection = Detect.Search;

    private void Start()
    {
        nodes = TileDataManager.instance.GetNodes();
        int startX = (int)startPos.x - (int)bottomLeft.x;
        int startY = (int)startPos.y - (int)bottomLeft.y;
    }
    private void Update()
    {
        currentPosition = new Vector2(transform.position.x, transform.position.y);
        //MoveNext(CurposX, CurposY);
        switch (detection)
        {
            case Detect.None:
                break;
            case Detect.Search:
                checkDirection();
                break;
            case Detect.Move:
                MoveNext(CurposX, CurposY);
                break;
            case Detect.Sign:
                break;
        }
    }
    public void checkDirection()
    {
        nodes = TileDataManager.instance.GetNodes();    // 매우 매우 중요!! 코드 간결화
                                                        //Debug.Log(TileDataManager.instance.nodes[1, 1].x);
                                                        //Debug.Log("간단하게 불러오는.. " + nodes[1, 1].x);

        int leftx = CurposX + leftX[_dir];
        int lefty = CurposY + leftY[_dir];
        int frontx = CurposX + frontX[_dir];
        int fronty = CurposY + frontY[_dir];
        int rightx = CurposX + rightX[_dir];
        int righty = CurposY + rightY[_dir];

        if (leftx < TileDataManager.instance.sizeX && leftx >= 0 && lefty < TileDataManager.instance.sizeY && lefty >= 0)
        {
            //  1. 현재 바라보는 방향으로 기준으로 왼쪽으로 갈 수 있는지 확인.
            if (nodes[leftx, lefty].nodeSprite == GetdetectSprite(0))
            {
                Debug.Log("왼쪽에 Ground가 있습니다.");
                // 왼쪽 방향으로 90도 회전 
                _dir = (_dir + 1) % 4;

                CurposX += frontX[_dir];
                CurposY += frontY[_dir];
                detection = Detect.Move;
                return;
                Debug.Log("왼쪽 여기로 빠지면 안되는데");
            }
        }
        if (frontx < TileDataManager.instance.sizeX && frontx >= 0 && fronty < TileDataManager.instance.sizeY && fronty >= 0)
        {
            Debug.Log("else if로 들어옴?");
            // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
            if (nodes[frontx, fronty].nodeSprite == GetdetectSprite(0))
            {
                Debug.Log("앞쪽에 Ground가 있습니다.");
                 CurposX += frontX[_dir];  
                 CurposY += frontY[_dir] ;
                 detection = Detect.Move;
                return;
                Debug.Log("앞으로 여기로 빠지면 안되는데");
            }
        }
        if (rightx < TileDataManager.instance.sizeX && rightx >= 0 && righty < TileDataManager.instance.sizeY && righty >= 0)
        {
            // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
            if (nodes[rightx, righty].nodeSprite == GetdetectSprite(0))
            {
                Debug.Log("앞쪽에 Ground가 있습니다.");
                // 우측 방향이라면 90도 회전
                _dir = (_dir - 1 + 4) % 4;

                 CurposX += frontX[_dir];  
                 CurposY += frontY[_dir]; 
                 detection = Detect.Move;
                return;
                Debug.Log("오른쪽 여기로 빠지면 안되는데");
            }
        }
        else
            Debug.Log("아무것도 안들어갔음!");
        return;
    }



    private Sprite GetdetectSprite(int _num)
    {   // Ground sprite일때.
        if (_num == 0)
            return sprites[0];
        else
            return null;
    }

    private void MoveNext(int _curposx, int _curposy)
    {
        Vector2 targetVector = new Vector2(_curposx, _curposy);
        Vector2 direction = (targetVector - currentPosition).normalized;

        if(direction.magnitude >= 0.001f)
        {
            Vector2 movement = direction.normalized * moveSpeed * Time.deltaTime;

            transform.Translate(movement);
        }
        else
        {
            transform.position = targetVector;
            if(direction.magnitude == 0)
            {
                detection = Detect.Search;
            }
        }
    }
}
