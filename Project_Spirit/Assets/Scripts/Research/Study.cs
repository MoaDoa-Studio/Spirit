using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Study : MonoBehaviour
{
    // CSV에서 읽어 올 파일.
    public int StudyID;
    public string StudyName;
    public bool KindofStudy;
    public int CategoryofStudy;
    public string StudyContent;
    public int PhaseofStudy;
    public int StoneRequirement;
    public int WoodRequirement;
    public int EssenceRequirement;
    public int WorkRequirement;
    public int StudyEffect;
    public int PriorResearch;

    // 게임 진행하면서 변하는 가변 데이터
    public bool isComplete;

    private void Start()
    {
        isComplete = false;
    }
}
