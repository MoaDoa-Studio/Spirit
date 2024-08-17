using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

partial class ResearchManager : MonoBehaviour
{
    [Header("오브젝트")]
    [SerializeField]
    private GameObject Research_UI;    
    [SerializeField]
    private GameObject StudyDetail;
    [SerializeField]
    private GameObject StudyProgress;
    [SerializeField]
    private GameObject StudyComplete;
    [SerializeField]
    private GameObject[] Tree;
    [SerializeField]
    private GameObject[] Blurry;
    [SerializeField]
    private GameObject[] StepButton;
  
    public GameObject gainWorkUI;
    private GameObject currentClickedObj;
    private Study currentStudy;
    public int currentWork;
    private bool inProgress;

    private void Start()
    {
        inProgress = false;
        currentStudy = null;        
    }
    
    // 연구소 UI 띄워주는 함수.
    public void ShowResearchUI()
    {
        if (inProgress)
            UpdateStudyProgress();
        Research_UI.SetActive(true);
    }

    Study GetStudy(int StudyID)
    {
        if (!DatabaseManager.instance.Studies.ContainsKey(StudyID))
            return null;
        return DatabaseManager.instance.Studies[StudyID];
    }    

    public void OnClickStudyButton(int StudyID)
    {
        Study _study = GetStudy(StudyID);                
        if (_study.isComplete)
            return;
        
        currentClickedObj = EventSystem.current.currentSelectedGameObject;
        SetStudyDetail(_study);
        StudyDetail.SetActive(true);
    }

    void SetStudyDetail(Study _study)
    {        
        TextMeshProUGUI Detail_StudyName = StudyDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI Detail_Explain = StudyDetail.transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        Button Detail_StartButton = StudyDetail.transform.GetChild(4).GetComponent<Button>();        

        Detail_StudyName.text = _study.StudyName;
        Detail_Explain.text = _study.StudyContent;
        Detail_StartButton.gameObject.SetActive(true);
        Detail_StartButton.onClick.RemoveAllListeners();
        Detail_StartButton.onClick.AddListener(() => OnClickResearchStartButton(_study));        

        StudyDetail.SetActive(true);
        StudyProgress.SetActive(false);        
        //if (_study.WoodRequire < (현재 나무) || _study.StoneRequire < (현재 돌))
        // StartBtn.interactable = false;               
    }

    void UpdateStudyProgress()
    {                
        Slider ProgressSlider = StudyProgress.transform.GetChild(3).GetComponent<Slider>();
        
        ProgressSlider.gameObject.SetActive(true);
        ProgressSlider.value = (float)currentWork / currentStudy.WorkRequirement;                

        StudyDetail.SetActive(false);
        StudyProgress.SetActive(true);
    }
    
    public void OnClickResearchStartButton(Study _study)
    {
        inProgress = true;
        currentStudy = _study;
        currentWork = 0;                

        UpdateStudyProgress();
    }

    public void CompleteStudy()
    {
        currentStudy.isComplete = true;

        ApplyStudyEffect();                
        ShowStudyComplete();

        currentClickedObj.GetComponent<Button>().interactable = false;        

        currentStudy = null;
        currentClickedObj = null;
        inProgress = false;        
    }

    public void ShowStudyComplete()
    {
        StudyComplete.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = currentStudy.StudyName;
        StudyComplete.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = currentStudy.StudyContent;

        StudyComplete.SetActive(true);
        StudyDetail.SetActive(false);
        StudyProgress.SetActive(false);
    }
    
    #region For Debug    
    // 정령이 연구소에서 일할 때 호출 될 함수 로직.
    public void OnClickWork()
    {        
        currentWork += 2000;
        UpdateStudyProgress();
        if (currentWork >= currentStudy.WorkRequirement)
        {
            currentWork = 0;            
            CompleteStudy();            
        }
    }
    #endregion
}

partial class ResearchManager
{
    [Header("효과 적용 오브젝트")]
    [SerializeField]
    private GameObject[] StoneFactory_Button;
    [SerializeField]
    private GameObject[] WoodFactory_Button;
    [SerializeField]
    private GameObject[] StoneStorage_Button;
    [SerializeField]
    private GameObject[] WoodStorage_Button;
    [SerializeField]
    private GameObject KnightTraining;
    [SerializeField]
    private GameObject LeaderTraining;
    [SerializeField]
    private GameObject WoodRoad;
    [SerializeField]
    private GameObject SpawnerSpiritUpgradeButton;

    [Header("효과 적용 스크립트")]
    [SerializeField]
    private SpiritSpawner[] spiritSpawner;    
    [SerializeField]
    private ResouceManager resourceManager;
    // 각 연구별 효과 클래스.
    public void ApplyStudyEffect()
    {
        switch(currentStudy.StudyID)
        {
            case 1001: // 건물 1단계 해금                
                Blurry[0].transform.GetChild(0).gameObject.SetActive(false);
                StepButton[0].transform.GetChild(1).GetComponent<Button>().interactable = true;
                break;
            case 1002: // 건물 2단계 해금
                Blurry[0].transform.GetChild(1).gameObject.SetActive(false);
                StepButton[0].transform.GetChild(2).GetComponent<Button>().interactable = true;
                break;
            case 1003: // 건물 3단계 해금
                Blurry[0].transform.GetChild(2).gameObject.SetActive(false);
                StepButton[0].transform.GetChild(3).GetComponent<Button>().interactable = true;
                break;
            case 1004: // 건물 4단계 해금
                Blurry[0].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1011: // 돌 생산소 1 생산 가능.
                StoneFactory_Button[0].SetActive(true);
                Tree[0].transform.Find("Studies/Step2/StoneFactory2").GetComponent<Button>().interactable = true;
                break;
            case 1012: // 나무 생산소 1 생산 가능.
                WoodFactory_Button[0].SetActive(true);
                Tree[0].transform.Find("Studies/Step2/WoodFactory2").GetComponent<Button>().interactable = true;
                break;
            case 1013: // 평범한 돌 저장소 생산 가능.
                StoneStorage_Button[0].SetActive(true);
                break;
            case 1014: // 평범한 나무 저장소 생산 가능.
                WoodStorage_Button[0].SetActive(true);
                break;
            case 1021: // 기사 훈련소
                KnightTraining.SetActive(true);
                break;
            case 1022: // 장사 훈련소
                LeaderTraining.SetActive(true);
                break;
            case 1023: // 돌 생산소 2
                StoneFactory_Button[1].SetActive(true);
                break;
            case 1024: // 나무 생산소 2
                WoodFactory_Button[1].SetActive(true);
                break;
            case 1025: // 나무 길
                WoodRoad.SetActive(true);
                break;

            case 1101: // 정령 1단계 해금
                Blurry[1].transform.GetChild(0).gameObject.SetActive(false);
                StepButton[1].transform.GetChild(1).GetComponent<Button>().interactable = true;
                break;
            case 1102: // 정령 2단계 해금
                Blurry[1].transform.GetChild(1).gameObject.SetActive(false);
                StepButton[1].transform.GetChild(2).GetComponent<Button>().interactable = true;
                break;
            case 1103: // 정령 3단계 해금
                Blurry[1].transform.GetChild(2).gameObject.SetActive(false);
                StepButton[1].transform.GetChild(3).GetComponent<Button>().interactable = true;
                break;
            case 1104: // 정령 4단계 해금
                Blurry[1].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1111: // 정령 스폰시간 단축 1 (미구현)                
                for(int i = 0; i < 4; i++)
                {
                    spiritSpawner[i].spawnWeight = 1.2f;
                }
                break;
            case 1112: // 정령 이동 속도 상승 1
                SpiritManager.instance.spiritMoveSpeed = 1.03f;
                SpiritManager.instance.ChangeSpiritSpeed(1.03f);
                Tree[1].transform.Find("Studies/Step2/Speed2").GetComponent<Button>().interactable = true;
                break;
            case 1121: // 정령 단계 업그레이드 1
                SpawnerSpiritUpgradeButton.GetComponent<Button>().interactable = true;
                break;
            case 1122: // 정령 이동 속도 상승 2
                SpiritManager.instance.spiritMoveSpeed = 1.06f;
                SpiritManager.instance.ChangeSpiritSpeed(1.06f);
                break;

            case 1201: // 자원 1단계 해금
                Blurry[2].transform.GetChild(0).gameObject.SetActive(false);
                StepButton[2].transform.GetChild(1).GetComponent<Button>().interactable = true;
                break;
            case 1202: // 자원 2단계 해금
                Blurry[2].transform.GetChild(1).gameObject.SetActive(false);
                StepButton[2].transform.GetChild(2).GetComponent<Button>().interactable = true;
                break;
            case 1203: // 자원 3단계 해금
                Blurry[2].transform.GetChild(2).gameObject.SetActive(false);
                StepButton[2].transform.GetChild(3).GetComponent<Button>().interactable = true;
                break;
            case 1204: // 자원 4단계 해금
                Blurry[2].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1211: // 자원 자연 생산량 증가 1
                resourceManager.IncreaseResourceWeight();
                Tree[2].transform.Find("Studies/Step2/IncreaseResource2").GetComponent<Button>().interactable = true;
                break;
            case 1212: // 자원 채집 환경 개선 1
                SpiritManager.instance.resourceBuildingDamagePercent = 0.85f;
                Tree[2].transform.Find("Studies/Step2/ImproveResource2").GetComponent<Button>().interactable = true;
                break;
            case 1222: // 자원 자연 생산량 증가 2
                resourceManager.IncreaseResourceWeight();                
                break;
            case 1223: // 자원 채집 환경 개선 2
                SpiritManager.instance.resourceBuildingDamagePercent = 0.9f;
                break;
        }
    }
}