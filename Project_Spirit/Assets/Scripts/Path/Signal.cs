using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using UnityEngine.Tilemaps;

public class Signal : MonoBehaviour
{
    public Dictionary<Vector3Int, Quaternion> tileRotations = new Dictionary<Vector3Int, Quaternion>(); // 타일의 위치와 회전 정보를 저장할 딕셔너리

    [SerializeField]
    Sprite[] signalSprite;
    [SerializeField]
    Tilemap tilemap;

    int[] frontdirX = { 0, -1, 0, 1 };
    int[] frontdirY = { 1, 0, -1, 0 };

    int[] leftdirX = { -1, 0, 1, 0 }; 
    int[] leftdirY = { 0, -1, 0, 1 };

    int[] rightdirX = { 1, 0, -1, 0 }; 
    int[] rightdirY = { 0, 1, 0, -1 };

    public int dir;
    public int spiritDir;
    public (int,int) pair;
    enum Dir
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    public void Start()
    {
      
    }
    // Sprite code => 해당하는 함수 호출
    public void SetSignType(int _num, Quaternion _quaternion, int _dir)
    {
        int number = _num;
        Quaternion rot = _quaternion;
        signalType = (SignalType)number + 1;    // Type + 1 값으로 enum 선언.
        Debug.Log("표지탑은 다음과 같습니다." + ((SignalType)number + 1));
        dir = CheckRotation(rot);
        spiritDir = _dir;
        Setdirerction(dir);
    }

    // Signal의 방향을 분석하고, 
    enum SignalType
    {
        None = 0,
        Forward = 1,
        Left= 2,
        Right = 3,
        LeftFoward = 4,
        FowardRight = 5,
        LeftRight = 6,
        All = 7,
        Stop = 8
    }
    SignalType signalType;
    
    
    // 해당 오브젝트가 z축으로 얼마나 회전해있는지로 방향값을 측정함

    public void Setdirerction(int _dir)
    {
        switch(signalType)
        {
            case SignalType.None:
                break;
            case SignalType.Forward:
                Forward(_dir);
                break;
            case SignalType.Left:
                Left(_dir);
                break;
            case SignalType.Right:
                Right(_dir);
                break;
            case SignalType.LeftFoward:
                LeftFoward();
                break;
            case SignalType.FowardRight:
                FowardRight();
                break;
            case SignalType.LeftRight:
                LeftRight();
                break;
            case SignalType.All:
                All();
                break;
            case SignalType.Stop:
                Stop();
                break;
        }
    }

    void Forward(int _dir)
    {  
        Debug.Log("전진방향에서 회전방향 몇번 ? : " + _dir);
       
        int frontx = frontdirX[_dir];
        int fronty = frontdirY[_dir];
        // 당도할 방향
        pair = (frontx, fronty);
        signalType = SignalType.None;
    }
    void Left(int _dir)
    {
        Debug.Log("왼쪽 표지방향 : " + _dir);
        int leftx = leftdirX[_dir];
        int lefty = leftdirY[_dir];

        pair = (leftx, lefty);
        spiritDir = (spiritDir + 1) % 4;
        signalType = SignalType.None;
    }
    void Right(int _dir)
    {
        Debug.Log("오른쪽 표지방향 : " + _dir);
        int rightx = rightdirX[_dir];
        int righty = rightdirY[_dir];

        pair = (rightx, righty);
        spiritDir = (spiritDir - 1 + 4) % 4;
        signalType = SignalType.None;
    }

    void LeftFoward()
    {

        signalType = SignalType.None;
    }

    void FowardRight()
    {

        signalType = SignalType.None;
    }

    void LeftRight()
    {

        signalType = SignalType.None;
    }

    void All()
    {

        signalType = SignalType.None;
    }

    void Stop()
    {

        signalType = SignalType.None;
    }

    int CheckRotation(Quaternion _quaternion)
    {   // Quaternion을 오일러 각도로 변환
        Vector3 eulerRotation = _quaternion.eulerAngles;
        // 회전값을 0 ~ 360 범위로 조정
        float adjustedRotation = eulerRotation.z % 360;
        // 0에서 360 사이의 회전값에 대한 int 값을 계산
        int intValue = Mathf.FloorToInt(adjustedRotation / 90);

        // 값이 4를 초과하는 경우 처리
        if (intValue > 4)
        {
            intValue = 0; // 360도 회전은 0도와 같으므로 1로 처리
        }

        return intValue;
    }
}
