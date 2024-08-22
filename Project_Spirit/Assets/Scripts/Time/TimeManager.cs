using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TimeManager : MonoBehaviour
{
    public int Month;
    public int Day;

    [Header("승리/패배 UI 세팅")]
    [SerializeField]
    private TextMeshProUGUI Time_text;
    [SerializeField]
    private TextMeshProUGUI ClearTime_text;
    [SerializeField]
    private TextMeshProUGUI DefeatTime_text;
    [SerializeField]
    private TextMeshProUGUI SpiritKingLv_text;
    [SerializeField]
    private GameObject GameOver;
    [SerializeField]
    private GameObject GameClear;

    [SerializeField]
    private TextMeshProUGUI Date_text;
    [SerializeField]
    private GameObject LightController;
    [SerializeField]
    private GameObject soundController;
    GameObject EventManager;

    DateTime DefaultDate;
    DateTime CurrentDate;
    DateTime calc;

    // 날짜별 이벤트 데이터.
    DateTime weatherEventDate = new DateTime(1, 3, 3);  // 폭우 정령 뉴스
    DateTime weatherEventBegin = new DateTime(1, 3, 15);
    DateTime weatherEventOverDate = new DateTime(1, 3, 17); 
    DateTime weatherHotDate = new DateTime(1, 4, 27); 
    DateTime weatherHotOverDate = new DateTime(1, 5, 6); 
    DateTime HotWarnDate = new DateTime(1, 4, 20);
    DateTime BookEventDate = new DateTime(1, 3, 4);
    DateTime TempEndDate = new DateTime(1, 5, 6);


    // 퀘스트 이벤트 데이터.
    DateTime RainQuest = new DateTime(1, 3, 15);
    DateTime SpiritQuest = new DateTime(1, 4, 27);
    DateTime ResourceQuest = new DateTime(1, 3, 1);
    DateTime ResearchQuest = new DateTime(1,3,13);

    // 날짜별 이벤트 시간
    int bookEventHour = 13;
    int weatherEventHour = 19;
    int weatherEventBeginHour = 9;
    int weatherEventOverHour = 6;
    int weatherHotHour = 7;
    int weatherHotOverHour = 0;
    int weatherwarnHour = 7;


    // 퀘스트 이벤트 시간
    int RainQuestHour = 9;
    int SpiritQuestHour = 7;
    int ResourcQuestHour = 12;
    int ResearchQuestHour = 8;


    int currentWeather;
    int temporature;
    public int timeSpeed = 1;
    public float accumulatedGameTime = 0f;

    bool isResource;
    
    TimeSpan span = TimeSpan.FromSeconds(10);
    TemperatureManager temperatureManager;
    SpiritManager spiritManager;
    BuildingDataManager buildingDataManager;
    CradleManager cradleManager;

    [Header("조명 세팅")]
    public Image lightImage; // LightController에 연결된 Image 컴포넌트
    public Color nightColor = new Color(56f / 255f, 0f / 255f, 130f / 255f, 160f / 255f);
    public Color morningColor = new Color(61f / 255f, 0f / 255f, 182f / 255f, 160f / 255f);
    public Color dayColor = new Color(255f / 255f, 255f / 255f, 255f / 255f, 1f);
    public Color eveningColor = new Color(255f / 255f, 0f / 255f, 23f / 255f, 60f / 255f);

    // BGM 설정
    private bool RainBgm = false;
    private int previousRandomNumber = -1; // 이전 랜덤 숫자를 저장할 변수, 초기값은 임의의 값으로 설정
    private bool bgmPlayed = false;

    // 승.패 UI 확인용 시간.
    private DateTime gameStartDate;

    private void Start()
    {
        DefaultDate = DateTime.ParseExact("03-01 06:00:00", "MM-dd HH:mm:ss", null);
        CurrentDate = DateTime.ParseExact("03-01 06:00:00", "MM-dd HH:mm:ss", null);
        calc = DateTime.Now;
        gameStartDate = DateTime.Now; // 게임 시작 시점을 기록

        // For Debug.
        temporature = 26;
        currentWeather = 0;

        EventManager = GameObject.Find("[EventManager]");
        spiritManager = GameObject.Find("GameManager").GetComponent<SpiritManager>();
        cradleManager = GameObject.Find("CradleManager").GetComponent<CradleManager>();
        buildingDataManager = GameObject.Find("GameManager").GetComponent<BuildingDataManager>();
        temperatureManager = GetComponent<TemperatureManager>();

        // 날씨와 정령의 체력 감소 관계
        StartCoroutine(TakeDamageRoutine());

    }

    private void Update()
    {
        CalculateTime();
        SetTimeText();      // 인게임 시간 UI
        SetSunLight();      // 조명 효과 세팅
        CheckEventDate();
        CheckEventBGM();    // 폭우 BGM 조건
        CheckBGMTime();     // BGM 조건
        CheckQuestDate();   // 퀘스트 조건
    }

    void CalculateTime()
    {
        // 현실 시간 1초 = 게임 시간 12분
        float deltaTime = (float)(DateTime.Now - calc).TotalSeconds;
        accumulatedGameTime += deltaTime * Time.timeScale * 12f * 60f * timeSpeed * 2;  // 플레이 시간이 너무 긺 
        calc = DateTime.Now;

        // 누적된 게임 시간을 이용해 현재 게임 날짜와 시간을 계산합니다.
        CurrentDate = DefaultDate + TimeSpan.FromSeconds(accumulatedGameTime); 

    }

    void SetTimeText()
    {        
        if (CurrentDate.Hour >= 12)
            Time_text.text = "PM " + CurrentDate.ToString("hh:mm");
        else
            Time_text.text = "AM " + CurrentDate.ToString("hh:mm");
        Date_text.text = CurrentDate.ToString("M월 d일");

    }

    void SetBGMbyTime()
    {
        // 오전 6:30에 BGM 설정
        if (CurrentDate.Hour == 6 && CurrentDate.Minute == 30)
        {
            SetBGM();
       
        }
        // 특정 시간이 지난 후 bgmPlayed를 false로 재설정
        if (CurrentDate.Hour == 6 && CurrentDate.Minute == 20)
        {
            bgmPlayed = false;
        }

    }

    void CheckBGMTime()
    {
        // 오전 6:30 또는 오후 6:30에 BGM 설정
        if ((CurrentDate.Hour == 6 && CurrentDate.Minute == 30) || (CurrentDate.Hour == 18 && CurrentDate.Minute == 30))
        {
            if (!bgmPlayed)
            {
                SetBGM();
                bgmPlayed = true;
            }
        }
        else if (CurrentDate.Minute != 30)
        {
            // 30분이 아닐 때는 bgmPlayed를 false로 설정하여 다시 재생할 수 있도록 함
            bgmPlayed = false;
        }
    }

    void CheckEventBGM()
    {
        if (EventManager.GetComponent<WaterFallEvent>().waterFallEvent)
        {
            soundController.GetComponent<SoundManager>().PlayBgm("Rain");
            RainBgm = true;
        }
        else if(!EventManager.GetComponent<WaterFallEvent>())
            RainBgm = false;
    }
    void SetBGM()
    {
        if (RainBgm) return;

    
        // 4월 27일 이후인지 확인
        bool IsAfterApril27()
        {
            return CurrentDate.Month > 4 || (CurrentDate.Month == 4 && CurrentDate.Day >= 27);
        }


        // 특정 시간 (오전 6:30)인지 확인하는 함수
        bool IsSpecificTime()
        {
            return CurrentDate.Hour == 6 && CurrentDate.Minute == 30;
        }

        // 전 BGM과 다른 BGM 호출
        int GenerateRandomNumber(int min, int max)
        {
            System.Random random = new System.Random();
            int newRandomNumber;
            do
            {
                newRandomNumber = random.Next(min, max);
            } while (newRandomNumber == previousRandomNumber);
            previousRandomNumber = newRandomNumber;
            return newRandomNumber;
        }

        if (IsAfterApril27())
        {
            if (IsSpecificTime() && !bgmPlayed)
            {
                int randomNumber = GenerateRandomNumber(0, 2);
                switch (randomNumber)
                {
                    case 0:
                        soundController.GetComponent<SoundManager>().PlayBgm("Summer_BGM_01");
                        break;
                    case 1:
                        soundController.GetComponent<SoundManager>().PlayBgm("Summer_BGM_02");
                        break;
                   
                }
                return;
            }
        }
        else
        {
            if (IsSpecificTime())
            {
                int randomNumber = GenerateRandomNumber(0, 3);
                switch (randomNumber)
                {
                    case 0:
                        soundController.GetComponent<SoundManager>().PlayBgm("BGM_Day0");
                        break;
                    case 1:
                        soundController.GetComponent<SoundManager>().PlayBgm("BGM_Day1");
                        break;
                    case 2:
                        soundController.GetComponent<SoundManager>().PlayBgm("BGM_Night");
                        break;
                   
                }
                return;
            }
        }

      
    }


// 메인테마 사운드
public void SetMainThemeBGM()
    {

        soundController.GetComponent<SoundManager>().PlayBgm("MainTheme");
    }

    void SetSunLight()
    {
        int hour = CurrentDate.Hour;

        float t = (CurrentDate.Minute + CurrentDate.Second / 60f) / 60f; // 분과 초를 0-1 사이의 값으로 변환

        if (hour >= 21 || hour < 5)
        {
            // 밤 색상
            lightImage.color = Color.Lerp(nightColor, nightColor, t); // 변화 없음
        }
        else if (hour >= 5 && hour < 7)
        {
            // 밤 -> 아침 색상 전환
            lightImage.color = Color.Lerp(nightColor, morningColor, t + (hour - 5));
        }
        else if (hour >= 7 && hour < 18)
        {
            // 아침 -> 낮 색상 전환
            lightImage.color = Color.Lerp(morningColor, dayColor, (hour - 7 + t) / 11f);
        }
        else if (hour >= 18 && hour < 21)
        {
            // 낮 -> 저녁 색상 전환
            lightImage.color = Color.Lerp(dayColor, eveningColor, (hour - 18 + t) / 3f);
        }
        else if (hour >= 21 && hour < 24)
        {
            // 저녁 -> 밤 색상 전환
            lightImage.color = Color.Lerp(eveningColor, nightColor, (hour - 21 + t) / 3f);
        }
    }

    public DateTime GetCurrentDate()
    {
        return CurrentDate;
    }

    public int GetWeather()
    {
        return currentWeather;
    }

    // 날짜별 이벤트 호출 메서드
    void CheckEventDate()
    {
        if (CurrentDate.Month == weatherEventDate.Month && CurrentDate.Day == weatherEventDate.Day && CurrentDate.Hour == weatherEventHour)
        {
            EventManager.GetComponent<WaterFallEvent>().NewsPaperEventTrigger();
        }
        if(CurrentDate.Month == weatherEventBegin.Month && CurrentDate.Day == weatherEventBegin.Hour && CurrentDate.Hour == weatherEventBeginHour)
        {
            EventManager.GetComponent<WaterFallEvent>().RainDropEventTrigger();
            QuestManager.instance.rain = true;
        }

        if (CurrentDate.Month == weatherEventOverDate.Month && CurrentDate.Day == weatherEventOverDate.Day && CurrentDate.Hour == weatherEventOverHour)
        {
            EventManager.GetComponent<WaterFallEvent>().RainDropEventEnd();
            // 비 이벤트 종료 후 보상 지급
            QuestManager.instance.GainItem();
        }

        if(CurrentDate.Month == BookEventDate.Month && CurrentDate.Day == BookEventDate.Day && CurrentDate.Hour == bookEventHour)
        {
            EventManager.GetComponent<BookEvent>().BookEventTrigger();
        }
        // 폭염 경고창
        if (CurrentDate.Month == HotWarnDate.Month && CurrentDate.Day == HotWarnDate.Day && CurrentDate.Hour == weatherwarnHour)
        {
            EventManager.GetComponent<WaterFallEvent>().HotnewsEventTrigger();
        }
        // 폭염 이벤트 시작
        if (CurrentDate.Month == weatherHotDate.Month && CurrentDate.Day == weatherHotDate.Day && CurrentDate.Hour == weatherHotHour)
        {
            EventManager.GetComponent<WaterFallEvent>().HotEventTrigger();
        }
        if (CurrentDate.Month == weatherHotOverDate.Month && CurrentDate.Day == weatherHotOverDate.Day && CurrentDate.Hour == weatherHotOverHour)
        {
            EventManager.GetComponent<WaterFallEvent>().HotEventEventEnd();
        }
        if(CurrentDate.Month == TempEndDate.Month && CurrentDate.Day == TempEndDate.Day)
        {
            if (cradleManager.Level > 3)
                cradleManager.CheckTempWin();
            else
                cradleManager.CheckTempLose();
        }

    }

    // 퀘스트 날짜별 부여
    private void CheckQuestDate()
    {
        // 폭우 피해 축소 퀘스트 Q.1001
        if(CurrentDate.Month == RainQuest.Month && CurrentDate.Day == RainQuest.Day && CurrentDate.Hour == RainQuestHour)
        {
            QuestManager.instance.InstantiateQuest(1001);
            QuestManager.instance.OverRain = true;
        }

        // 정령왕의 성장기 Q.1003
        if(CurrentDate.Month == SpiritQuest.Month && CurrentDate.Day == SpiritQuest.Day && CurrentDate.Hour == SpiritQuestHour)
        {
            QuestManager.instance.InstantiateQuest(1003);
            QuestManager.instance.Spirit = true;
        }
        // 자원 흭득 Q.1004 
        if (CurrentDate.Month == ResourceQuest.Month && CurrentDate.Day == ResourceQuest.Day && CurrentDate.Hour == ResourcQuestHour)
        {
            if(!isResource)
            {
                QuestManager.instance.InstantiateQuest(1004);
                QuestManager.instance.GainR = true;
                // 중복 호출 방지
                isResource = true;
            }
            
        }

        // 저장소 건설 Q.1005
        // 저장소 퀘스트 미구현

        // 연구소 2단계 해금 Q. 1006
        if(CurrentDate.Month == ResearchQuest.Month && CurrentDate.Day == ResearchQuest.Day && CurrentDate.Hour == ResearchQuestHour)
        {
            QuestManager.instance.InstantiateQuest(1006);
            QuestManager.instance.ResearchMode = true;
        }

    }


   public void PauseCradleUI()
   {
        SoundManager.instance.UIButtonclick();
        spiritManager.GetSpeed();
        spiritManager.SetSpeed(0f);
        timeSpeed = 0;
   }

    public void PlayCradleUI()
    {
        SoundManager.instance.UIButtonclick();
        spiritManager.GetSpeed();
        spiritManager.SetSpeed(1f);
        timeSpeed = 1;
        
    }

    public void FastPlay()
    {
        SoundManager.instance.UIButtonclick();
        spiritManager.GetSpeed();
        spiritManager.SetSpeed(2f);
        timeSpeed = 2;
        
    }

    public void DoudlbePlay()
    {
        SoundManager.instance.UIButtonclick();
        spiritManager.GetSpeed();
        spiritManager.SetSpeed(3f);
      
        timeSpeed = 4;
        
    }

    public string GetCurrentDateTimeString()
    {
        string month = CurrentDate.Month.ToString("00");
        string day = CurrentDate.Day.ToString("00");
        string hour = CurrentDate.Hour.ToString("00");

        string timeString = month + day + hour;
        return timeString;
    }
    public void SetDate(int Month, int day)
    {
        // 새로운 날짜 설정
        DefaultDate = new DateTime(1, Month, day, 0, 0, 0);
        CurrentDate = new DateTime(1, Month, day, 0, 0, 0);

        // 게임 시간 초기화
        accumulatedGameTime = 0f;
        calc = DateTime.Now;

        // 업데이트된 날짜로 즉시 시간을 재설정
        SetTimeText();
        //SetSunLight();
        CheckEventDate();
    }

    public void MoveDateTo313()
    {
        SetDate(3, 13);
    }

    public void MoveDateTo321()
    {
        SetDate(3, 21);
    }

    public void MoveTo329()
    {
        SetDate(3, 29);
    }

    public void MoveTo42()
    {
        SetDate(4, 02);
    }

    public void MoveTo415()
    {
        SetDate(4, 15);
    }
    // 1분마다 TakeDamageByWeather 메서드를 실행하는 Coroutine
    IEnumerator TakeDamageRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f); // 1분 대기

            // 예시: EventManager의 하위 오브젝트에 대해 피해를 입힘
            temperatureManager.WeatherAndSpiritRealtion(); ;
        }
    }

    public void CheckGameTime()
    {
        // DefaultDate부터 CurrentDate까지의 경과 시간 계산
        TimeSpan elapsedDateSpan = CurrentDate - DefaultDate;
        int months = (CurrentDate.Year - DefaultDate.Year) * 12 + CurrentDate.Month - DefaultDate.Month;
        int days = elapsedDateSpan.Days;
        // 요일 부분을 제외한 일수를 계산
        days -= months * 30; // 대략적인 평균 일수로 계산

        string elapsedDateText = $"{months}개월 {days}일";

        // 게임 시작부터 Check 메서드 호출까지 걸린 시간 계산
        TimeSpan elapsedGameTimeSpan = DateTime.Now - gameStartDate;
        string elapsedGameTimeText = $"{elapsedGameTimeSpan.Hours:D2}시간 {elapsedGameTimeSpan.Minutes:D2}분";

        // 모든 시간 일시정지
        timeSpeed = 0;

        StartCoroutine(UpdateClearUIText(elapsedDateText, elapsedGameTimeText));
    }

    public void CheckGameLoseTime(int lv)
    {
        // DefaultDate부터 CurrentDate까지의 경과 시간 계산
        TimeSpan elapsedDateSpan = CurrentDate - DefaultDate;
        int months = (CurrentDate.Year - DefaultDate.Year) * 12 + CurrentDate.Month - DefaultDate.Month;
        int days = elapsedDateSpan.Days;
        // 요일 부분을 제외한 일수를 계산
        days -= months * 30; // 대략적인 평균 일수로 계산

        string elapsedDateText = $"{months}개월 {days}일";

        // 게임 시작부터 Check 메서드 호출까지 걸린 시간 계산
        TimeSpan elapsedGameTimeSpan = DateTime.Now - gameStartDate;
        string elapsedGameTimeText = $"{elapsedGameTimeSpan.Hours:D2}시간 {elapsedGameTimeSpan.Minutes:D2}분";

        // 모든 시간 일시정지
        timeSpeed = 0;

        // 결과 출력을 위한 Coroutine 호출
        StartCoroutine(UpdateFailUIText(elapsedDateText, elapsedGameTimeText, lv));

    }


    private IEnumerator UpdateFailUIText(string elapsedDateText, string elapsedGameTimeText, int lv)
    {
        // 0.25초 대기 후 DefeatTime_text 업데이트
        yield return new WaitForSeconds(0.25f);
        
        GameOver.transform.GetChild(4).gameObject.SetActive(true);
        GameOver.transform.GetChild(5).gameObject.SetActive(true);

        // 추가로 0.25초 대기 후 SpiritKingLv_text 업데이트
        yield return new WaitForSeconds(0.25f);
        GameOver.transform.GetChild(6).gameObject.SetActive(true);
        GameOver.transform.GetChild(7).gameObject.SetActive(true);
        DefeatTime_text.text = elapsedDateText + " " + elapsedGameTimeText;
        Debug.Log($"경과 시간: {elapsedDateText}");
        Debug.Log($"게임 시작부터 체크 호출까지 걸린 시간: {elapsedGameTimeText}");
        SpiritKingLv_text.text = $"{lv + 1}단계";
    }

    private IEnumerator UpdateClearUIText(string elapsedDateText, string elapsedGameTimeText)
    {
        // 0.25초 대기 후 Clear 업데이트
        yield return new WaitForSeconds(0.25f);

        GameClear.transform.GetChild(3).gameObject.SetActive(true);
        GameClear.transform.GetChild(4).gameObject.SetActive(true);

        // 결과 출력
        ClearTime_text.text = elapsedDateText + " " + elapsedGameTimeText;
        Debug.Log($"경과 시간: {elapsedDateText}");
        Debug.Log($"게임 시작부터 체크 호출까지 걸린 시간: {elapsedGameTimeText}");
        
    }
}
