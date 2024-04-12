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
    Text stopduration;
    [SerializeField]
    Vector2Int bottomLeft, topRight, startPos, targetPos;
    
    Node[,] nodes;  // TileDataManager instance.
    Signal signal = new Signal();
    Vector2 currentPosition;
    SpriteRenderer spriteRenderer;
    List<Tuple<Vector2Int, Vector2Int>> buildingList = new List<Tuple<Vector2Int, Vector2Int>>(); // 건물 우상단, 좌하단 좌표 저장.

    public int CurposX = 2;
    public int CurposY = 1;
    // 바라보는 방향 기준 좌표 변화.  
    int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
    int[] frontY = { 1, 0, -1, 0 };

    int[] leftX = { -1, 0, 1, 0 }; // 좌회전 변경 위치점
    int[] leftY = { 0, -1, 0, 1 }; // 좌회전 이후에 현재 바라보는 방향이 반시계방향으로 90도 회전

    int[] rightX = { 1, 0, -1, 0 };  // 우회전 변경 위치점
    int[] rightY = { 0, 1, 0, -1 };

    public float moveSpeed = 1f;
    int _dir = (int)Dir.Up;
    int spiritID;
    int signType;
    public int spiritElement;
   
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
        Factory_MoveMent,
        Basic_MoveMent,
        Move,
        Stop,
        Factory,
        Loot,
        Academy,
        Mark_Check
    }
    Detect detection = Detect.None;

    private void Start()
    {
        nodes = TileDataManager.instance.GetNodes();
        signal = GameObject.Find("[SignalManager]").GetComponent<Signal>();
        spiritID = GetComponent<Spirit>().SpiritID;
        spiritElement = GetComponent<Spirit>().SpiritElement;
        spriteRenderer = GetComponent<SpriteRenderer>();
        int startX = (int)startPos.x - (int)bottomLeft.x;
        int startY = (int)startPos.y - (int)bottomLeft.y;
    }
    private void Update()
    {  
        switch (detection)
        {
            case Detect.None:
                detection = Detect.CheckTile;
                break;
            case Detect.CheckTile:
                CheckTile();
                break;
            case Detect.Factory_MoveMent:
                FactoryMove();
                break;
            case Detect.Basic_MoveMent:
                BaseMove();
                break;
            case Detect.Factory:
                FactoryWork();
                break;
            case Detect.Loot:
                break;
            case Detect.Academy:
                break;
            case Detect.Move:
                Move(CurposX, CurposY);
                break;
            case Detect.Mark_Check:
                MarkCheck(signType);
                break;
            case Detect.Stop:
                StopMove();
                break;
        }
    }


    private void CheckTile()
    {   // 표시 0
        nodes = TileDataManager.instance.GetNodes();

        // 길을 걸을 수 있는 타일일때.
        if (nodes[CurposX, CurposY].isWalk)
        {
            nodes[CurposX, CurposY].spiritElement = spiritElement;  // 노드에 정령원소 체크
            string signName = nodes[CurposX, CurposY].nodeSprite.name;
            signType = ExtractNumber(signName);  // Sign type 판별.
            
            if (nodes[CurposX, CurposY].isSignal)
            {
                detection = Detect.Mark_Check;
                
            }
            
            else
            {
                detection = Detect.Basic_MoveMent;
            }


        }
    }


    private void BaseMove()
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
                // 가야할 타일에 같은 정령이 있다면.
                if (nodes[leftx, lefty].spiritElement == spiritElement) return;
                // 공장 혹은 자원 채집하는 곳이라면.. 꽉 차있다고 하면...
                
                nodes[CurposX, CurposY].spiritElement = 0;
                // 왼쪽 방향으로 90도 회전 
                _dir = (_dir + 1) % 4;

                CurposX += frontX[_dir];
                CurposY += frontY[_dir];

                // if(nodes[curposx,curposy].isFactory) detection = Detect.EnterFactory
                detection = Detect.Move;
                return;
            }
        }
        if (frontx < TileDataManager.instance.sizeX && frontx >= 0 && fronty < TileDataManager.instance.sizeY && fronty >= 0)
        {
             // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
            if (nodes[frontx, fronty].isWalk)
            {
                if (nodes[frontx, fronty].spiritElement == spiritElement) return;

                nodes[CurposX, CurposY].spiritElement = 0;
                CurposX += frontX[_dir];  
                 CurposY += frontY[_dir] ;

                // if(nodes[curposx,curposy].isFactory) detection = Detect.EnterFactory
                detection = Detect.Move;
                return;
            }
        }
        if (rightx < TileDataManager.instance.sizeX && rightx >= 0 && righty < TileDataManager.instance.sizeY && righty >= 0)
        {
            // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
            if (nodes[rightx, righty].isWalk)
            {
                if (nodes[rightx, righty].spiritElement == spiritElement) return;
                nodes[CurposX, CurposY].spiritElement = 0;
                // 우측 방향이라면 90도 회전
                _dir = (_dir - 1 + 4) % 4;

                 CurposX += frontX[_dir];  
                 CurposY += frontY[_dir];

                // if(nodes[curposx,curposy].isFactory) detection = Detect.EnterFactory => 이동할때 점프하는 애니메이션 나오는 함수
                detection = Detect.Move;
                return;
                
            }
        }
        else
            Debug.Log("아무것도 안들어갔음!");
        return;
    }

    private void StopMove()
    {
    }
    private void MarkCheck(int _signType)
    {
        if (_signType == 7)
        {  
            float stopDuration = float.Parse(stopduration.text);
            StartCoroutine(StopSign(stopDuration));

            return;
        }
        
        else
            signal.SetSignType(_signType, nodes[CurposX, CurposY].rotation, _dir, CurposX, CurposY); // signal 에 sign 타입을 지정하고 dir 방향을 받음
                                                                                           // 정령 Dir = 신호 Dir이 같다면,
        
        if (_dir == signal.dir)
        {

            CurposX += signal.pair.Item1;
            CurposY += signal.pair.Item2;

            // 표지방향으로 정령의 방향을 꺽어줌.
            _dir = signal.spiritDir;

            // 타일 체크후 걸을 수 없다면..
            if (!nodes[CurposX, CurposY].isWalk)
            {

                detection = Detect.None;
            }
            else
            {
                // 다음 칸이 공장 혹은 자원 채집하는 곳이라면..
                detection = Detect.Move;
            }
        }
        else
        {

            detection = Detect.Basic_MoveMent;
        }
    }

    private void FactoryMove()
    {
        
    }

    private void Move(int _curposx, int _curposy)
    {
        if(detection != Detect.Move) { return; }
        Vector2 targetVector = new Vector2(_curposx, _curposy);
        Vector2 direction = (targetVector - (Vector2)transform.position).normalized;

        if(Vector2.Distance(targetVector, transform.position) <= 0.01f)
        {
            transform.position = targetVector;
            detection = Detect.None;
            return;
            //if(direction.magnitude == 0)
        }
        else
        {
            Vector2 movement = direction.normalized * moveSpeed * Time.smoothDeltaTime;
             
            transform.Translate(movement);
            //{
            //}
        }
    }

    private void FactoryWork()
    {
        // 일하는 시간만큼 spriterenderer = null
        float workatFactory = 10f;
        while(workatFactory <= 0)
        {
            spriteRenderer.enabled = false;
            workatFactory -= Time.deltaTime;
        }
        spriteRenderer.enabled = true;
        workatFactory = 10f;
        // 이동 로직 구현
        detection = Detect.None;
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

    IEnumerator StopSign(float _time)
    {
        yield return new WaitForSeconds(_time);
        Debug.Log("여기 호출되엇ㅇ므");
        detection = Detect.Basic_MoveMent;
        Debug.Log("detection의 상태는 " + detection);
    }
}
