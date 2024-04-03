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
    int sigNum = (int)SignalType.Forward;

    public void Start()
    {
      
    }
    // Sprite code => 해당하는 함수 호출
    public void GetSignCode(string _SpriteName)
    {
         string input = _SpriteName;

        int number = ExtractNumber(input);      
        signalType = (SignalType)number;
    }

    // Signal의 방향을 분석하고, 
    enum SignalType
    {
        Forward = 0,
        Left= 1,
        Right = 2,
        LeftFoward = 3,
        FowardRight = 4,
        LeftRight = 5,
        All = 6,
        Stop = 7
    }
    SignalType signalType;
    
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
            throw new ArgumentException("입력된 문자열에 숫자가 없습니다.");
            // 또는
            // return 0;
        }
    }

    // 해당 오브젝트가 z축으로 얼마나 회전해있는지로 방향값을 측정함

    private void Update()
    {
        // 마우스 왼쪽 버튼을 클릭하면
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 위치를 월드 좌표로 변환
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 월드 좌표를 타일 좌표로 변환
            Vector3Int tilePos = tilemap.WorldToCell(mouseWorldPos);
            // 타일 좌표로 해당하는 타일을 가져옴
            TileBase clickedTile = tilemap.GetTile(tilePos);

            if (clickedTile != null)
            {
                Debug.Log("Clicked tile information: " + clickedTile.name);

                // 해당 타일의 위치를 월드 좌표로 변환하여 게임 오브젝트의 transform을 가져옴
                Vector3 clickedWorldPos = tilemap.GetCellCenterWorld(tilePos);
                // 타일의 회전 정보를 가져옴
                Quaternion tileRotation = tilemap.GetTransformMatrix(tilePos).rotation; ;
                tileRotations.TryGetValue(tilePos, out tileRotation);
                Debug.Log("Clicked tile rotation before: " + tileRotation.eulerAngles);

                // 클릭한 타일의 z축 회전을 90도 증가시킴
                tileRotation *= Quaternion.Euler(0, 0, 90f);
                tileRotations[tilePos] = tileRotation; // 업데이트된 회전 정보 저장
                Debug.Log("Clicked tile rotation after: " + tileRotation.eulerAngles);

                // 타일맵에서 해당 타일의 회전 정보 업데이트
                Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one);
                tilemap.SetTransformMatrix(tilePos, matrix);

            }
            else
            {
                Debug.Log("No tile found at clicked position.");
            }
        }
    }
}


