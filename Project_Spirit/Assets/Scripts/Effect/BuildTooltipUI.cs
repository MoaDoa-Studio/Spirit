using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildTooltipUI : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField buildName;
    [SerializeField]
    private TMP_InputField buildDescrition;
    [SerializeField]
    private TMP_InputField buildWoodPrice;
    [SerializeField]
    private TMP_InputField buildRockPrice;
    [SerializeField]
    private TMP_InputField UseWoodPrice;
    [SerializeField]
    private TMP_InputField UseRockPrice;

    [SerializeField]
    private GameObject go_base;

    List<BuildData> buildDataList;
    List<StructUniqueData> structUniqueDataList;
    BuildData buildData;
    StructUniqueData structUniqueData;

    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);

    private void Start()
    {
        buildDataList = GameObject.Find("GameManager").GetComponent<BuildingDataManager>().buildDataList;
        structUniqueDataList = GameObject.Find("GameManager").GetComponent<BuildingDataManager>().structUniqueDataList;
    }
    public void ShowToolTip(int _item, Vector3 _pos)
    {
        this.gameObject.SetActive(true);

        transform.position = new Vector3(_pos.x, _pos.y + 100f,0);

        buildData = FindDataFromBuildData(buildDataList, _item);
        structUniqueData = FindDataFromStructUnique(structUniqueDataList, buildData.UniqueProperties);

        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = buildData.structureName;
        transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = buildData.StructureDescription;
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = buildData.woodRequirement.ToString();
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = buildData.stoneRequirement.ToString();
        transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = structUniqueData.CostUseWood.ToString();
        transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = structUniqueData.CostOfStone.ToString();


    }

    public void HideToolTip()
    {
        this.gameObject.SetActive(false);
    }

    private BuildData FindDataFromBuildData(List<BuildData> buildDataList, int _buildID)
    {
        foreach (BuildData buildData in buildDataList)
        {
            if (buildData.structureID == _buildID)
            {
                return buildData;
            }
        }
        return null;
    }

    private StructUniqueData FindDataFromStructUnique(List<StructUniqueData> structUniqueData, int _buildID)
    {
        foreach (StructUniqueData uniqueData in structUniqueData)
        {
            if (uniqueData.UniqueProperties == _buildID)
            { return uniqueData; }

        }
        return null;
    }
}
