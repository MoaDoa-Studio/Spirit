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
    DateTime weatherHotOverDate = new DateTime(1, 5, 6); 
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
        //SetSunLight();
        CheckEventDate();
        CheckEventBGM();
        SetBGM();
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

        // 오후 6:30에 BGM 설정
        if (CurrentDate.Hour == 18 && CurrentDate.Minute == 30)
        {
            SetBGM();
         
        }
    }

    void CheckEventBGM()
    {
        if (EventManager.GetComponent<WaterFallEvent>().waterFallEvent)
        {
            soundController.GetComponent<SoundManager>().PlayBgm("Rain");
        }
    }
    void SetBGM()
    {

        // 특정 시간대 (오전 6:30부터 오후 6:30까지)인지 확인하는 함수
        bool IsDayTime()
        {
            return (CurrentDate.Hour > 6 || (CurrentDate.Hour == 6 && CurrentDate.Minute >= 30)) &&
                   (CurrentDate.Hour < 18 || (CurrentDate.Hour == 18 && CurrentDate.Minute < 30));
        }

        // 4월 27일 이후인지 확인
        bool IsAfterApril27()
        {
            return CurrentDate.Month > 4 || (CurrentDate.Month == 4 && CurrentDate.Day >= 27);
        }

        // 짝수 날인지 확인
        bool IsEvenDay()
        {
            return CurrentDate.Day % 2 == 0;
        }

        if (IsAfterApril27())
        {
            if (IsDayTime())
            {
                if (IsEvenDay())
                {
                    soundController.GetComponent<SoundManager>().PlayBgm("Summer_BGM_01");
                }
                else
                {
                    soundController.GetComponent<SoundManager>().PlayBgm("Summer_BGM_02");
                }
            }
            else
            {
                soundController.GetComponent<SoundManager>().PlayBgm("BGM_Night");
            }
        }
        else
        {
            if (IsDayTime())
            {
                if (IsEvenDay())
                {
                    soundController.GetComponent<SoundManager>().PlayBgm("BGM_Day0");
                }
                else
                {
                    soundController.GetComponent<SoundManager>().PlayBgm("BGM_Day1");
                }
            }
            else
            {
                soundController.GetComponent<SoundManager>().PlayBgm("BGM_Night");
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
        if (hour >= 19 || hour < 4)
        {
            // �ּ� ���.
            LightController.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        }    
        else if (hour >= 8 && hour < 15)
        {
            // �ִ� ���.
            LightController.GetComponent<Image>().color = new Color(0, 0, 0, 0f);
        }
        else if (hour >= 5 && hour < 8)
        {
            // ���� �����            
            if (hour == 5)            
                LightController.GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);
            else if (hour == 6)
                LightController.GetComponent<Image>().color = new Color(1, 0.5f, 0, 0.2f);
            else if (hour == 7)
                LightController.GetComponent<Image>().color = new Color(1, 0.5f, 0, 0.1f);
        }
        else
        {
            // ���� ��ο���.
            if (hour == 16)
                LightController.GetComponent<Image>().color = new Color(1, 0.5f, 0, 0.1f);            
            else if (hour == 17)
                LightController.GetComponent<Image>().color = new Color(1, 0.5f, 0, 0.2f);
            else if (hour == 18)
                LightController.GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);

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

        if (CurrentDate.Month == HotWarnDate.Month && CurrentDate.Day == HotWarnDate.Day && CurrentDate.Hour == weatherwarnHour)
        {
            EventManager.GetComponent<BookEvent>().WeatherHotEvent();
        }
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
