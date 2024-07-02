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
    DateTime weatherEventDate = new DateTime(1, 3, 25); 
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
        
        CheckEventDate();
    }

    void CalculateTime()
    {
        // ���� �ð� 1�� = ���� �ð� 12��
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
        Temperature_text.text = "��� " + temporature.ToString() + "��";
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
            // ���� ȣ�� �̺�Ʈ �߹�!
            Debug.Log(" ���� ȣ�� �̺�Ʈ");
        }
    }
}
