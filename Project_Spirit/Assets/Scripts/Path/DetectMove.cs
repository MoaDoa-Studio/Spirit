using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TMPro;
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
    SpriteRenderer spriteRenderer;
    
    public float CurposX = 2;
    public float CurposY = 1;
    // 바라보는 방향 기준 좌표 변화.  
    int[] frontX = { 0, -1, 0, 1 };  // Up , Left, Down, Right
    int[] frontY = { 1, 0, -1, 0 };

    int[] leftX = { -1, 0, 1, 0 }; 
    int[] leftY = { 0, -1, 0, 1 }; 

    int[] rightX = { 1, 0, -1, 0 };  
    int[] rightY = { 0, 1, 0, -1 };

    public float moveSpeed = 1f;
    public int spiritElement;
    int _dir = (int)Dir.Up;
    int spiritID;
    int signType;
    bool isFactory = false;
    bool isLoot = false;
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
                FactoryWork(); // 공장에서 움직임 시작
                break;
            case Detect.Basic_MoveMent:
                BaseMove();
                break;
            case Detect.Factory:
                FactoryWork(); // 일하는 값
                break;
            case Detect.Loot:
                LootWork();
                break;
            case Detect.Academy:
                break;
            case Detect.Move:
                Move((int)CurposX, (int)CurposY);
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
    {   
        nodes = TileDataManager.instance.GetNodes();
        if (isFactory) return;
        if (isLoot) return;
        if (nodes[(int)CurposX, (int)CurposY].isWalk)
        {
            if(nodes[(int)CurposX, (int)CurposY].isBuild)
            {
                detection = Detect.Factory_MoveMent;
            }
            // 자원일때 처리해야할 로직 추가 필요.
            if(nodes[(int)CurposX, (int)CurposY].GetNodeType() == 6 || nodes[(int)CurposX, (int)CurposY].GetNodeType() == 7)
            {
                detection = Detect.Loot;
            }
            else
            {
                // 노드 정령원소 확인.
                nodes[(int)CurposX, (int)CurposY].spiritElement = spiritElement;  
                string signName = nodes[(int)CurposX, (int)CurposY].nodeSprite.name;
                // Sign type 판별.
                signType = ExtractNumber(signName);  
            
                if (nodes[(int)CurposX, (int)CurposY].isSignal)
                {
                    detection = Detect.Mark_Check;
                }
            
                else
                {
                    detection = Detect.Basic_MoveMent;
                }

            }


        }
    }

    #region 기본 움직임
    private void BaseMove()
    {
        nodes = TileDataManager.instance.GetNodes();    // 매우 매우 중요!! 코드 간결화
                                                        
        float leftx = CurposX + leftX[_dir];
        float lefty = CurposY + leftY[_dir];
        float frontx = CurposX + frontX[_dir];
        float fronty = CurposY + frontY[_dir];
        float rightx = CurposX + rightX[_dir];
        float righty = CurposY + rightY[_dir];

        if (leftx < TileDataManager.instance.sizeX && leftx >= 0 && lefty < TileDataManager.instance.sizeY && lefty >= 0)
        {
            //  1. 현재 바라보는 방향으로 기준으로 왼쪽으로 갈 수 있는지 확인.
            if (nodes[(int)leftx, (int)lefty].isWalk)
            {
                // 가야할 타일에 같은 정령이 있다면.
                if (nodes[(int)leftx, (int)lefty].spiritElement == spiritElement) return;
                
                nodes[(int)CurposX, (int)CurposY].spiritElement = 0;
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
            if (nodes[(int)frontx, (int)fronty].isWalk)
            {
                if (nodes[(int)frontx, (int)fronty].spiritElement == spiritElement) return;
               
                nodes[(int)CurposX, (int)CurposY].spiritElement = 0;
                CurposX += frontX[_dir];  
                CurposY += frontY[_dir] ;
                detection = Detect.Move;
                return;
            }
        }
         if (rightx < TileDataManager.instance.sizeX && rightx >= 0 && righty < TileDataManager.instance.sizeY && righty >= 0)
        {
            // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
            if (nodes[(int)rightx, (int)righty].isWalk)
            {
                if (nodes[(int)rightx, (int)righty].spiritElement == spiritElement) return;
                
                nodes[(int)CurposX, (int)CurposY].spiritElement = 0;
                _dir = (_dir - 1 + 4) % 4;

                 CurposX += frontX[_dir];  
                 CurposY += frontY[_dir];
                 detection = Detect.Move;
                return;
            }
        }
        else
            Debug.Log("아무것도 안들어갔음!");
       
    }
    private void Move(int _curposx, int _curposy)
    {
        if(detection != Detect.Move) { return; }
        Vector2 targetVector = new Vector2(_curposx + 0.5f, _curposy + 0.5f);
        Vector2 direction = (targetVector - (Vector2)transform.position).normalized;

        if(Vector2.Distance(targetVector, transform.position) <= 0.01f)
        {
            transform.position = targetVector;
            detection = Detect.None;
            return;
        }
        else
        {
            Vector2 movement = direction.normalized * moveSpeed * Time.smoothDeltaTime;
            transform.Translate(movement);
        }
    }
    #endregion
    private void MarkCheck(int _signType)
    {
        if (_signType == 7)
        {  
            float stopDuration = float.Parse(stopduration.text);
            StartCoroutine(StopSign(stopDuration));
             return;
        }
        else
            signal.SetSignType(_signType, nodes[(int)CurposX, (int)CurposY].rotation, _dir, (int)CurposX, (int)CurposY); // signal 에 sign 타입을 지정하고 dir 방향을 받음
                                                                                          
        
        if (_dir == signal.dir)
        {
            CurposX += signal.pair.Item1;
            CurposY += signal.pair.Item2;

            // 표지방향으로 정령의 방향을 꺽어줌.
            _dir = signal.spiritDir;

            // 타일 체크후 걸을 수 없다면..
            if (!nodes[(int)CurposX, (int)CurposY].isWalk)
            { detection = Detect.None;}
            else
            {// 다음 칸이 공장 혹은 자원 채집하는 곳이라면..
                detection = Detect.Move;}
        }
        else
        {
            detection = Detect.Basic_MoveMent;
        }
    }

    #region 공장 움직임
    private void FactoryWork()
    {
        StartCoroutine(Buildpattern());
        detection = Detect.None;
        return;
    }
    IEnumerator Buildpattern()
    {
        FindFactoryPoint();
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(7f);

        isFactory = false;
        spriteRenderer.enabled = true;
        detection = Detect.None;
        
    }
    private void FindFactoryPoint()
    {
        if (!isFactory)
        {
            Debug.Log("나 공장 일함? 헿");
            Vector2 sP = nodes[(int)CurposX, (int)CurposY].building.connectedRoads.Item1;
            Vector2 nP = nodes[(int)CurposX, (int)CurposY].building.connectedRoads.Item2;
            Vector2 transformPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 target = Vector2.zero;
            float distanceToA = Vector2.Distance(transformPosition, sP);
            float distanceToB = Vector2.Distance(transformPosition, nP);
            if (distanceToA < distanceToB)
            {
                isFactory = true;
                target = nP;
                CurposX = target.x; CurposY = target.y;
                RedirectionafterFactory(CurposX, CurposY);
            }
            else
            {
                isFactory = true;
                target = sP;
                CurposX = target.x; CurposY = target.y;
                RedirectionafterFactory(CurposX, CurposY);
            } 

            transform.position = new Vector2(target.x + 0.5f, target.y + 0.5f);

        }
        
    }
    private void RedirectionafterFactory(float _curposX, float _curposY)
    {
        int[] FactorydirX = { 0, 0, 1, -1 };
        int[] FactorydirY = { 1, -1, 0, 0 };

        for(int i = 0; i < 4; i++)
        {
            if (nodes[(int)_curposX + FactorydirX[i], (int)_curposY + FactorydirY[i]].isBuild)
            {
                if (i == 0)
                {
                    _dir = (int)Dir.Down;
                }
                else if (i == 1)
                {
                    _dir = (int)Dir.Up;
                }
                else if (i == 2)
                {
                    _dir = (int)Dir.Left;
                }
                else
                    _dir = (int)Dir.Right;
            }
        }
    }
    #endregion

    #region 자원 움직임
    void LootWork()
    {
        StartCoroutine(LootPattern());
        detection = Detect.None;
        return;
    }

    IEnumerator LootPattern()
    {
        FindLootPoint();
        spriteRenderer.enabled = false;
        yield return new WaitForSeconds(7f);

        isLoot = false;
        spriteRenderer.enabled = true;
        detection = Detect.None;
    }

    void FindLootPoint()
    {
        if(!isLoot)
        {
            Debug.Log("나 자원 일함!!");
            Vector2 sP = nodes[(int)CurposX, (int)CurposY].resourceBuilding.connectedRoads.Item1;
            Vector2 nP = nodes[(int)CurposX, (int)CurposY].resourceBuilding.connectedRoads.Item2;
            Vector2 transformPosition = new Vector2(transform.position.x, transform.position.y);
            Vector2 target = Vector2.zero;
            float distanceToA = Vector2.Distance(transformPosition, sP);
            float distanceToB = Vector2.Distance(transformPosition, nP);
            if (distanceToA < distanceToB)
            {
                isLoot = true;
                target = nP;
                CurposX = target.x; CurposY = target.y;
                RedirectionafterFactory(CurposX, CurposY);
            }
            else
            {
                isLoot = true;
                target = sP;
                CurposX = target.x; CurposY = target.y;
                RedirectionafterLoot(CurposX, CurposY);
            }

            transform.position = new Vector2(target.x + 0.5f, target.y + 0.5f);

        }
    
    }
    void RedirectionafterLoot(float _curposX, float _curposY)
    {
        int[] FactorydirX = { 0, 0, 1, -1 };
        int[] FactorydirY = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            if (nodes[(int)_curposX + FactorydirX[i], (int)_curposY + FactorydirY[i]].GetNodeType() == 6 || nodes[(int)_curposX + FactorydirX[i], (int)_curposY + FactorydirY[i]].GetNodeType() == 7)
            {
                if (i == 0)
                {
                    _dir = (int)Dir.Down;
                }
                else if (i == 1)
                {
                    _dir = (int)Dir.Up;
                }
                else if (i == 2)
                {
                    _dir = (int)Dir.Left;
                }
                else
                    _dir = (int)Dir.Right;
            }
        }
    }

    #endregion

    private void StopMove()
    {
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
        Debug.Log("정지신호 호출");
        detection = Detect.Basic_MoveMent;
        Debug.Log("detection의 상태는 " + detection);
    }



}
