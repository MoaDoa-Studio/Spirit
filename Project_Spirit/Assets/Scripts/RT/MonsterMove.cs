using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    [SerializeField] private Monster monster;
    [SerializeField] private MonsterTileMapManager monsterTileMapManager;

    public enum MoveDirection //enum으로 방향을 나타내며 enum에 따라서 타일 탐지 결정할 예정
    {
        Stop = 0, //혹시 멈추는 기믹을 위해 임시로 넣음
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    [SerializeField, Tooltip("일단 방향 확인용으로 사용")] private MoveDirection moveDirection; //몬스터 이동 방향

    [Header("몬스터 루트 설정")]
    [SerializeField] private Transform monsterRootParent;
    public List<Transform> monsterRootList = new List<Transform>();

    [Header("목표 설정")]
    public List<Vector3Int> targetList;
    [SerializeField] private Vector3Int target;

    [SerializeField] private Vector3Int nullTarget = new Vector3Int(-1, -1, 0);
    private int targetNum = 0;

    [Header("타일 사이즈 결정")]
    public int size;

    [Header("타일 및 이동 제어 트리거")]
    public bool setRedTile = false;
    [SerializeField] private bool setEnd; //목표에 도착하면 코드 막기

    public enum MoveType
    {
        wait,
        start,
        end
    }

    [SerializeField] private MoveType moveType;

    private void Start()
    {
        monster = GetComponent<Monster>();
        monsterTileMapManager = MonsterTileMapManager.instance;

        SetRootList();

        target = nullTarget; //-1, -1을 넣어 빈 타겟으로 지정 
        SetRedTiles();
    }

    private void Update()
    {
        SelectMoveType();   
    }

    private void SetRootList()
    {
        //자식 오브젝트의 수 만큼 설정
        int count = monsterRootParent.childCount;

        for (int i = 0; i < count; i++)
        {
            monsterRootList.Add(monsterRootParent.GetChild(i)); //인덱스 순차적으로 추가
        }
    }

    /// <summary>
    /// 몬스터가 소환된 후 타일 깔기
    /// </summary>
    private void SetRedTiles()
    {
        StartCoroutine(monsterTileMapManager.PC_OnRoadTileSet(this)); //타일 배치
    }

    /// <summary>
    /// 목표 셋팅
    /// </summary>
    private void SetTargets()
    {
        Debug.Log("목표 지점 설정");

        target = targetList[0]; //타겟을 첫 좌표로 지정
        monsterTileMapManager.P_SetDeadTile(target.x, target.y, size); //지정하자마자 검은 타일로 변경
    }

    private void SelectMoveType()
    {
        if (setEnd) { return; }

        switch (moveType) //몬스터의 움직임 상태를 체크하며 적용
        {
            case MoveType.wait: //몬스터가 이동 대기 상태일 때
                //대기 상태에서 타일이 순차적으로 다 깔릴 경우 몬스터가 움직이기 시작하며 타겟설정
                if (setRedTile)
                {
                    moveType = MoveType.start; //이동 모드로 설정
                    SetTargets(); //타겟 설정
                }
                break;

            case MoveType.start: //몬스터가 움직이기 시작할 때
                MonsterMoveOnRoad(); //몬스터가 길 따라 이동(상시 내용은 코드 확인)
                break;

            case MoveType.end: //몬스터가 최종 목적지에 도착할 경우
                setEnd = true; //이동 관련 코드 멈추게 하기
                break;
        }
    }

    /// <summary>
    /// 몬스터 이동 함수
    /// </summary>
    private void MonsterMoveOnRoad()
    {
        SetMoveDirection(); //몬스터 이동 방향 표시

        if (moveDirection != MoveDirection.Stop) //만약 이동 방향이 스탑상태가 아닐 경우 이동
        {
            //타겟 좌표로 이동
            transform.position = Vector3.MoveTowards(transform.position, target, monster.curMoveSpeed * Time.deltaTime);
        }

        //거리 계산
        float distance = Vector3.Distance(transform.position, target);

        //목표와의 거리가 가까우면
        if (distance == 0) //MoveTowards 코드는 지정한 좌표까지 완벽하게 이동하는 코드이고 int로 판정을 하기 때문에 0만 인식
        {
            //타일을 다 밟은 후 검은 타일로 변경
            //monsterTileMapManager.P_SetDeadTile(target.x, target.y);

            monsterTileMapManager.P_CheckPassObject(target.x, target.y, size); //몬스터가 건물 타일을 완전히 지나쳤는지 확인

            targetNum++; //다음 목표 번호

            if (targetNum <= targetList.Count - 1) //다음 목표가 있을 경우
            {
                target = targetList[targetNum]; //다음 목표 설정

                //다음 타일을 밟기 시작할 때 검은 타일로 변경
                monsterTileMapManager.P_SetDeadTile(target.x, target.y, size);

            }

            else //끝까지 갔으면
            {
                target = nullTarget; //타겟 비우기
                Debug.Log("이동을 멈춥니다.");
                moveType = MoveType.end; //도착 판정
            }
        }
    }

    /// <summary>
    /// 몬스터의 좌표와 목표 좌표에 따라 이동방향 결정
    /// </summary>
    private void SetMoveDirection()
    {
        //목표와의 위치가 x축이 같고 y축이 목표보다 낮을 경우
        if (transform.position.x == target.x && transform.position.y < target.y)
        {
            moveDirection = MoveDirection.Up;
        }

        //목표와의 위치가 x축이 같고 y축이 목표보다 높을 경우
        else if (transform.position.x == target.x && transform.position.y > target.y)
        {
            moveDirection = MoveDirection.Down;
        }

        //목표와의 위치가 x축이 목표보다 오른쪽에 있고 y축이 같을 경우
        else if (transform.position.x > target.x && transform.position.y == target.y)
        {
            moveDirection = MoveDirection.Left;
        }

        //목표와의 위치가 x축이 목표보다 왼쪽에 있고 y축이 같을 경우
        else if (transform.position.x < target.x && transform.position.y == target.y)
        {
            moveDirection = MoveDirection.Right;
        }

        else //이도저도 아닌 상황일 때 Stop판정
        {
            moveDirection = MoveDirection.Stop;
        }
    }
}
