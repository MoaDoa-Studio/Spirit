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

    [SerializeField] private GameObject prfMonster01; //소환된 몬스터01
    [SerializeField] private GameObject prfMonster02; //소환된 몬스터02
    [SerializeField] private GameObject prfMonsterBoss; //소환된 보스 몬스터

    [SerializeField] private Transform parentObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (prfMonster01 == null) //몬스터가 존재하지 않을 경우 소환
            {
                prfMonster01 = Instantiate(monsterSpawnList[0].objMonster, monsterSpawnList[0].spawnPoint.position, Quaternion.identity, parentObject);
            }

            else if (prfMonster01 != null) //몬스터가 존재할 경우 지우기
            {
                Destroy(prfMonster01);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (prfMonster02 == null) //몬스터가 존재하지 않을 경우 소환
            {
                prfMonster02 = Instantiate(monsterSpawnList[1].objMonster, monsterSpawnList[1].spawnPoint.position, Quaternion.identity, parentObject);
            }

            else if (prfMonster02 != null) //몬스터가 존재할 경우 지우기
            {
                Destroy(prfMonster02);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (prfMonsterBoss == null) //몬스터가 존재하지 않을 경우 소환
            {
                prfMonsterBoss = Instantiate(monsterSpawnList[2].objMonster, monsterSpawnList[2].spawnPoint.position, Quaternion.identity, parentObject);
            }

            else if (prfMonsterBoss != null) //몬스터가 존재할 경우 지우기
            {
                Destroy(prfMonsterBoss);
            }
        }
    }
}
