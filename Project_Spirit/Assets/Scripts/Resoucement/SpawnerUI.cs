using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnerUI : MonoBehaviour
{
    [Header("정령 생산소 UI 세팅")]
    [SerializeField]
    Slider[] allSliders;
    [SerializeField]
    GameObject Spawner_Name;
    [SerializeField]
    GameObject SpawnLv;
    [SerializeField]
    GameObject SpiritLv;
    [SerializeField]
    GameObject otherSlider;
    [SerializeField]
    GameObject[] spirit123;
    [SerializeField]
    Transform parentSliderPos;
    [SerializeField]
    GameObject[] showSlider;
    [SerializeField]
    GameObject MainSlider;
   
    public Button[] buttons;
    
    public Slider slider;
    
    public TMP_InputField textcoom;
    
    public GameObject[] Spawner;
    
    public Sprite[] HandleSprite;
    public Sprite[] HandleSubSprite;
    
    GameObject MainSpawner;

    private List<SpiritSpawnInfo> spawnInfoList = new List<SpiritSpawnInfo>();
    private List<SpiritSpawnInfo> notspawnInfoList = new List<SpiritSpawnInfo>();
    bool isSlider1Active = false;
    bool isSlider2Active = false;
    bool isSlider3Active = false;
    bool isSliderAllActive = false;
    
    List<GameObject> spawnManage = new List<GameObject>(); // 정령 생산 전체 리스트
    Canvas canvas;

    private void Start()
    {
        slider.onValueChanged.AddListener(updateText);
        foreach (Slider slider in allSliders)
        {
            slider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        foreach(Button button in buttons)
        {
            button.onClick.AddListener(() => OnButtonClick(button));
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
            MainSpawner.GetComponent<SpiritSpawner>().sliderValue = value;
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
            Debug.Log(inputValue);
            if(MainSpawner != null)
            {
                Debug.Log(MainSpawner);
                MainSpawner.GetComponent<SpiritSpawner>().sliderValue = inputValue;
                Debug.Log(MainSpawner.GetComponent<SpiritSpawner>().sliderValue);
            }
        }
    }

    #endregion

    #region UI 세팅
   
    public void ReceiveDefaultSpiritSpawnInfo(SpiritSpawnInfo spawnInfo)
    {
        UpdateSpawnInfoList(spawnInfo);
        
    }

    public void ReceiveSpiritSpawnInfo(SpiritSpawnInfo spawnInfo, GameObject gameObject)
    {
        // 업데이트 정령 생산소 리스트
        UpdateSpawnInfoList(spawnInfo);
        MainSpawner = gameObject;
        slider.value = MainSpawner.GetComponent<SpiritSpawner>().sliderValue;
        ApplyNowSpawnInfo(spawnInfo);
        ApplyOtherSpawnInfo(spawnInfo);
    }

    void UpdateSpawnInfoList(SpiritSpawnInfo newSpawnInfo)
    {
        // 새로운 정보와 동일한 spawnerName을 가진 요소를 찾습니다.
        SpiritSpawnInfo existingSpawnInfo = spawnInfoList.Find(item => item.SpawnerName == newSpawnInfo.SpawnerName);
        if (existingSpawnInfo != null)
        { 
            existingSpawnInfo.UpdateFrom(newSpawnInfo);
        }
        
        else
        {
            // 존재하지 않는 경우 새로운 요소를 추가합니다.
            spawnInfoList.Add(newSpawnInfo);
        }
    }



    // 클릭한 정령 생산소 정보 입력.
    void ApplyNowSpawnInfo(SpiritSpawnInfo spawnInfo)
    {
        Spawner_Name.GetComponent<TextMeshProUGUI>().text = spawnInfo.SpawnerName + "의 생산소";
        SpawnLv.GetComponentInChildren<TextMeshProUGUI>().text = spawnInfo.SpwnLv.ToString() + "단계";
        SpiritLv.GetComponentInChildren<TextMeshProUGUI>().text = spawnInfo.SpiritLv.ToString() + "단계";
        ChangeMainHandleImageInChildren(MainSlider.transform, spawnInfo.elementNum);
    }

    // 타 정령 생산소 스폰시간 세팅
    void ApplyOtherSpawnInfo(SpiritSpawnInfo newspawnInfo)
    {
        // 호출할때마다 새롭게 정리
        notspawnInfoList.Clear();

        for(int i = 0; i < 4; i++)
        {
            if (spawnInfoList[i].SpawnerName != newspawnInfo.SpawnerName)
            {
                notspawnInfoList.Add(spawnInfoList[i]);
            }            
        }
        for(int i = 0; i < 3; i++)
        {
            spirit123[i].GetComponent<TextMeshProUGUI>().text = notspawnInfoList[i].SpawnerName + "의 정령";

        }
        
    }


  
    #endregion

    
    void OnButtonClick(Button clickedButton)
    {
        switch(clickedButton.name)
        {
            case "0":
                ClickSpawner1(0);
                break;
            case "1":
                ClickSpawner2(1);
                break;
            case "2":
                ClickSpawner3(2);
                break;
        }
    }

    public void ClickAll()
    {
        isSliderAllActive = !isSliderAllActive;
        if(isSliderAllActive)
        {
            isSlider1Active = true;
            isSlider2Active = true;
            isSlider3Active = true;
            for (int i = 0; i < notspawnInfoList.Count; i++)
            {
                int selected = Output(notspawnInfoList[i].SpawnerName);

                showSlider[selected].SetActive(true);
                Slider[] tempsliders = showSlider[selected].GetComponentsInChildren<Slider>();
                foreach (Slider slider in tempsliders)
                {
                    slider.value = Spawner[selected].GetComponent<SpiritSpawner>().sliderValue;

                }
                // 생성된 오브젝트의 하위 오브젝트들을 모두 가져옵니다.
                ChangeHandleImageInChildren(showSlider[selected].transform, selected);
            }
        }
        else
        {
            isSlider1Active = false;
            isSlider2Active = false;
            isSlider3Active = false;

            for(int i = 0; i < showSlider.Length; i++)
            {
                showSlider[i].SetActive(false);
            }
        }

    }
    private void ClickSpawner1(int num) 
    {
        isSlider1Active = !isSlider1Active;
        // 배치 시킬 UI 생성 이후 setactive 호출
       // otherSlider.SetActive(isSlider1Active);
        if(isSlider1Active)
        {
            int selected = Output(notspawnInfoList[num].SpawnerName);

            showSlider[selected].SetActive(true);
            Slider[] tempsliders = showSlider[selected].GetComponentsInChildren<Slider>();
            foreach(Slider slider in tempsliders)
            {
                slider.value = Spawner[selected].GetComponent<SpiritSpawner>().sliderValue;

            }
            // 생성된 오브젝트의 하위 오브젝트들을 모두 가져옵니다.
            ChangeHandleImageInChildren(showSlider[selected].transform, selected);
        }
        else
        {
            int selected = Output(notspawnInfoList[num].SpawnerName);
            showSlider[selected].SetActive(false);
        }

    }

    private void ClickSpawner2(int num)
    {
        isSlider2Active = !isSlider2Active;
        // 배치 시킬 UI 생성 이후 setactive 호출
        //otherSlider.SetActive(isSlider2Active);
        if (isSlider2Active)
        {
            int selected = Output(notspawnInfoList[num].SpawnerName);
            showSlider[selected].SetActive(true);
            Slider[] tempsliders = showSlider[selected].GetComponentsInChildren<Slider>();
            
            foreach (Slider slider in tempsliders)
            {
                slider.value = Spawner[selected].GetComponent<SpiritSpawner>().sliderValue;

            }
            // 생성된 오브젝트의 하위 오브젝트들을 모두 가져옵니다.
            ChangeHandleImageInChildren(showSlider[selected].transform, selected);
        }
        else
        {
            int selected = Output(notspawnInfoList[num].SpawnerName);
            showSlider[selected].SetActive(false);
        }
    }

    private void ClickSpawner3(int num)
    {
        isSlider3Active = !isSlider3Active;
        // 배치 시킬 UI 생성 이후 setactive 호출
       // otherSlider.SetActive(isSlider3Active);
        if (isSlider3Active)
        {
            int selected = Output(notspawnInfoList[num].SpawnerName);
            showSlider[selected].SetActive(true);
            Slider[] tempsliders = showSlider[selected].GetComponentsInChildren<Slider>();
          
            foreach (Slider slider in tempsliders)
            {
                slider.value = Spawner[selected].GetComponent<SpiritSpawner>().sliderValue;
            }

            // 생성된 오브젝트의 하위 오브젝트들을 모두 가져옵니다.
            ChangeHandleImageInChildren(showSlider[selected].transform, selected);
            // 버튼 모양 바뀌는 거 추가

        }
        else
        {
            int selected = Output(notspawnInfoList[num].SpawnerName);
            showSlider[selected].SetActive(false);
        }

    }
    void ChangeMainHandleImageInChildren(Transform parent, int selected)
    {
        foreach (Transform child in parent)
        {
            // 하위 오브젝트의 이름이 "Handle"이면 이미지를 변경합니다.
            if (child.name == "Handle")
            {
                Image handleImage = child.GetComponent<Image>();
                if (handleImage != null)
                {
                    handleImage.sprite = HandleSubSprite[selected];
                }
            }

            // 하위 오브젝트가 또 다른 하위 오브젝트를 가지고 있는지 재귀적으로 탐색합니다.
            ChangeMainHandleImageInChildren(child, selected);
        }
    }

    void ChangeHandleImageInChildren(Transform parent, int selected)
    {
        foreach (Transform child in parent)
        {
            // 하위 오브젝트의 이름이 "Handle"이면 이미지를 변경합니다.
            if (child.name == "Handle")
            {
                Image handleImage = child.GetComponent<Image>();
                if (handleImage != null)
                {
                    handleImage.sprite = HandleSubSprite[selected];
                }
            }

            // 하위 오브젝트가 또 다른 하위 오브젝트를 가지고 있는지 재귀적으로 탐색합니다.
            ChangeHandleImageInChildren(child, selected);
        }
    }


    int Output(string name)
    {
        int num = 0;
        for(int i = 0; i < 4; i++)
        {
            if (Spawner[i].GetComponent<SpiritSpawner>().SpawnerName == name)
            {
                num = i;
                break;
            }
           
        }
        return num;
    }

   

    // UI 버튼별 상호작용 case문 작성 예시
    public void CloseTab()
    {
        this.gameObject.SetActive(false);
    }
}
