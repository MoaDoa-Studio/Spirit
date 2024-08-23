using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    [SerializeField] private Monster monster;
    [SerializeField] private MonsterTileMapManager monsterTileMapManager;

    public enum MoveDirection //enum���� ������ ��Ÿ���� enum�� ���� Ÿ�� Ž�� ������ ����
    {
        Stop = 0, //Ȥ�� ���ߴ� ����� ���� �ӽ÷� ����
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    [SerializeField, Tooltip("�ϴ� ���� Ȯ�ο����� ���")] private MoveDirection moveDirection; //���� �̵� ����

    [Header("���� ��Ʈ ����")]
    [SerializeField] private Transform monsterRootParent;
    public List<Transform> monsterRootList = new List<Transform>();

    [Header("��ǥ ����")]
    public List<Vector3Int> targetList;
    [SerializeField] private Vector3Int target;

    [SerializeField] private Vector3Int nullTarget = new Vector3Int(-1, -1, 0);
    private int targetNum = 0;

    [Header("Ÿ�� ������ ����")]
    public int size;

    [Header("Ÿ�� �� �̵� ���� Ʈ����")]
    public bool setRedTile = false;
    [SerializeField] private bool setEnd; //��ǥ�� �����ϸ� �ڵ� ����

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

        target = nullTarget; //-1, -1�� �־� �� Ÿ������ ���� 
        SetRedTiles();
    }

    private void Update()
    {
        SelectMoveType();   
    }

    private void SetRootList()
    {
        //�ڽ� ������Ʈ�� �� ��ŭ ����
        int count = monsterRootParent.childCount;

        for (int i = 0; i < count; i++)
        {
            monsterRootList.Add(monsterRootParent.GetChild(i)); //�ε��� ���������� �߰�
        }
    }

    /// <summary>
    /// ���Ͱ� ��ȯ�� �� Ÿ�� ���
    /// </summary>
    private void SetRedTiles()
    {
        StartCoroutine(monsterTileMapManager.PC_OnRoadTileSet(this)); //Ÿ�� ��ġ
    }

    /// <summary>
    /// ��ǥ ����
    /// </summary>
    private void SetTargets()
    {
        Debug.Log("��ǥ ���� ����");

        target = targetList[0]; //Ÿ���� ù ��ǥ�� ����
        monsterTileMapManager.P_SetDeadTile(target.x, target.y, size); //�������ڸ��� ���� Ÿ�Ϸ� ����
    }

    private void SelectMoveType()
    {
        if (setEnd) { return; }

        switch (moveType) //������ ������ ���¸� üũ�ϸ� ����
        {
            case MoveType.wait: //���Ͱ� �̵� ��� ������ ��
                //��� ���¿��� Ÿ���� ���������� �� �� ��� ���Ͱ� �����̱� �����ϸ� Ÿ�ټ���
                if (setRedTile)
                {
                    moveType = MoveType.start; //�̵� ���� ����
                    SetTargets(); //Ÿ�� ����
                }
                break;

            case MoveType.start: //���Ͱ� �����̱� ������ ��
                MonsterMoveOnRoad(); //���Ͱ� �� ���� �̵�(��� ������ �ڵ� Ȯ��)
                break;

            case MoveType.end: //���Ͱ� ���� �������� ������ ���
                setEnd = true; //�̵� ���� �ڵ� ���߰� �ϱ�
                break;
        }
    }

    /// <summary>
    /// ���� �̵� �Լ�
    /// </summary>
    private void MonsterMoveOnRoad()
    {
        SetMoveDirection(); //���� �̵� ���� ǥ��

        if (moveDirection != MoveDirection.Stop) //���� �̵� ������ ��ž���°� �ƴ� ��� �̵�
        {
            //Ÿ�� ��ǥ�� �̵�
            transform.position = Vector3.MoveTowards(transform.position, target, monster.curMoveSpeed * Time.deltaTime);
        }

        //�Ÿ� ���
        float distance = Vector3.Distance(transform.position, target);

        //��ǥ���� �Ÿ��� ������
        if (distance == 0) //MoveTowards �ڵ�� ������ ��ǥ���� �Ϻ��ϰ� �̵��ϴ� �ڵ��̰� int�� ������ �ϱ� ������ 0�� �ν�
        {
            //Ÿ���� �� ���� �� ���� Ÿ�Ϸ� ����
            //monsterTileMapManager.P_SetDeadTile(target.x, target.y);

            monsterTileMapManager.P_CheckPassObject(target.x, target.y, size); //���Ͱ� �ǹ� Ÿ���� ������ �����ƴ��� Ȯ��

            targetNum++; //���� ��ǥ ��ȣ

            if (targetNum <= targetList.Count - 1) //���� ��ǥ�� ���� ���
            {
                target = targetList[targetNum]; //���� ��ǥ ����

                //���� Ÿ���� ��� ������ �� ���� Ÿ�Ϸ� ����
                monsterTileMapManager.P_SetDeadTile(target.x, target.y, size);

            }

            else //������ ������
            {
                target = nullTarget; //Ÿ�� ����
                Debug.Log("�̵��� ����ϴ�.");
                moveType = MoveType.end; //���� ����
            }
        }
    }

    /// <summary>
    /// ������ ��ǥ�� ��ǥ ��ǥ�� ���� �̵����� ����
    /// </summary>
    private void SetMoveDirection()
    {
        //��ǥ���� ��ġ�� x���� ���� y���� ��ǥ���� ���� ���
        if (transform.position.x == target.x && transform.position.y < target.y)
        {
            moveDirection = MoveDirection.Up;
        }

        //��ǥ���� ��ġ�� x���� ���� y���� ��ǥ���� ���� ���
        else if (transform.position.x == target.x && transform.position.y > target.y)
        {
            moveDirection = MoveDirection.Down;
        }

        //��ǥ���� ��ġ�� x���� ��ǥ���� �����ʿ� �ְ� y���� ���� ���
        else if (transform.position.x > target.x && transform.position.y == target.y)
        {
            moveDirection = MoveDirection.Left;
        }

        //��ǥ���� ��ġ�� x���� ��ǥ���� ���ʿ� �ְ� y���� ���� ���
        else if (transform.position.x < target.x && transform.position.y == target.y)
        {
            moveDirection = MoveDirection.Right;
        }

        else //�̵����� �ƴ� ��Ȳ�� �� Stop����
        {
            moveDirection = MoveDirection.Stop;
        }
    }
}
