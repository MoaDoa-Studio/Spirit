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
    DateTime weatherEventOverDate = new DateTime(1, 3, 21); 
    DateTime BookEventDate = new DateTime(1, 3, 25);

    int bookEventHour = 13;
    int weatherEventHour = 17;
    int weatherEventOverHour = 6;

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
        DefaultDate = DateTime.ParseExact("03-01 07:00:00", "MM-dd HH:mm:ss", null);
        CurrentDate = DateTime.ParseExact("03-01 07:00:00", "MM-dd HH:mm:ss", null);
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
        SetBGM();
    }

    void CalculateTime()
    {
        // 현실 시간 1초 = 게임 시간 12분
        float deltaTime = (float)(DateTime.Now - calc).TotalSeconds;
        accumulatedGameTime += deltaTime * Time.timeScale * 12f * 60f * timeSpeed;
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
        Date_text.text = CurrentDate.ToString("MM-dd");
       
    }

    void SetBGM()
    {
        if(EventManager.GetComponent<WaterFallEvent>().waterFallEvent)
        {
            soundController.GetComponent<SoundManager>().PlayBgm("Rain");
        }
        else
        {

            if(CurrentDate.Hour >= 7 && CurrentDate.Hour < 18)
            {
                soundController.GetComponent<SoundManager>().PlayBgm("BGM4");
            }
            else if ((CurrentDate.Hour > 18 || (CurrentDate.Hour == 18 && CurrentDate.Minute >= 30)) ||
                  (CurrentDate.Hour < 6 || (CurrentDate.Hour == 6 && CurrentDate.Minute < 20)))
            {
                soundController.GetComponent<SoundManager>().PlayBgm("BGM5");

            }
        }
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
    // 1분마다 TakeDamageByWeather 메서드를 실행하는 Coroutine
    IEnumerator TakeDamageRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3600); // 1분 대기

            // 예시: EventManager의 하위 오브젝트에 대해 피해를 입힘
            temperatureManager.WeatherAndSpiritRealtion(); ;
        }
    }
}
