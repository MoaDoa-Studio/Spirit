using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CradleManager : MonoBehaviour
{
    [Header("UI 요소들")]
    [SerializeField]
    private GameObject cradle;    
    [SerializeField]
    private GameObject cradleGrowthRate;    
    [SerializeField]
    private GameObject[] elementSlider;
    [SerializeField]
    private GameObject cradleGage;

    [Header("스프라이트들")]
    [SerializeField]
    private Sprite[] cradleSprite;
    [SerializeField]
    private Sprite[] cradleGrowthRateSprite;
    [SerializeField]
    private Sprite[] cradleGageSprite;

    TimeManager timeManager;
    // 요소 큐 선언
    Queue<Tuple<int, DateTime>>[] elementQueue = { 
        new Queue<Tuple<int, DateTime>>(), 
        new Queue<Tuple<int, DateTime>>(), 
        new Queue<Tuple<int, DateTime>>(), 
        new Queue<Tuple<int, DateTime>>() 
    };
    private float[] elementAverage = { 0, 0, 0, 0 };
    private int[] elementSum = { 0, 0, 0, 0 };
    private bool[] checkFirstElement = { false, false, false, false };
    TimeSpan span = TimeSpan.FromSeconds(5);    // 성장 속도 계산 시간.

    // 성장 관련 변수들
    private int Level = 0;
    [SerializeField]
    private int GrowthPoint = 0;
    private int GrowthState = 0;
    private int[] GrowthValue = { 50, 25, 10, -40 };
    private float GrowthTime = 0f;
    private float GrowthCooldown = 3f;
    private int[] LevelPoint = { 1500, 2000, 2500, 3000, 5300, 5400, 5500, 6000 };


    // 시간 경과 추적 변수
    private float timeSinceLastAverageCalculation = 0f;
    private const float AverageCalculationInterval = 3f; // 3초 간격
    [SerializeField]
    private bool cradleUIenable = false;
    [SerializeField]
    bool first = false;


    void Start()
    {
        SetCradleMap();
        timeManager = GameObject.Find("TimeNTemperatureManager").GetComponent<TimeManager>();   
    }

    // Update is called once per frame    
    void Update()
    {
        RemoveExpiredElement();

        timeSinceLastAverageCalculation += Time.deltaTime;
        if (timeSinceLastAverageCalculation >= AverageCalculationInterval)
        {
            CalculateElementAverage(); // 3초마다 평균 계산
            timeSinceLastAverageCalculation = 0f; // 타이머 초기화
        }

        // �� ������ ���� �� ���� �� ���Ŀ��� ���ɿ� ���� ǥ��.
        for (int i = 0; i < 4; i++)
        {
            if (!checkFirstElement[i])
                return;
            else
            {
                if(!first)
                StartCoroutine(EnableCradleUIAfterDelay(60 / timeManager.timeSpeed));
               
            }
        }

        if(cradleUIenable)
        {
            AddCradleGrowth();
            UpdateCradleUI();
        }
    }

    // 4가지 요소를 기반으로 성장 추가           
    void AddCradleGrowth()
    {
        GrowthTime += Time.deltaTime;
        if (GrowthTime > GrowthCooldown)
        {
            GrowthPoint += GrowthValue[GrowthState];

            if (GrowthPoint > LevelPoint[Level])
                ToNextCradle();
            else if (GrowthPoint < LevelPoint[Level])
            {
                // ���� ����.
            }            
            
            SetCradleGrowthSlider();
            GrowthTime = 0f;
        }
    }
    private IEnumerator EnableCradleUIAfterDelay(float delay)
    {
        //SetCradleGrowthState(1);
        yield return new WaitForSeconds(delay);
        cradleUIenable = true;
        first = true;
    }
    void SetCradleGrowthSlider()
    {
        if (GrowthPoint < 0)
        {            
            cradleGage.GetComponent<Image>().fillAmount = GrowthPoint * 0.01f * (-1f);
            cradleGage.GetComponent<Image>().sprite = cradleGageSprite[1];
        }
        else
        {            
            cradleGage.GetComponent<Image>().fillAmount = GrowthPoint * 0.01f;
            cradleGage.GetComponent<Image>().sprite = cradleGageSprite[0];
        }
    }

    // 다음 단계로 성장
    public void ToNextCradle()
    {
        Level++;
        GrowthPoint = 0;
        if (Level < 8)
            cradle.transform.Find("CradleImage").GetComponent<Image>().sprite = cradleSprite[Level];
        else
        {
            // ���� Ŭ���� ����.
        }
    }

    // 요소 추가 시 호출되는 메서드
    public void AddElement(string spiritElement, int val)
    {
        int index = -1;
        switch (spiritElement)
        {
            case "Fire":                
                index = 0;
                break;
            case "Water":                
                index = 1;
                break;
            case "Ground":                
                index = 2;
                break;
            case "Air":
                index = 3;
                break;
        }

        if (index == -1)
        {
            Debug.LogError("�߸��� ���� Ÿ���� �Էµ�");
            return;
        }

        if (!checkFirstElement[index])
            checkFirstElement[index] = true;
        elementQueue[index].Enqueue(new Tuple<int, DateTime>(val, DateTime.Now));
        elementSum[index] += val;
    }

    public void RemoveExpiredElement()
    {        
        for (int i = 0; i < 4; i++)
        {            
            while (elementQueue[i].Count != 0)
            {
                // Temp. ���� �ð��� ���� ������ ������ ���� �ð��� 10�� �̳��� ���� �� ��� Break;
                if (DateTime.Now - elementQueue[i].Peek().Item2 < span)
                    break;                
                
                elementSum[i] -= elementQueue[i].Peek().Item1;                
                elementQueue[i].Dequeue();
            }            
        }
    }

    public void CalculateElementAverage()
    {
        for (int i = 0; i < 4; i++)
        {
            if (elementQueue[i].Count == 0)            
                elementAverage[i] = 0f;            
            else
                elementAverage[i] = elementSum[i] / elementQueue[i].Count;
        }
    }
    // UI 업데이트 메서드
    // 성장 상태, 슬라이더 크기, 색상 업데이트
    public void UpdateCradleUI()
    {
        // 성장 상태 설정
        SetCradleGrowthState(GetCradleGrowthRate());

        // 요소 슬라이더 업데이트
        SetElementSliderColor();
        SetElementSliderSize();        
    }


    // 요소 슬라이더 크기 설정
    public void SetElementSliderSize()
    {
        float totalElementAverage = GetTotalElementAverage();
        float offset = 1.3f; // ��ü �����̴��� ���̿� ���� �ٲ�� ��. ��ü �����̴� ���� / 2 / 100 ���� ��.

        for (int i = 0; i < 4; i++)
        {
            RectTransform result = elementSlider[i].transform.GetChild(0).GetComponent<RectTransform>();

            float ratio = 0f; // ���������� ������ ����.
            float x_pos = 0f;
            float width = 0f;            

            ratio = Mathf.Log(Mathf.Abs(elementAverage[i] - totalElementAverage) / totalElementAverage * 1000) * 10;
            width = ratio * offset;
            if (elementAverage[i] - totalElementAverage > 0)
                x_pos = width / 2;
            else if (elementAverage[i] - totalElementAverage < 0)
                x_pos = -width / 2;
            else
                x_pos = 0;

            result.sizeDelta = new Vector2(width, result.rect.height);
            result.anchoredPosition = new Vector2(x_pos, 0);
        }
    }

    // 성장률 계산
    public int GetCradleGrowthRate()
    {
        float totalElementAverage = GetTotalElementAverage();
        int result = 0;
        for (int i = 0; i < 4; i++)
        {
            if (totalElementAverage * 0.9f <= elementAverage[i] && totalElementAverage * 1.1f >= elementAverage[i])            
                result += 1;                            
            else if (totalElementAverage * 0.65f <= elementAverage[i] && totalElementAverage * 1.35f >= elementAverage[i])            
                result += 2;                
            else            
                result += 3;                
        }        
        return result;
    }

    void SetElementSliderColor()
    {
        float totalElementAverage = GetTotalElementAverage();        
        for (int i = 0; i < 4; i++)
        {
            if (totalElementAverage * 0.9f <= elementAverage[i] && totalElementAverage * 1.1f >= elementAverage[i])            
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.green;            
            else if (totalElementAverage * 0.65f <= elementAverage[i] && totalElementAverage * 1.35f >= elementAverage[i])            
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.yellow;            
            else            
                elementSlider[i].transform.GetChild(0).GetComponent<Image>().color = Color.red;            
        }
    }
    void SetCradleGrowthState(int val)
    {
        if (GetTotalElementAverage() == 0)
        {
            GrowthState = 3;
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[3];
            return;
        }

        if (val <= 4)
        {
            GrowthState = 0;
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[0];
        }
        else if (val <= 6)
        {
            GrowthState = 1;
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[1];
        }
        else if (val <= 8)
        {
            GrowthState = 2;
            cradleGrowthRate.GetComponent<Image>().sprite = cradleGrowthRateSprite[2];
        }
        else
        {
            GrowthState = 3;
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
    // ���� ���� �Լ�.

    void SetCradleMap()
    {
        for(int i = 49; i < 54; i++)
        {
            for(int j = 49; j < 54; j++)
            {
                TileDataManager.instance.SetTileType(i, j, 2);
            }
        }
    }
}