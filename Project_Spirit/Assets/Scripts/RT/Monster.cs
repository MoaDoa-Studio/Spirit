using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int monsterID; //몬스터 ID
    public string monsterName; //몬스터 이름
    public int monsterHealth; //몬스터 체력
    public float monsterSpeed; //설정할 몬스터 이동 속도
    public Tuple<int, int> mosterSize; //몬스터 크기(x, y)
    public string mosterDescription; //몬스터 설명
    public int monsterSkill01; //몬스터 스킬 1
    public int monsterSkill02; //몬스터 스킬 2

    public float curMoveSpeed; //현재 이동 속도

    public enum MonsterSize
    {
        Normal, //3X3
        Boss    //7X7
    }

    public MonsterSize monsterSize; //몬스터 종류 설정하고 크기 결정
    //public int size;

    private void OnTriggerEnter(Collider collision) //캡슐 콜리더 사용
    {
        //만약 부딛힌 오브젝트가 정령일 경우
        if (collision.tag == "Spirit")
        {
            Spirit sc = collision.GetComponent<Spirit>(); //정령 스크립트 가져오기

            monsterHealth -= (int)sc.HP; //정령의 체력만큼 몬스터의 체력 감소

            if (sc.SpiritJob == -1) //추후 직업 번호 확인을 하여 수정
            {
                curMoveSpeed = monsterSpeed / 2; //적용되는 이동속도 = 설정한 이동 속도 / 2
                
                //근데 영구적인 것인지 일정 시간만 적용되는 것인지를 모름
            }
        }
    }

    private void Awake()
    {
        curMoveSpeed = monsterSpeed; //현재 이동 속도를 몬스터 기초 설정 속도로 적용
    }
}
