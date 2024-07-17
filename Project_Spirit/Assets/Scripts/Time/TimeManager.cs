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
    GameObject EventManager;

    DateTime DefaultDate;
    DateTime CurrentDate;
    DateTime calc;
    DateTime weatherEventDate = new DateTime(1, 3, 13); 
    DateTime weatherEventOverDate = new DateTime(1, 3, 21); 
    int currentWeather;
    int temporature;
    float accumulatedGameTime = 0f;
    TimeSpan span = TimeSpan.FromSeconds(10);

    TemperatureManager temperatureManager;
    private void Start()
    {
        DefaultDate = DateTime.ParseExact("01-01 00:00:00", "MM-dd HH:mm:ss", null);
        CurrentDate = DateTime.ParseExact("01-01 00:00:00", "MM-dd HH:mm:ss", null);
        calc = DateTime.Now;

        // For Debug.
        temporature = 26;
        currentWeather = 0;

        EventManager = GameObject.Find("[EventManager]");
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
    }

    void CalculateTime()
    {
        // 현실 시간 1초 = 게임 시간 12분
        float deltaTime = (float)(DateTime.Now - calc).TotalSeconds;
        accumulatedGameTime += deltaTime * Time.timeScale * 12f * 60f;
        calc = DateTime.Now;

        // 누적된 게임 시간을 이용해 현재 게임 날짜와 시간을 계산합니다.
        CurrentDate = DefaultDate + TimeSpan.FromSeconds(accumulatedGameTime); 

        // ���� �ð� 1�� = ���� �ð� 12��
       // TimeSpan diff = DateTime.Now - calc;
       // CurrentDate = DefaultDate 
        //    + TimeSpan.FromMinutes(diff.Seconds * 12) 
         //   + TimeSpan.FromHours(diff.Minutes * 12) 
          //  + TimeSpan.FromDays(diff.Hours * 30);
    }

    void SetTimeText()
    {        
        if (CurrentDate.Hour >= 12)
            Time_text.text = "PM " + CurrentDate.ToString("hh:mm");
        else
            Time_text.text = "AM " + CurrentDate.ToString("hh:mm");
        Date_text.text = CurrentDate.ToString("MM-dd");
        //Temperature_text.text = "기온 " + temporature.ToString() + "°";
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
        if (CurrentDate.Month == weatherEventDate.Month && CurrentDate.Day == weatherEventDate.Day)
        {
            EventManager.GetComponent<WaterFallEvent>().NewsPaperEventTrigger();
        }

        if (CurrentDate.Month == weatherEventOverDate.Month && CurrentDate.Day == weatherEventOverDate.Day)
        {
            EventManager.GetComponent<WaterFallEvent>().RainDropEventEnd();
        }
    }

   public void PauseCradleUI()
   {
        Time.timeScale = 0f;
   }

    public void PlayCradleUI()
    {
        Time.timeScale = 1f;
    }

    public void FastPlay()
    {
        Time.timeScale = 4f;
    }

    public void DoudlbePlay()
    {
        Time.timeScale = 8f;

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
        SetSunLight();
        CheckEventDate();
    }

    public void MoveDate()
    {
        SetDate(3, 12);
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
