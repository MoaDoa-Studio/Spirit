using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
    Signal signal = new Signal();

    // 바라보는 방향 기준 좌표 변화.  
    int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
    int[] frontY = { 1, 0, -1, 0 };

    int[] leftX = { -1, 0, 1, 0 }; // 좌회전 변경 위치점
    int[] leftY = { 0, -1, 0, 1 }; // 좌회전 이후에 현재 바라보는 방향이 반시계방향으로 90도 회전

    int[] rightX = { 1, 0, -1, 0 };  // 우회전 변경 위치점
    int[] rightY = { 0, 1, 0, -1 };

    public float moveSpeed = 1f;
    int _dir = (int)Dir.Up;
    int nxtposX, nxtposY;
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
        CheckTile,
        Normal,
        Move,
        Factory,
        Loot,
        Academy,
        Sign
    }
    Detect detection = Detect.None;

    private void Start()
    {
        nodes = TileDataManager.instance.GetNodes();
        signal = GameObject.Find("[SignalManager]").GetComponent<Signal>();
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
                detection = Detect.CheckTile;
                break;
            case Detect.CheckTile:
                CheckTile();
                break;
            case Detect.Normal:
                NormalDirection();
                break;
            case Detect.Factory:
                break;
            case Detect.Loot:
                break;
            case Detect.Academy:
                break;
            case Detect.Move:
                MoveNext(CurposX, CurposY);
                break;

            case Detect.Sign:
                break;
        }
    }


    private void CheckTile()
    {   // 표시 0
        nodes = TileDataManager.instance.GetNodes();
        string callSign = nodes[CurposX, CurposY].nodeSprite.name;
        int num = ExtractNumber(callSign);  // Sign type 판별.
        if(nodes[CurposX, CurposY].isSignal)
        {   
            signal.SetSignType(num, nodes[CurposX,CurposY].rotation, _dir); // signal 에 sign 타입을 지정하고 dir 방향을 받음
            Debug.Log("체크타일의 방향이 없어서 지금 방황중이니?");
            // 정령 Dir = 신호 Dir이 같다면,
            if(_dir == signal.dir)
            {
                Debug.Log("표지와 정령의 방향이 일치해요!");
                CurposX += signal.pair.Item1;
                CurposY += signal.pair.Item2;

                // 표지방향으로 정령의 방향을 꺽어줘야함
                _dir = signal.spiritDir;

                // 길이 없으면 // return
                detection = Detect.Move;
            }
            else
            {

                detection = Detect.Normal;
            }
        }
        else
        {
            detection = Detect.Normal;
        }

    }


    private void NormalDirection()
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
            if (nodes[leftx, lefty].isWalk)
            {
                // 왼쪽 방향으로 90도 회전 
                _dir = (_dir + 1) % 4;

                CurposX += frontX[_dir];
                CurposY += frontY[_dir];
                detection = Detect.Move;
                return;
                
            }
        }
        if (frontx < TileDataManager.instance.sizeX && frontx >= 0 && fronty < TileDataManager.instance.sizeY && fronty >= 0)
        {
             // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
            if (nodes[frontx, fronty].isWalk)
            {  
                CurposX += frontX[_dir];  
                 CurposY += frontY[_dir] ;
                 detection = Detect.Move;
                return;
              
            }
        }
        if (rightx < TileDataManager.instance.sizeX && rightx >= 0 && righty < TileDataManager.instance.sizeY && righty >= 0)
        {
            // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
            if (nodes[rightx, righty].isWalk)
            {
                // 우측 방향이라면 90도 회전
                _dir = (_dir - 1 + 4) % 4;

                 CurposX += frontX[_dir];  
                 CurposY += frontY[_dir]; 
                 detection = Detect.Move;
                return;
                
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
                detection = Detect.None;
            }
        }
    }

    static int ExtractNumber(string input)
    {
        // 정규표현식을 사용하여 숫자만 추출
        Match match = Regex.Match(input, @"\d+");

        // 추출된 숫자가 있는지 확인하고 반환
        if (match.Success)
        {
            return int.Parse(match.Value);
        }
        else
        {
            // 숫자가 없는 경우 예외처리 또는 기본값 반환
             return 100;
        }
    }
}
