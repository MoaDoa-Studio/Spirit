using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public class MonsterTileMapManager : MonoBehaviour
{
    public static MonsterTileMapManager instance = null;
    public int[,] tileArray = new int[103, 103]; //좌표 값에 int를 넣어 타일 상태 설정

    public Tilemap monsterTileMap; //보스 연출용 타일맵

    public Vector2Int position;
    public Vector2Int bottomLeft, topRight, startPos, targetPos;

    public int sizeX, sizeY;

    private TileDataManager tileDataManager;

    [Header("빌딩 타일 좌표 저장")]
    public List<ObjectNode> objNodeList = new List<ObjectNode>();

    [Header("몬스터 이동 경로 연출 타일")]
    [SerializeField] private TileBase monsterRoadTile; //몬스터 이동 경로 나타내는 타일
    [SerializeField] private TileBase deadTile; //죽은 타일

    [Header("깔린 보스 경로 타일 저장")]
    public List<Vector3Int> redTileList;

    [SerializeField] private float waitSecond;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            sizeX = topRight.x - bottomLeft.x + 1;
            sizeY = topRight.y - bottomLeft.y + 1;
        }

        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        tileDataManager = TileDataManager.instance;
        //SetRootPointToVector3Int();
        //OnPointTileSet();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            OffRoadTileSet();
        }
    }

    /// <summary>
    /// x, y좌표 모두 0 ~ 103 사이의 숫자만 적용
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool isRange(int x, int y)
    {
        return x >= 0 && x < 103 && y >= 0 && y < 103;
    }

    /// <summary>
    /// 타일 초기화(완X) ==> 나중에 모든 타일 초기화 기능 만들고 싶을 때 수정
    /// </summary>
    private void OffRoadTileSet()
    {
        //남아있는 모든 타일 제거 기능 ==> 나중에 수정
        //for (int iNum01 = vec3IntMonsterPoint.x; iNum01 <= vec3IntRootList[0].x; iNum01++)
        //{
        //    Vector3Int setX = new Vector3Int(iNum01, vec3IntRootList[0].y, 0);

        //    monsterTileMap.SetTile(setX, null);

        //    redTileList.Remove(setX);
        //}

        //for (int iNum02 = vec3IntRootList[0].y; iNum02 >= vec3IntRootList[1].y; iNum02--)
        //{
        //    Vector3Int setY = new Vector3Int(vec3IntRootList[1].x, iNum02, 0);

        //    monsterTileMap.SetTile(setY, null);


        //    redTileList.Remove(setY);
        //}
    }

    /// <summary>
    /// 일정 시간 마다 타일 나오는 연출 추가
    /// </summary>
    public IEnumerator PC_OnRoadTileSet(MonsterMove sc)
    {
        int count = sc.monsterRootList.Count;

        for (int iNum01 = 0; iNum01 < count; iNum01++)
        {
            if (iNum01 != count - 1)
            {
                float curPositionX = sc.monsterRootList[iNum01].position.x; //현재 좌표의 x값
                float nextPositionX = sc.monsterRootList[iNum01 + 1].position.x; //다음 좌표의 x값
                float curPositionY = sc.monsterRootList[iNum01].position.y; //현재 좌표의 y값
                float nextPositionY = sc.monsterRootList[iNum01 + 1].position.y; //다음 좌표의 y값

                bool xLine = (curPositionY == nextPositionY) ? true : false; //현재와 다음 좌표의 y값이 일치하면 x축 이동

                if (xLine) //x축 이동일 때
                {
                    if (curPositionX < nextPositionX) //이동 방향이 우측일 경우
                    {
                        for (int iNum02 = (int)curPositionX; iNum02 < (int)nextPositionX; iNum02++) //x++
                        {
                            yield return new WaitForSeconds(waitSecond);

                            Vector3Int setX = new Vector3Int(iNum02, (int)curPositionY, 0); //y값 고정하며 설치

                            //monsterTileMap.SetTile(setX, monsterRoadTile); //빨간 타일 설치
                            SetRoadSquareTile(setX, sc.size);

                            sc.targetList.Add(setX); //깔린 타일 좌표를 몬스터 경로 스크립트에 저장
                        }
                    }

                    else if (curPositionX > nextPositionX) //이동 방향이 좌측일 경우
                    {
                        for (int iNum02 = (int)curPositionX; iNum02 > (int)nextPositionX; iNum02--) //x--
                        {
                            yield return new WaitForSeconds(waitSecond);

                            Vector3Int setX = new Vector3Int(iNum02, (int)curPositionY, 0); //y값 고정하며 설치

                            //monsterTileMap.SetTile(setX, monsterRoadTile); //빨간 타일 설치
                            SetRoadSquareTile(setX, sc.size);

                            sc.targetList.Add(setX); //깔린 타일 좌표를 몬스터 경로 스크립트에 저장
                        }
                    }
                }

                else //!xLine == y축 이동일 때
                {
                    if (curPositionY < nextPositionY) //이동 방향이 위일 경우
                    {
                        for (int iNum02 = (int)curPositionY; iNum02 < (int)nextPositionY; iNum02++) //y++
                        {
                            yield return new WaitForSeconds(waitSecond);

                            Vector3Int setY = new Vector3Int((int)curPositionX, iNum02, 0); //x값 고정하며 설치

                            //monsterTileMap.SetTile(setY, monsterRoadTile); //빨간 타일 설치
                            SetRoadSquareTile(setY, sc.size);

                            sc.targetList.Add(setY); //깔린 타일 좌표를 몬스터 경로 스크립트에 저장
                        }
                    }

                    else if (curPositionY > nextPositionY) //이동 방향이 아래일 경우
                    {
                        for (int iNum02 = (int)curPositionY; iNum02 > (int)nextPositionY; iNum02--) //y--
                        {
                            yield return new WaitForSeconds(waitSecond);

                            Vector3Int setY = new Vector3Int((int)curPositionX, iNum02, 0); //x값 고정하며 설치

                            //monsterTileMap.SetTile(setY, monsterRoadTile); //빨간 타일 설치
                            SetRoadSquareTile(setY, sc.size);

                            sc.targetList.Add(setY); //깔린 타일 좌표를 몬스터 경로 스크립트에 저장
                        }
                    }
                }
            }

            else //iNum01 == count - 1
            {
                //마지막 좌표 찍기
                int x = (int)sc.monsterRootList[iNum01].position.x;
                int y = (int)sc.monsterRootList[iNum01].position.y;

                yield return new WaitForSeconds(waitSecond);

                Vector3Int setFinalTile = new Vector3Int(x, y, 0);

                //monsterTileMap.SetTile(setFinalTile, monsterRoadTile); //빨간 타일 설치
                SetRoadSquareTile(setFinalTile, sc.size);

                sc.targetList.Add(setFinalTile); //깔린 타일 좌표를 몬스터 경로 스크립트에 저장

                sc.setRedTile = true; //타일 설치 완료 표시
            }
        }
    }

    /// <summary>
    /// 정사각형 타일 까는 코드
    /// </summary>
    /// <param name="_center"></param>
    /// <param name="_size"></param>
    private void SetRoadSquareTile(Vector3Int _center, int _size)
    {
        //정사각형의 반지름을 계산
        int radius = _size / 2;

        //정사각형의 좌표를 반복하여 타일을 배치
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                //현재 타일의 좌표를 계산
                Vector3Int tilePosition = new Vector3Int(_center.x + x, _center.y + y, 0);

                //타일을 타일맵에 배치
                monsterTileMap.SetTile(tilePosition, monsterRoadTile);
            }
        }
    }

    /// <summary>
    /// 몬스터가 닿은 타일을 죽은 타일로 변경
    /// 매개변수 (x좌표, y좌표)
    /// </summary>
    public void P_SetDeadTile(int _x, int _y, int _size)
    {
        Vector3Int setVector = new Vector3Int(_x, _y, 0);

        monsterTileMap.SetTile(setVector, deadTile); //_x, _y좌표 타일을 죽은 타일로 변경
        
        //정사각형의 반지름을 계산
        int radius = _size / 2;

        //정사각형의 좌표를 반복하여 타일을 배치
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                //현재 타일의 좌표를 계산
                Vector3Int tilePosition = new Vector3Int(setVector.x + x, setVector.y + y, 0);

                if (tileDataManager.GetTileType(tilePosition.x, tilePosition.y) == 1) //만약 읽은 타일이 건물인 경우
                {
                    Debug.Log($"건물 타일을 읽습니다.");
                    //건물 읽기
                    SearchObject(tilePosition);
                }

                //타일을 타일맵에 배치
                monsterTileMap.SetTile(tilePosition, deadTile);

                if (tileDataManager.GetTileType(tilePosition.x, tilePosition.y) != 10) //죽은 타일 타입이 설정되어 있지 않으면
                {
                    tileDataManager.SetTileType(tilePosition.x, tilePosition.y, 10); //타일 설정
                }
            }
        }

        //tileDataManager.SetTileType(_x, _y, 10); //_x, _y좌표 타일의 타입을 10(enum번호)로 변경
        //아래는 검은 타일 타입 변경 확인용 ==> 10으로 변경되는 것을 확인
        //Debug.Log($"{setVector}의 타입 : {tileDataManager.GetTileType(_x, _y)}");
    }

    private void SearchObject(Vector3Int _tile)
    {
        int count = objNodeList.Count;

        for (int i = 0; i < count; i++)
        {
            //읽을 범위 설정
            bool inUpperRightX = _tile.x <= objNodeList[i].range.Item1.x;
            bool inUpperRightY = _tile.y <= objNodeList[i].range.Item1.y;
            bool inBottomLeftX = _tile.x >= objNodeList[i].range.Item2.x;
            bool inBottomLeftY = _tile.y >= objNodeList[i].range.Item2.y;

            //읽은 타일이 범위 안에 들어가면
            if (inUpperRightX && inUpperRightY && inBottomLeftX && inBottomLeftY)
            {
                if (!objNodeList[i].isBreaking) //건물이 닿아있지 않은 상태일 때
                {
                    objNodeList[i].isBreaking = true; //건물 닿은 판정으로 만들기

                    if(objNodeList[i].obj.gameObject.GetComponent<Building>().bomb)
                    {
                        // 스턴 넣는 ...
                    }

                    //건물 파괴 코드 작성해보기
                    //Destroy(objNodeList[i].obj);
                    //objNodeList.RemoveAt(i);

                    return;
                }
            }
        }
    }

    /// <summary>
    /// 몬스터가 건물 타일을 완전히 지나쳤는지 체크하는 함수
    /// 이 코드는 타일 하나 이동할 때마다 발동 되는데 건물이 만약 철거 될 경우 리스트가 줄어들기 때문에 매번 최신화 시킴
    /// </summary>
    public void P_CheckPassObject(int _x, int _y, int _size)
    {
        if (objNodeList.Count == 0) //설치된 건물이 없으면 리턴
        {
            return;
        }

        else
        {
            int count = objNodeList.Count; //모든 리스트 검색

            for (int i = 0; i < count; i++)
            {
                if (objNodeList[i].isBreaking) //만약 부서지고 있는 건물일 경우
                {
                    //정사각형의 반지름을 계산
                    int radius = _size / 2;

                    //정사각형의 좌표를 반복하여 타일을 배치
                    for (int x = -radius; x <= radius; x++)
                    {
                        for (int y = -radius; y <= radius; y++)
                        {
                            Vector2Int searchVector = new Vector2Int(_x + x, _y + y); //탐지 좌표

                            bool inX = searchVector.x <= objNodeList[i].range.Item1.x && searchVector.x >= objNodeList[i].range.Item2.x;
                            //bool inBottomX = searchVector.x >= objNodeList[i].range.Item2.x;
                            bool inY = searchVector.y <= objNodeList[i].range.Item1.y && searchVector.y >= objNodeList[i].range.Item2.y;
                            //bool inBottomY = searchVector.y >= objNodeList[i].range.Item2.y;

                            if (inX && inY)
                            {
                                return; //하나라도 범위 안에 들어갈 경우 리턴시켜서 중지
                            }
                        }
                    }

                    Destroy(objNodeList[i].obj);
                    objNodeList.RemoveAt(i);

                    //건물이 여러개 있어 리스트 인덱스가 복수로 존재하면 건물 삭제 후 비어진 리스트까지 탐지해버림
                    return; //리스트 최신화로 인한 오류 방지 ==> 구조상 한번에 2개를 파괴 시키지 않음
                }
            }
        }
    }

    public void P_GetObjNode(GameObject _obj, Tuple<Vector2Int, Vector2Int> _range)
    {
        objNodeList.Add(new ObjectNode(_obj, _range));
    }
}
