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

    [SerializeField]
    private TextMeshProUGUI Time_text;
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
    DateTime weatherEventDate = new DateTime(1, 3, 13); 
    DateTime weatherEventOverDate = new DateTime(1, 3, 15); 
    DateTime weatherHotDate = new DateTime(1, 4, 27); 
    DateTime weatherHotOverDate = new DateTime(1, 4, 30); 
    DateTime HotWarnDate = new DateTime(1, 4, 20);
    DateTime BookEventDate = new DateTime(1, 3, 25);
    int bookEventHour = 13;
    int weatherEventHour = 17;
    int weatherEventOverHour = 6;
    int weatherHotHour = 7;
    int weatherHotOverHour = 0;
    int weatherwarnHour = 7;

    int currentWeather;
    int temporature;
    public int timeSpeed = 1;
    public float accumulatedGameTime = 0f;

    
    TimeSpan span = TimeSpan.FromSeconds(10);
    TemperatureManager temperatureManager;
    SpiritManager spiritManager;
    BuildingDataManager buildingDataManager;

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

    private void Start()
    {
        DefaultDate = DateTime.ParseExact("03-01 06:00:00", "MM-dd HH:mm:ss", null);
        CurrentDate = DateTime.ParseExact("03-01 06:00:00", "MM-dd HH:mm:ss", null);
        calc = DateTime.Now;

        // For Debug.
        temporature = 26;
        currentWeather = 0;

        EventManager = GameObject.Find("[EventManager]");
        spiritManager = GameObject.Find("GameManager").GetComponent<SpiritManager>();
        buildingDataManager = GameObject.Find("GameManager").GetComponent<BuildingDataManager>();
        temperatureManager = GetComponent<TemperatureManager>();

        // 날씨와 정령의 체력 감소 관계
        StartCoroutine(TakeDamageRoutine());

    }

    private void Update()
    {
        CalculateTime();
        SetTimeText();
        SetSunLight();
        CheckEventDate();
        CheckEventBGM();
        CheckBGMTime();
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
        else
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

        if (CurrentDate.Month == weatherEventOverDate.Month && CurrentDate.Day == weatherEventOverDate.Day && CurrentDate.Hour == weatherEventOverHour)
        {
            EventManager.GetComponent<WaterFallEvent>().RainDropEventEnd();
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

    public void MoveDateTo312()
    {
        SetDate(3, 12);
    }

    public void MoveDateTo325()
    {
        SetDate(3, 25);
    }

    public void MoveTo420()
    {
        SetDate(4, 20);
    }

    public void MoveTo427()
    {
        SetDate(4, 27);
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
}
