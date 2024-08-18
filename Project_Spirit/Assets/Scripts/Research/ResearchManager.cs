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
    private GameObject Research_prior;
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
    [SerializeField]
    private GameObject[] Flask_UI;
    [SerializeField]
    private GameObject currentClickedObj;
    [SerializeField]
    private bool inProgress;

    public GameObject gainWorkUI;
    public int currentWork;

    private GameObject StudyObject;
    private Study currentStudy;
    

    bool priorUI = false;
    int tabInt;
    int studyNum;
    private void Start()
    {
        inProgress = false;
        currentStudy = null;        
    }
    
    // 연구소 UI 띄워주는 함수.
    public void ShowResearchUI()
    {
        // 선행 UI 토글 Off.
        Research_prior.SetActive(false);

        if (inProgress)
            UpdateStudyProgress();
        Research_UI.SetActive(true);
    }
    public void ShowPriorUI()
    {
        Research_prior.SetActive(true);
        priorUI = true;

        // 연구 진행중일 상황
        if(inProgress)
        {
            Research_prior.transform.Find("Slider").gameObject.SetActive(true);
            Research_prior.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>($"pictogram 1/{currentStudy.StudyID}");
            Slider ProgressSlider = Research_prior.transform.GetChild(3).GetComponent<Slider>();

            ProgressSlider.gameObject.SetActive(true);
            ProgressSlider.value = (float)currentWork / currentStudy.WorkRequirement;
        }
        else
        {
            Research_prior.transform.Find("Slider").gameObject.SetActive(false);
            Research_prior.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>($"pictogram 1/button");
        }
    }

    private void Update()
    {
        if(priorUI)
        { if(Input.GetKeyDown(KeyCode.Escape))
            { Research_prior.SetActive(false);  priorUI = false; }
        }
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
        studyNum = StudyID;
        // 이미 연구중이라면, 다른 연구 시작 불가, 버튼 비활성화.
        if(inProgress)
        {
            StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = false;

            // 연구 하위 이미지 누끼 적용

        }
        else
        {
            // 연구 하위 이미지 누끼 적용 
         
            // 연구 하위 자원 소모 적용
            StudyDetail.transform.GetChild(4).transform.Find("wood_icon/Text").GetComponent<TextMeshProUGUI>().text = _study.WoodRequirement.ToString();
            StudyDetail.transform.GetChild(4).transform.Find("rock_icon/Text").GetComponent<TextMeshProUGUI>().text = _study.StoneRequirement.ToString();
            StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = true;

            // 자원 여유량 체크
            if (resourceManager.GetComponent<ResouceManager>().Timber_reserves - _study.WoodRequirement < 0)
            {
                StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = false;
                
            }
            if (resourceManager.GetComponent<ResouceManager>().Rock_reserves - _study.StoneRequirement < 0)
            {
                StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = false;
               
            }
        }

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

        StudyObject = currentClickedObj;

        StudyDetail.SetActive(true);
        StudyProgress.SetActive(false);        
        //if (_study.WoodRequire < (현재 나무) || _study.StoneRequire < (현재 돌))
       
        // 연구에서 쓰일 자원 소모
        if(resourceManager.GetComponent<ResouceManager>().Timber_reserves - _study.WoodRequirement < 0)
        {
            StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = false;
            return;
        }
        if(resourceManager.GetComponent<ResouceManager>().Rock_reserves - _study.StoneRequirement < 0)
        {
            StudyDetail.transform.GetChild(4).GetComponent<Button>().interactable = false;
            return ;
        }
        resourceManager.GetComponent<ResouceManager>().Timber_reserves -= _study.WoodRequirement;
        resourceManager.GetComponent<ResouceManager>().Rock_reserves -= _study.StoneRequirement;
                    
    }

    void UpdateStudyProgress()
    {                
        Slider ProgressSlider = StudyProgress.transform.GetChild(3).GetComponent<Slider>();
        
        ProgressSlider.gameObject.SetActive(true);
        ProgressSlider.value = (float)currentWork / currentStudy.WorkRequirement;                

        StudyDetail.SetActive(false);

        // 해당 창을 띄우는 것이 아닌 애니메이션이 출력되어야함
        SetFlaskObj();
        // StudyProgress.SetActive(true);


    }
    
    public void OnClickResearchStartButton(Study _study)
    {
        inProgress = true;
        currentStudy = _study;
        currentWork = 0;                

        // 연구에 필요한 자원이 차감됨.

        UpdateStudyProgress();
    }

    public void CompleteStudy()
    {
        currentStudy.isComplete = true;

        // 연구 애니메이션을 종료함
        for(int i = 0; i < 5; i++)
        Flask_UI[i].SetActive(false);

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
        currentWork += 100;
        UpdateStudyProgress();
        if (currentWork >= currentStudy.WorkRequirement)
        {
            currentWork = 0;            
            CompleteStudy();            
        }
    }

    // 연구소 UI 창에서 연구 트리를 설치할떄, 연구소 플라스크 띄울 오브젝트 지정하는 함수.
    private void SetFlaskObj()
    {
        for(int j = 0; j < 5; j++)
            Flask_UI[j].SetActive(false);

        switch (tabInt)
        {
            case 0:
                RectTransform currentRect = StudyObject.GetComponent<RectTransform>();
                RectTransform rect1 = Flask_UI[tabInt].GetComponent<RectTransform>();
                Flask_UI[tabInt].SetActive(true);
                rect1.anchoredPosition = new Vector2(currentRect.anchoredPosition.x + 50, currentRect.anchoredPosition.y + 70);
                break;
            case 1:
                RectTransform currentRect2 = StudyObject.GetComponent<RectTransform>();
                RectTransform rect2 = Flask_UI[tabInt].GetComponent<RectTransform>();
                Flask_UI[tabInt].SetActive(true);
                rect2.anchoredPosition = new Vector2(currentRect2.anchoredPosition.x + 50, currentRect2.anchoredPosition.y + 70);
                break;
            case 2:
                RectTransform currentRect3 = StudyObject.GetComponent<RectTransform>();
                RectTransform rect3 = Flask_UI[tabInt].GetComponent<RectTransform>();
                Flask_UI[tabInt].SetActive(true);
                rect3.anchoredPosition = new Vector2(currentRect3.anchoredPosition.x + 50, currentRect3.anchoredPosition.y + 70);
                break;
            case 3:
                RectTransform currentRect4 = StudyObject.GetComponent<RectTransform>();
                RectTransform rect4 = Flask_UI[tabInt].GetComponent<RectTransform>();
                Flask_UI[tabInt].SetActive(true);
                rect4.anchoredPosition = new Vector2(currentRect4.anchoredPosition.x + 50, currentRect4.anchoredPosition.y + 70);
                break;
            case 4:
                RectTransform currentRect5 = StudyObject.GetComponent<RectTransform>();
                RectTransform rect5 = Flask_UI[tabInt].GetComponent<RectTransform>();
                Flask_UI[tabInt].SetActive(true);
                rect5.anchoredPosition = new Vector2(currentRect5.anchoredPosition.x + 50, currentRect5.anchoredPosition.y + 70);
                break;
        }
       
      
    }

    public void SetTab(int idx)
    {
        switch (idx)
        {
            case 0:
               tabInt = 0;
                break;
            case 1:
                tabInt = 1;
                break;
            case 2:
                tabInt = 2;
              
                break;
            case 3:
                tabInt = 3;
              
                break;
            case 4:
                tabInt = 4;
              
                break;
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
    private GameObject HealTraining;
    [SerializeField]
    private GameObject HealFoundation;
    [SerializeField]
    private GameObject WoodRoad;
    [SerializeField]
    private GameObject SpawnerUI;
    [SerializeField]
    private GameObject SpawnerSpiritUpgradeButton;
    [SerializeField]
    private GameObject SpawnerSpiritSpawnUpgradeButton;
    [SerializeField]
    private GameObject[] LowUp;
    [SerializeField]
    private GameObject[] highUp;


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
                SpawnerSpiritSpawnUpgradeButton.GetComponent<Button>().interactable = true;
                SpawnerSpiritSpawnUpgradeButton.GetComponent<Image>().sprite = SpawnerUI.GetComponent<SpawnerUI>().UpgradeSprite[1];
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
            case 2001:  // 2단계 연구들을 잠금 해제
                break;
            case 3001: // 3단계 연구들을 잠금 해제
                break;
            case 1301:  // 정령왕 1단계 해금
                Blurry[3].transform.GetChild(0).gameObject.SetActive(false);
                StepButton[3].transform.GetChild(1).GetComponent<Button>().interactable = true;
                break;
            case 1302:  // 정령왕 2단계 해금
                Blurry[3].transform.GetChild(1).gameObject.SetActive(false);
                StepButton[3].transform.GetChild(2).GetComponent<Button>().interactable = true;
                break;
            case 1303:  // 정령왕 3단계 해금
                Blurry[3].transform.GetChild(2).gameObject.SetActive(false);
                StepButton[3].transform.GetChild(3).GetComponent<Button>().interactable = true;
                break;
            case 1304:  // 정령왕 4단계 해금
                Blurry[3].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1401:  // 정령세계 1 해금
                Blurry[4].transform.GetChild(0).gameObject.SetActive(false);
                StepButton[4].transform.GetChild(1).GetComponent<Button>().interactable = true;
                break;
            case 1402:  // 정령세계 2 해금
                Blurry[4].transform.GetChild(1).gameObject.SetActive(false);
                StepButton[4].transform.GetChild(2).GetComponent<Button>().interactable = true;
                break;
            case 1403:  // 정령세계 3 해금
                Blurry[4].transform.GetChild(2).gameObject.SetActive(false);
                StepButton[4].transform.GetChild(3).GetComponent<Button>().interactable = true;
                break;
            case 1404:  // 정령세계 4해금
                Blurry[4].transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 1321:  // 정령왕 저성장 지원 1
                for(int i = 0; i < 2; i++)
                { highUp[i].gameObject.GetComponent<Button>().interactable = false;}
                break;
            case 1322:  // 정령왕 고성장 지원 1
                for (int i = 0; i < 2; i++)
                { LowUp[i].gameObject.GetComponent<Button>().interactable = false; }
                break;
            case 1331:  // 정령왕 저성장 지원 2
                for (int i = 0; i < 2; i++)
                { highUp[i].gameObject.GetComponent<Button>().interactable = false; }
                break;
            case 1332:  // 정령왕 고성장 지원 2
                for (int i = 0; i < 2; i++)
                { LowUp[i].gameObject.GetComponent<Button>().interactable = false; }
                break;
            case 1411:  // 정령 세계 냉각 1
                break;
            case 1421:  // 정령 세계 냉각 2
                break;
            case 1431:  // 정령 세계 냉각 3
                break;
            case 1031:  // 돌 생산소- 기술자 2개
                break;
            case 1032:  // 나무 생산소 기술자 2개
                break;
            case 1033:  // 귀족 훈련소 피통 4배
                break;
            case 1034:  // 치유사 훈련소 해금
                HealTraining.SetActive(true);
                break;
            case 1131:  // 정령 스폰 시간 단축 2
                for (int i = 0; i < 4; i++)
                {
                    spiritSpawner[i].spawnWeight = 1.31f;
                }
                SpawnerSpiritSpawnUpgradeButton.GetComponent<Button>().interactable = true;
                SpawnerSpiritSpawnUpgradeButton.GetComponent<Image>().sprite = SpawnerUI.GetComponent<SpawnerUI>().UpgradeSprite[1];
                break;
            case 1132:  // 정령 이동 속도 상승 3
                SpiritManager.instance.spiritMoveSpeed = 1.14f;
                SpiritManager.instance.ChangeSpiritSpeed(1.14f);
                Tree[1].transform.Find("Studies/Step3/Speed3").GetComponent<Button>().interactable = true;
                break;
            case 1133:  // 정령 단계 업그레이드 2
                SpawnerSpiritUpgradeButton.GetComponent<Button>().interactable = true;
                break;
            case 1231:  // 자원 자연 생산량 증가 2
                resourceManager.IncreaseResourceWeightMax();
                break;
            case 1035:  // 마법의 분수
                HealFoundation.SetActive(true);
                break;
        }
    }
}