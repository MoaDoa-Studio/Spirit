using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TimeManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Time_text;
    [SerializeField]
    private TextMeshProUGUI Temperature_text;
    [SerializeField]
    private TextMeshProUGUI Date_text;
    [SerializeField]
    private GameObject LightController;

    DateTime DefaultDate;
    DateTime CurrentDate;
    DateTime calc;
    int currentWeather;
    int temporature;    
    TimeSpan span = TimeSpan.FromSeconds(10);
    private void Start()
    {
        DefaultDate = DateTime.ParseExact("01-01 00:00:00", "MM-dd HH:mm:ss", null);
        CurrentDate = DateTime.ParseExact("01-01 00:00:00", "MM-dd HH:mm:ss", null);
        calc = DateTime.Now;

        // For Debug.
        temporature = 26;
        currentWeather = 0;
    }

    private void Update()
    {
        CalculateTime();
        SetTimeText();
        // 임시 조치
        //SetSunLight();
    }

    void CalculateTime()
    {
        // 현실 시간 1초 = 게임 시간 12분
        TimeSpan diff = DateTime.Now - calc;
        CurrentDate = DefaultDate 
            + TimeSpan.FromMinutes(diff.Seconds * 12) 
            + TimeSpan.FromHours(diff.Minutes * 12) 
            + TimeSpan.FromDays(diff.Hours * 30);
    }

    void SetTimeText()
    {        
        if (CurrentDate.Hour >= 12)
            Time_text.text = "PM " + CurrentDate.ToString("hh:mm");
        else
            Time_text.text = "AM " + CurrentDate.ToString("hh:mm");
        Date_text.text = CurrentDate.ToString("MM-dd");
        Temperature_text.text = "기온 " + temporature.ToString() + "°";
    }

    void SetSunLight()
    {
        int hour = CurrentDate.Hour;
        if (hour >= 19 || hour < 4)
        {
            // 최소 밝기.
            LightController.GetComponent<Image>().color = new Color(0, 0, 0, 0.8f);
        }    
        else if (hour >= 8 && hour < 15)
        {
            // 최대 밝기.
            LightController.GetComponent<Image>().color = new Color(0, 0, 0, 0f);
        }
        else if (hour >= 5 && hour < 8)
        {
            // 점점 밝아짐            
            if (hour == 5)            
                LightController.GetComponent<Image>().color = new Color(0, 0, 0, 0.6f);
            else if (hour == 6)
                LightController.GetComponent<Image>().color = new Color(1, 0.5f, 0, 0.2f);
            else if (hour == 7)
                LightController.GetComponent<Image>().color = new Color(1, 0.5f, 0, 0.1f);
        }
        else
        {
            // 점점 어두워짐.
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
}
