using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int monsterID; //���� ID
    public string monsterName; //���� �̸�
    public int monsterHealth; //���� ü��
    public float monsterSpeed; //������ ���� �̵� �ӵ�
    public Tuple<int, int> mosterSize; //���� ũ��(x, y)
    public string mosterDescription; //���� ����
    public int monsterSkill01; //���� ��ų 1
    public int monsterSkill02; //���� ��ų 2

    public float curMoveSpeed; //���� �̵� �ӵ�

    public enum MonsterSize
    {
        Normal, //3X3
        Boss    //7X7
    }

    public MonsterSize monsterSize; //���� ���� �����ϰ� ũ�� ����
    //public int size;

    private void OnTriggerEnter(Collider collision) //ĸ�� �ݸ��� ���
    {
        //���� �ε��� ������Ʈ�� ������ ���
        if (collision.tag == "Spirit")
        {
            Spirit sc = collision.GetComponent<Spirit>(); //���� ��ũ��Ʈ ��������

            monsterHealth -= (int)sc.HP; //������ ü�¸�ŭ ������ ü�� ����

            if (sc.SpiritJob == -1) //���� ���� ��ȣ Ȯ���� �Ͽ� ����
            {
                curMoveSpeed = monsterSpeed / 2; //����Ǵ� �̵��ӵ� = ������ �̵� �ӵ� / 2
                
                //�ٵ� �������� ������ ���� �ð��� ����Ǵ� �������� ��
            }
        }
    }

    private void Awake()
    {
        curMoveSpeed = monsterSpeed; //���� �̵� �ӵ��� ���� ���� ���� �ӵ��� ����
    }
}
