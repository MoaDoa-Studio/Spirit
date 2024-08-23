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
    public int[,] tileArray = new int[103, 103]; //��ǥ ���� int�� �־� Ÿ�� ���� ����

    public Tilemap monsterTileMap; //���� ����� Ÿ�ϸ�

    public Vector2Int position;
    public Vector2Int bottomLeft, topRight, startPos, targetPos;

    public int sizeX, sizeY;

    private TileDataManager tileDataManager;

    [Header("���� Ÿ�� ��ǥ ����")]
    public List<ObjectNode> objNodeList = new List<ObjectNode>();

    [Header("���� �̵� ��� ���� Ÿ��")]
    [SerializeField] private TileBase monsterRoadTile; //���� �̵� ��� ��Ÿ���� Ÿ��
    [SerializeField] private TileBase deadTile; //���� Ÿ��

    [Header("�� ���� ��� Ÿ�� ����")]
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
    /// x, y��ǥ ��� 0 ~ 103 ������ ���ڸ� ����
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool isRange(int x, int y)
    {
        return x >= 0 && x < 103 && y >= 0 && y < 103;
    }

    /// <summary>
    /// Ÿ�� �ʱ�ȭ(��X) ==> ���߿� ��� Ÿ�� �ʱ�ȭ ��� ����� ���� �� ����
    /// </summary>
    private void OffRoadTileSet()
    {
        //�����ִ� ��� Ÿ�� ���� ��� ==> ���߿� ����
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
    /// ���� �ð� ���� Ÿ�� ������ ���� �߰�
    /// </summary>
    public IEnumerator PC_OnRoadTileSet(MonsterMove sc)
    {
        int count = sc.monsterRootList.Count;

        for (int iNum01 = 0; iNum01 < count; iNum01++)
        {
            if (iNum01 != count - 1)
            {
                float curPositionX = sc.monsterRootList[iNum01].position.x; //���� ��ǥ�� x��
                float nextPositionX = sc.monsterRootList[iNum01 + 1].position.x; //���� ��ǥ�� x��
                float curPositionY = sc.monsterRootList[iNum01].position.y; //���� ��ǥ�� y��
                float nextPositionY = sc.monsterRootList[iNum01 + 1].position.y; //���� ��ǥ�� y��

                bool xLine = (curPositionY == nextPositionY) ? true : false; //����� ���� ��ǥ�� y���� ��ġ�ϸ� x�� �̵�

                if (xLine) //x�� �̵��� ��
                {
                    if (curPositionX < nextPositionX) //�̵� ������ ������ ���
                    {
                        for (int iNum02 = (int)curPositionX; iNum02 < (int)nextPositionX; iNum02++) //x++
                        {
                            yield return new WaitForSeconds(waitSecond);

                            Vector3Int setX = new Vector3Int(iNum02, (int)curPositionY, 0); //y�� �����ϸ� ��ġ

                            //monsterTileMap.SetTile(setX, monsterRoadTile); //���� Ÿ�� ��ġ
                            SetRoadSquareTile(setX, sc.size);

                            sc.targetList.Add(setX); //�� Ÿ�� ��ǥ�� ���� ��� ��ũ��Ʈ�� ����
                        }
                    }

                    else if (curPositionX > nextPositionX) //�̵� ������ ������ ���
                    {
                        for (int iNum02 = (int)curPositionX; iNum02 > (int)nextPositionX; iNum02--) //x--
                        {
                            yield return new WaitForSeconds(waitSecond);

                            Vector3Int setX = new Vector3Int(iNum02, (int)curPositionY, 0); //y�� �����ϸ� ��ġ

                            //monsterTileMap.SetTile(setX, monsterRoadTile); //���� Ÿ�� ��ġ
                            SetRoadSquareTile(setX, sc.size);

                            sc.targetList.Add(setX); //�� Ÿ�� ��ǥ�� ���� ��� ��ũ��Ʈ�� ����
                        }
                    }
                }

                else //!xLine == y�� �̵��� ��
                {
                    if (curPositionY < nextPositionY) //�̵� ������ ���� ���
                    {
                        for (int iNum02 = (int)curPositionY; iNum02 < (int)nextPositionY; iNum02++) //y++
                        {
                            yield return new WaitForSeconds(waitSecond);

                            Vector3Int setY = new Vector3Int((int)curPositionX, iNum02, 0); //x�� �����ϸ� ��ġ

                            //monsterTileMap.SetTile(setY, monsterRoadTile); //���� Ÿ�� ��ġ
                            SetRoadSquareTile(setY, sc.size);

                            sc.targetList.Add(setY); //�� Ÿ�� ��ǥ�� ���� ��� ��ũ��Ʈ�� ����
                        }
                    }

                    else if (curPositionY > nextPositionY) //�̵� ������ �Ʒ��� ���
                    {
                        for (int iNum02 = (int)curPositionY; iNum02 > (int)nextPositionY; iNum02--) //y--
                        {
                            yield return new WaitForSeconds(waitSecond);

                            Vector3Int setY = new Vector3Int((int)curPositionX, iNum02, 0); //x�� �����ϸ� ��ġ

                            //monsterTileMap.SetTile(setY, monsterRoadTile); //���� Ÿ�� ��ġ
                            SetRoadSquareTile(setY, sc.size);

                            sc.targetList.Add(setY); //�� Ÿ�� ��ǥ�� ���� ��� ��ũ��Ʈ�� ����
                        }
                    }
                }
            }

            else //iNum01 == count - 1
            {
                //������ ��ǥ ���
                int x = (int)sc.monsterRootList[iNum01].position.x;
                int y = (int)sc.monsterRootList[iNum01].position.y;

                yield return new WaitForSeconds(waitSecond);

                Vector3Int setFinalTile = new Vector3Int(x, y, 0);

                //monsterTileMap.SetTile(setFinalTile, monsterRoadTile); //���� Ÿ�� ��ġ
                SetRoadSquareTile(setFinalTile, sc.size);

                sc.targetList.Add(setFinalTile); //�� Ÿ�� ��ǥ�� ���� ��� ��ũ��Ʈ�� ����

                sc.setRedTile = true; //Ÿ�� ��ġ �Ϸ� ǥ��
            }
        }
    }

    /// <summary>
    /// ���簢�� Ÿ�� ��� �ڵ�
    /// </summary>
    /// <param name="_center"></param>
    /// <param name="_size"></param>
    private void SetRoadSquareTile(Vector3Int _center, int _size)
    {
        //���簢���� �������� ���
        int radius = _size / 2;

        //���簢���� ��ǥ�� �ݺ��Ͽ� Ÿ���� ��ġ
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                //���� Ÿ���� ��ǥ�� ���
                Vector3Int tilePosition = new Vector3Int(_center.x + x, _center.y + y, 0);

                //Ÿ���� Ÿ�ϸʿ� ��ġ
                monsterTileMap.SetTile(tilePosition, monsterRoadTile);
            }
        }
    }

    /// <summary>
    /// ���Ͱ� ���� Ÿ���� ���� Ÿ�Ϸ� ����
    /// �Ű����� (x��ǥ, y��ǥ)
    /// </summary>
    public void P_SetDeadTile(int _x, int _y, int _size)
    {
        Vector3Int setVector = new Vector3Int(_x, _y, 0);

        monsterTileMap.SetTile(setVector, deadTile); //_x, _y��ǥ Ÿ���� ���� Ÿ�Ϸ� ����
        
        //���簢���� �������� ���
        int radius = _size / 2;

        //���簢���� ��ǥ�� �ݺ��Ͽ� Ÿ���� ��ġ
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                //���� Ÿ���� ��ǥ�� ���
                Vector3Int tilePosition = new Vector3Int(setVector.x + x, setVector.y + y, 0);

                if (tileDataManager.GetTileType(tilePosition.x, tilePosition.y) == 1) //���� ���� Ÿ���� �ǹ��� ���
                {
                    Debug.Log($"�ǹ� Ÿ���� �н��ϴ�.");
                    //�ǹ� �б�
                    SearchObject(tilePosition);
                }

                //Ÿ���� Ÿ�ϸʿ� ��ġ
                monsterTileMap.SetTile(tilePosition, deadTile);

                if (tileDataManager.GetTileType(tilePosition.x, tilePosition.y) != 10) //���� Ÿ�� Ÿ���� �����Ǿ� ���� ������
                {
                    tileDataManager.SetTileType(tilePosition.x, tilePosition.y, 10); //Ÿ�� ����
                }
            }
        }

        //tileDataManager.SetTileType(_x, _y, 10); //_x, _y��ǥ Ÿ���� Ÿ���� 10(enum��ȣ)�� ����
        //�Ʒ��� ���� Ÿ�� Ÿ�� ���� Ȯ�ο� ==> 10���� ����Ǵ� ���� Ȯ��
        //Debug.Log($"{setVector}�� Ÿ�� : {tileDataManager.GetTileType(_x, _y)}");
    }

    private void SearchObject(Vector3Int _tile)
    {
        int count = objNodeList.Count;

        for (int i = 0; i < count; i++)
        {
            //���� ���� ����
            bool inUpperRightX = _tile.x <= objNodeList[i].range.Item1.x;
            bool inUpperRightY = _tile.y <= objNodeList[i].range.Item1.y;
            bool inBottomLeftX = _tile.x >= objNodeList[i].range.Item2.x;
            bool inBottomLeftY = _tile.y >= objNodeList[i].range.Item2.y;

            //���� Ÿ���� ���� �ȿ� ����
            if (inUpperRightX && inUpperRightY && inBottomLeftX && inBottomLeftY)
            {
                if (!objNodeList[i].isBreaking) //�ǹ��� ������� ���� ������ ��
                {
                    objNodeList[i].isBreaking = true; //�ǹ� ���� �������� �����

                    if(objNodeList[i].obj.gameObject.GetComponent<Building>().bomb)
                    {
                        // ���� �ִ� ...
                    }

                    //�ǹ� �ı� �ڵ� �ۼ��غ���
                    //Destroy(objNodeList[i].obj);
                    //objNodeList.RemoveAt(i);

                    return;
                }
            }
        }
    }

    /// <summary>
    /// ���Ͱ� �ǹ� Ÿ���� ������ �����ƴ��� üũ�ϴ� �Լ�
    /// �� �ڵ�� Ÿ�� �ϳ� �̵��� ������ �ߵ� �Ǵµ� �ǹ��� ���� ö�� �� ��� ����Ʈ�� �پ��� ������ �Ź� �ֽ�ȭ ��Ŵ
    /// </summary>
    public void P_CheckPassObject(int _x, int _y, int _size)
    {
        if (objNodeList.Count == 0) //��ġ�� �ǹ��� ������ ����
        {
            return;
        }

        else
        {
            int count = objNodeList.Count; //��� ����Ʈ �˻�

            for (int i = 0; i < count; i++)
            {
                if (objNodeList[i].isBreaking) //���� �μ����� �ִ� �ǹ��� ���
                {
                    //���簢���� �������� ���
                    int radius = _size / 2;

                    //���簢���� ��ǥ�� �ݺ��Ͽ� Ÿ���� ��ġ
                    for (int x = -radius; x <= radius; x++)
                    {
                        for (int y = -radius; y <= radius; y++)
                        {
                            Vector2Int searchVector = new Vector2Int(_x + x, _y + y); //Ž�� ��ǥ

                            bool inX = searchVector.x <= objNodeList[i].range.Item1.x && searchVector.x >= objNodeList[i].range.Item2.x;
                            //bool inBottomX = searchVector.x >= objNodeList[i].range.Item2.x;
                            bool inY = searchVector.y <= objNodeList[i].range.Item1.y && searchVector.y >= objNodeList[i].range.Item2.y;
                            //bool inBottomY = searchVector.y >= objNodeList[i].range.Item2.y;

                            if (inX && inY)
                            {
                                return; //�ϳ��� ���� �ȿ� �� ��� ���Ͻ��Ѽ� ����
                            }
                        }
                    }

                    Destroy(objNodeList[i].obj);
                    objNodeList.RemoveAt(i);

                    //�ǹ��� ������ �־� ����Ʈ �ε����� ������ �����ϸ� �ǹ� ���� �� ����� ����Ʈ���� Ž���ع���
                    return; //����Ʈ �ֽ�ȭ�� ���� ���� ���� ==> ������ �ѹ��� 2���� �ı� ��Ű�� ����
                }
            }
        }
    }

    public void P_GetObjNode(GameObject _obj, Tuple<Vector2Int, Vector2Int> _range)
    {
        objNodeList.Add(new ObjectNode(_obj, _range));
    }
}
