using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [Serializable]
    public class MonsterSpawnData
    {
        public GameObject objMonster;
        public Transform spawnPoint;
    }

    [SerializeField] private List<MonsterSpawnData> monsterSpawnList;

    [SerializeField] private GameObject prfMonster01; //��ȯ�� ����01
    [SerializeField] private GameObject prfMonster02; //��ȯ�� ����02
    [SerializeField] private GameObject prfMonsterBoss; //��ȯ�� ���� ����

    [SerializeField] private Transform parentObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (prfMonster01 == null) //���Ͱ� �������� ���� ��� ��ȯ
            {
                prfMonster01 = Instantiate(monsterSpawnList[0].objMonster, monsterSpawnList[0].spawnPoint.position, Quaternion.identity, parentObject);
            }

            else if (prfMonster01 != null) //���Ͱ� ������ ��� �����
            {
                Destroy(prfMonster01);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (prfMonster02 == null) //���Ͱ� �������� ���� ��� ��ȯ
            {
                prfMonster02 = Instantiate(monsterSpawnList[1].objMonster, monsterSpawnList[1].spawnPoint.position, Quaternion.identity, parentObject);
            }

            else if (prfMonster02 != null) //���Ͱ� ������ ��� �����
            {
                Destroy(prfMonster02);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (prfMonsterBoss == null) //���Ͱ� �������� ���� ��� ��ȯ
            {
                prfMonsterBoss = Instantiate(monsterSpawnList[2].objMonster, monsterSpawnList[2].spawnPoint.position, Quaternion.identity, parentObject);
            }

            else if (prfMonsterBoss != null) //���Ͱ� ������ ��� �����
            {
                Destroy(prfMonsterBoss);
            }
        }
    }
}
