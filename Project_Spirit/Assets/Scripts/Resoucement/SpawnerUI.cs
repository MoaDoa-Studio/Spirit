using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnerUI : MonoBehaviour
{
    [Header("정령 생산소 UI 세팅")]
    [SerializeField]
    Slider[] allSliders;
    [SerializeField]
    GameObject Spawner_Name;
    [SerializeField]
    GameObject Sprit1;
    [SerializeField]
    GameObject Sprit2;
    [SerializeField]
    GameObject Sprit3;
    [SerializeField]
    GameObject SpawnLv;
    [SerializeField]
    GameObject SpiritLv;

    [HideInInspector]
    public Slider slider;
    [HideInInspector]
    public InputField textcoom;
    public GameObject controllingSpawn;
    private List<SpiritSpawnInfo> spawnInfoList = new List<SpiritSpawnInfo>();
    
    private void Start()
    {
        slider = GetComponentInChildren<Slider>();
        slider.onValueChanged.AddListener(updateText);
        foreach (Slider slider in allSliders)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        textcoom.onEndEdit.AddListener(SetSliderValueFromInput);
    }

    #region 슬라이더 조정
    void updateText(float val)
    {
        textcoom.text = slider.value.ToString("F1");
    }

    // 다른 슬라이더들의 값을 변경하는 메서드
    void OnSliderValueChanged(float value)
    {   
        foreach (Slider s in allSliders)
        {
            s.value = value;
        }
    }
    
    // 사용자의 입력값에 따라 슬라이더의 값을 설정하는 메서드
    void SetSliderValueFromInput(string input)
    {
        // 사용자의 입력값을 float으로 변환하여 슬라이더의 값으로 설정
        float inputValue;
        if (float.TryParse(input, out inputValue))
        {
            slider.value = inputValue;
            if(controllingSpawn != null)
               controllingSpawn.GetComponent<SpiritSpawner>().slider.value = inputValue;
        }
    }

    #endregion

    #region UI 세팅
    public void ReceiveSpiritSpawnInfo(SpiritSpawnInfo spawnInfo)
    {
        spawnInfoList.RemoveAll(item => item.SpawnerName == spawnInfo.SpawnerName);
        spawnInfoList.Add(spawnInfo);
        ApplyInfo(spawnInfo);
        Debug.Log(spawnInfo.SpawnerName);
    }

    void ApplyInfo(SpiritSpawnInfo spawnInfo)
    {
        Spawner_Name.GetComponent<Text>().text = spawnInfo.SpawnerName;
        SpawnLv.GetComponentInChildren<Text>().text = spawnInfo.SpwnLv.ToString() + "단계";
        SpiritLv.GetComponentInChildren<Text>().text = spawnInfo.SpiritLv.ToString() + "단계";

        // 타 정령 스폰시간 세팅
    }
    #endregion

    // UI 버튼별 상호작용 case문 작성 예시
    public void CloseTab()
    {
        this.gameObject.SetActive(false);
    }
}
