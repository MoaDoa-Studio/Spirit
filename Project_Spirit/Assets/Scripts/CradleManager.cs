using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
public class CradleManager : MonoBehaviour
{
    [Header("오브젝트")]
    [SerializeField]
    private GameObject cradleState;    
    [SerializeField]
    private GameObject cradleGrowthRate;    
    [SerializeField]
    private GameObject[] elementSlider;

    [Header("스프라이트")]
    [SerializeField]
    private Sprite[] cradleSprite;
    [SerializeField]
    private Sprite[] cradleGrowthRateSprite;

    private int cradleLevel = 0;        

    // 원소 값 계산용 변수.
    // 불, 물, 땅, 공기.
    Queue<Tuple<int, DateTime>>[] elementQueue = { 
        new Queue<Tuple<int, DateTime>>(), 
        new Queue<Tuple<int, DateTime>>() , 
        new Queue<Tuple<int, DateTime>>() , 
        new Queue<Tuple<int, DateTime>>() 
    };
    public float[] elementAvarage = { 0, 0, 0, 0 };
    public int[] elementSum = { 0, 0, 0, 0 };
    TimeSpan span = TimeSpan.FromSeconds(10); // 인게임 내 1일의 실제 시간 값.

    // For Debug.
    [SerializeField]
    private GameObject DebuggingBtn;
    [SerializeField]
    private TextMeshProUGUI[] DebuggingText;
    void Start()
    {
        // For Debug.
        DebuggingBtn.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Fire", 10));
        DebuggingBtn.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Water", 10));
        DebuggingBtn.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Ground", 10));
        DebuggingBtn.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Air", 10));
        DebuggingBtn.transform.GetChild(4).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Fire", 50));
        DebuggingBtn.transform.GetChild(5).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Water", 20));
        DebuggingBtn.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Ground", 20));
        DebuggingBtn.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(() => AddSpiritToCradle("Air", 20));
    }

    // Update is called once per frame    
    void Update()
    {
        RemoveExpiredElement();
        CalculateElementAvarage();
        UpdateCradleUI();

        // For Debug.
        SetDebuggingText();
    }

    // 4원소 성장 게이지 관리 변수.
    // 정령왕 성장 게이지 변수.            

    // 정령왕 레벨업 함수.
    public void ToNextCradle()
    {
        cradleLevel++;
        if (cradleLevel >= 8)
        {
            // 게임 클리어 판정.
        }        
    }

    // 정령이 요람에 부딪혔을 경우 호출되는 함수 설계.
    public void AddSpiritToCradle(string name, int val)
    {
        switch (name)
        {
            case "Fire":                
                elementQueue[0].Enqueue(new Tuple<int, DateTime>(val, DateTime.Now));
                Debug.Log(DateTime.Now);
                elementSum[0] += val;                
                break;
            case "Water":
                elementQueue[1].Enqueue(new Tuple<int, DateTime>(val, DateTime.Now));
                elementSum[1] += val;                
                break;
            case "Ground":
                elementQueue[2].Enqueue(new Tuple<int, DateTime>(val, DateTime.Now));
                elementSum[2] += val;                
                break;
            case "Air":
                elementQueue[3].Enqueue(new Tuple<int, DateTime>(val, DateTime.Now));
                elementSum[3] += val;                
                break;
        }
    }

    public void RemoveExpiredElement()
    {        
        for (int i = 0; i < 4; i++)
        {            
            while (elementQueue[i].Count != 0)
            {
                // Temp. 현재 시간과 가장 과거의 원소의 진입 시간이 10초 이내로 차이 날 경우 Break;
                if (DateTime.Now - elementQueue[i].Peek().Item2 < span)
                    break;
                Debug.Log("파괴 " + DateTime.Now);
                elementSum[i] -= elementQueue[i].Peek().Item1;
                Debug.Log(i + "원소 " + elementQueue[i].Peek().Item1);
                elementQueue[i].Dequeue();
            }            
        }
    }

    public void CalculateElementAvarage()
    {
        for (int i = 0; i < 4; i++)
        {
            if (elementQueue[i].Count == 0)            
                elementAvarage[i] = 0f;            
            else
                elementAvarage[i] = elementSum[i] / elementQueue[i].Count;
        }
    }
    // 하단 UI 갱신 함수.
    // 갱신 목록, 각 원소별 성장 속도 게이지, 정령 성장 속도 마크, 정령왕의 성장 게이지, 정령왕 이미지
    // 서서히 올라가는 게이지는 코루틴 이용해서 설계.
    public void UpdateCradleUI()
    {
        if (cradleLevel < 8)
            cradleState.GetComponent<Image>().sprite = cradleSprite[cradleLevel];

        ShowSpiritSlider(); // 정령 값 슬라이더.
        CalculateCradleGrowthRate(); // 요람 성장 속도 계산.
    }

    // 원소별 성장 속도 게이지 표시
    public void ShowSpiritSlider()
    {
        float totalElementAverage = GetTotalElementAverage();
        for (int i = 0; i < 4; i++)
        {
            RectTransform result = elementSlider[i].transform.GetChild(0).GetComponent<RectTransform>();
            float val = elementAvarage[i] - totalElementAverage;
            
            float x_pos = val;
            float width = val > 0 ? val * 2 : val * (-2);
            result.sizeDelta = new Vector2(width, result.rect.height);
            result.anchoredPosition = new Vector2(x_pos, 0);
        }
    }

    // 정령왕의 성장 속도 계산 및 판정 함수.
    public void CalculateCradleGrowthRate()
    {
        float totalElementAverage = GetTotalElementAverage();
        int result = 0;

        for (int i = 0; i < 4; i++)
        {
            if (totalElementAverage * 0.9f <= elementAvarage[i] && totalElementAverage * 1.1f >= elementAvarage[i])
            {
                result += 1;
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.green;
            }
            else if (totalElementAverage * 0.65f <= elementAvarage[i] && totalElementAverage * 1.35f >= elementAvarage[i])
            {
                result += 2;
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.yellow;
            }
            else
            {
                result += 3;
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.red;
            }
        }

        if (result <= 4)
        {        
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[0];
        }
        else if (result <= 6)
        {            
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[1];
        }
        else if (result <= 8)
        {         
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[2];
        }
        else
        {            
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[3];
        }
    }

    float GetTotalElementAverage()
    {
        int sum = 0, count = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += elementSum[i];
            count += elementQueue[i].Count;
        }
        if (count == 0)
            return 0;
        return sum / count;
    }
    // 원소 조화, 부조화 판정 함수.

    // 게임 오버 함수.    

    // For Debug.
    void SetDebuggingText()
    {
        DebuggingText[0].text = "Total : " + GetTotalElementAverage().ToString();
        DebuggingText[1].text = "Fire : " + elementAvarage[0].ToString();
        DebuggingText[2].text = "Water : " + elementAvarage[1].ToString();
        DebuggingText[3].text = "Ground : " + elementAvarage[2].ToString();
        DebuggingText[4].text = "Air : " + elementAvarage[3].ToString();
    }
}