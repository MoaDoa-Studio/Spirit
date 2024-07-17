using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class TemperatureManager : MonoBehaviour
{
    public List<TemperatureData> temperatureDatas;
    public string fileName;

    private void Awake()
    {
        LoadTempData(fileName);
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
            TempData.Nowtime = float.Parse(xmlNode.SelectSingleNode("NowTime").InnerText);
            TempData.Temperature = float.Parse(xmlNode.SelectSingleNode("Temperature").InnerText);
            temperatureDatas.Add(TempData);
        }
    }
}


public class TemperatureData : ScriptableObject
{
    public float Nowtime;
    public float Temperature;

}