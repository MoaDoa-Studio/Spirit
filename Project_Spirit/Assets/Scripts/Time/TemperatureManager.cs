using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using TMPro;

public class TemperatureManager : MonoBehaviour
{
    TimeManager timeManager;
    public List<TemperatureData> temperatureDatas;
    public string fileName;

    [SerializeField]
    private GameObject SpiritParent;
    [SerializeField]
    private float worldTemperature;
    [SerializeField]
    private TextMeshProUGUI Temperature_text;
    [SerializeField]
    private string timeTostring;

    private void Awake()
    {
        LoadTempData(fileName);
    }

    private void Start()
    {
        timeManager = GetComponent<TimeManager>();
    }

    private void Update()
    {
        // 기온 & 날짜 형식 동기화.
        MatchDateWithTemperature();

        // 날씨와 정령의 체력 감소 관계
        // WeatherAndSpiritRealtion();
    }
    // 기온 XML 빌드 데이터 로드.
    private void LoadTempData(string _fileName)
    {
        TextAsset xmlAsset = Resources.Load<TextAsset>("XML/" + _fileName);
        if (xmlAsset == null)
        {
            Debug.LogError("XML file not found: " + _fileName);
            return;
        }
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlAsset.text);

        XmlNodeList xmlNodeList = xmlDoc.SelectNodes("//TemperatureData");

        foreach (XmlNode xmlNode in xmlNodeList)
        {
            TemperatureData TempData = ScriptableObject.CreateInstance<TemperatureData>();
            TempData.Nowtime = "0"+xmlNode.SelectSingleNode("NowTime").InnerText;
            TempData.Temperature = float.Parse(xmlNode.SelectSingleNode("Temperature").InnerText);
            temperatureDatas.Add(TempData);
        }
    }

    void MatchDateWithTemperature()
    {
        timeTostring = timeManager.GetCurrentDateTimeString();

        foreach(TemperatureData TempData in temperatureDatas)
        {
            // 같은 기온일 시에
            if(TempData.Nowtime == timeTostring)
            {
                Temperature_text.text = "/ 기온 " + TempData.Temperature.ToString() + "°";
                worldTemperature = TempData.Temperature;
            }
        }
    }

    public void WeatherAndSpiritRealtion()
    {
        // 기온이 25도 이하일시 1분당 체력 감소 정도
        // 0.041667 의 체력퍼 대미지
        if(worldTemperature <= 25)
        {
            TakeDamageByWeather(SpiritParent);
        }
        // 기온이 26도 이상일 시 정령의 1분당 체력 감소 정도
        // 0.041667 * ((현재기온 -25)/ 10)
        else
        {
            TakeDamageOverByWeather(SpiritParent);
        }
    }

    void TakeDamageByWeather(GameObject spiritParent)
    {
      foreach(Transform child in spiritParent.transform)
        {
            Spirit spirit = child.GetComponent<Spirit>();
            if(spirit != null)
            {
                spirit.TakeDamage25ByWeather();
            }
        }
    }
    void TakeDamageOverByWeather(GameObject spiritParent)
    {
        foreach (Transform child in spiritParent.transform)
        {
            Spirit spirit = child.GetComponent<Spirit>();
            if (spirit != null)
            {
                spirit.TakeDamage25OverByWeather(worldTemperature);
            }
        }
    }
}


public class TemperatureData : ScriptableObject
{
    public string Nowtime;
    public float Temperature;

}