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
    [SerializeField]
    GameObject otherSlider;

    [HideInInspector]
    public Slider slider;
    [HideInInspector]
    public InputField textcoom;
    public GameObject controllingSpawn;
    private List<SpiritSpawnInfo> spawnInfoList = new List<SpiritSpawnInfo>();
    List<GameObject> spawnManage = new List<GameObject>();
    private void Start()
    {
        AddListOfSpawner();
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
    void AddListOfSpawner()
    {
        spawnManage.Add(Sprit1);
        spawnManage.Add(Sprit2);
        spawnManage.Add(Sprit3);
    }
    public void ReceiveSpiritSpawnInfo(SpiritSpawnInfo spawnInfo)
    {
        spawnInfoList.RemoveAll(item => item.SpawnerName == spawnInfo.SpawnerName);
        spawnInfoList.Add(spawnInfo);
        ApplyNowSpawnInfo(spawnInfo);
        ApplyOtherSpawnInfo(spawnInfo);
        
    }
    // 클릭한 정령 생산소 정보 입력.
    void ApplyNowSpawnInfo(SpiritSpawnInfo spawnInfo)
    {
        Spawner_Name.GetComponent<Text>().text = spawnInfo.SpawnerName;
        SpawnLv.GetComponentInChildren<Text>().text = spawnInfo.SpwnLv.ToString() + "단계";
        SpiritLv.GetComponentInChildren<Text>().text = spawnInfo.SpiritLv.ToString() + "단계";


    }

    // 타 정령 생산소 스폰시간 세팅
    void ApplyOtherSpawnInfo(SpiritSpawnInfo newspawnInfo)
    {
        List<SpiritSpawnInfo> list = new List<SpiritSpawnInfo>();

        foreach (var item in spawnInfoList)
        {
            Debug.Log(spawnInfoList.Count);
            if (item.SpawnerName != newspawnInfo.SpawnerName)
            {
                list.Add(item);
            }

        }

       for(int i = 0; i < spawnManage.Count; i++)
       {
            spawnManage[i].GetComponent<Text>().text = list[i].SpawnerName + "의 정령";
           
       }
    }


  
    void OnClick(SpiritSpawnInfo spawnInfo)
    {
        // 배치 시킬 UI 생성 이후 setactive 호출
        otherSlider.SetActive(true);
        otherSlider.GetComponentInChildren<Slider>().value = spawnInfo.slider.value;
    }
    #endregion

    // UI 버튼별 상호작용 case문 작성 예시
    public void CloseTab()
    {
        this.gameObject.SetActive(false);
    }
}
