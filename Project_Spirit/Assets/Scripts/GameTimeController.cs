using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTimeController : MonoBehaviour
{
    // 게임 내 시간과 현실 시간 간의 비율
    public float gameSecondsPerReal = 12f * 60f;

    public float gameTimer = 0f;
    // Update is called once per frame
    void Update()
    {
        gameTimer += Time.deltaTime * gameSecondsPerReal;
    }
}
