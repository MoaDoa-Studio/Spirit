using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Resource;
    [SerializeField]
    private GameObject[] Steps;

    // 스크립트 변수.
    private bool inProgress;
    private Dictionary<string, bool> Research = new Dictionary<string, bool>();
    
    private void Start()
    {
        // For Debug.
        inProgress = false;

    }
    private void OnEnable()
    {
        SetInfo();
    }
    private void SetInfo()
    {
        // 연구실 페이지 켤 때 갱신할 정보.
        // 1. 상단 UI의 재료 정보.
        // 2. 블러리 스크린
        // 3. 스텝 업 버튼 enable;
        // + 연구가 진행 중인지.


    }

    private void SetBlurryScreen()
    {                    
    }
}