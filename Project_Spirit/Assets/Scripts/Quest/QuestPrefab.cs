using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class QuestPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{    
    public int QuestID;
    public string QuestName;
    public string QuestBody;
    public string QuestConditionMent;
    public int QuestCondition;
    public int QuestClearCondition;
    public int QuestClearTime;
    public int QuestReward;
    public int QuestDisadvantage;

    public int ConditionID;
    public int ConditionTarget;
    public float ConditionStandard;

    public int CurrentConditionAchieve;
    
    [SerializeField]
    private TextMeshProUGUI TimeText;

    private readonly float[] QuestPrefabHeight = new float[] { 50, 100 };
    private readonly float[] QuestNamePos = new float[] { 0, 25 };
    
    private bool isCleared;
    private float time;
    
    // For Debug.
    public void Start()
    {
        CurrentConditionAchieve = 0;
        isCleared = false;
        time = 0f;
        TimeText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void Update()
    {
        if (isCleared == false)
        {
            time += Time.deltaTime;
            if (time >= 1f)
            {
                time = 0f;
                SetTime();
                if (QuestClearTime <= 0)
                    FailQuest();
                else
                    QuestClearTime -= 1;
            }
        }
    }

    // Todo. 데이터 테이블 불러오는 기능 구현되면 데이터 ID 값을 통해 모든 정보 불러올 수 있도록 수정할거.
    public void SetQuest(int _id, string _name, string _body, string _conditionText,
        int _condition, int _clearCondition, int _clearTime, int _disadvantage)
    {
        QuestID = _id;
        QuestName = _name;
        QuestBody = _body;
        QuestConditionMent = _conditionText;
        QuestCondition = _condition;
        QuestClearCondition = _clearCondition;
        QuestClearTime = _clearTime;
        QuestDisadvantage = _disadvantage;

        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = _name;
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = _body;
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = _conditionText;
        SetTime();
    }
    public void SetQuest(int questID)
    {
        
    }
    public void SetCondition(int _cID, int _cTarget, float _cStandard)
    {
        ConditionID = _cID;
        ConditionTarget = _cTarget;
        ConditionStandard = _cStandard;
    }
    public void SetTime()
    {
        int min = QuestClearTime / 60;
        int sec = QuestClearTime % 60;        
        string _min = min.ToString();
        string _sec = sec.ToString();        
        if (_min.Length < 2)        
            _min = "0" + _min;
        if (_sec.Length < 2)
            _sec = "0" + _sec;

        TimeText.text = _min + ":" + _sec;
    }
    public bool CheckClear()
    {
        if (CurrentConditionAchieve >= ConditionStandard)
            return true;
        return false;
    }

    public void ClearQuest()
    {
        // 보상 지급 로직

        //
        isCleared = true;
        SetQuestSize(1);
        transform.GetComponent<Image>().color = Color.green;
        Destroy(this.gameObject, 1f);        
    }
    public void FailQuest()
    {
        // 디스어드밴티지

        //
        SetQuestSize(1);
        transform.GetComponent<Image>().color = Color.red;
        Destroy(this.gameObject, 1f);
    }    

    public void OnPointerEnter(PointerEventData eventData)
    {        
        SetQuestSize(1);
    }
    public void OnPointerExit(PointerEventData eventData)
    {        
        SetQuestSize(0);        
    }
    void SetQuestSize(int num)
    {        
        transform.GetComponent<RectTransform>().sizeDelta = new Vector2(300, QuestPrefabHeight[num]);
        transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(-49.2f, QuestNamePos[num]);
        transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(101.9f, QuestNamePos[num]);
        
        if (num == 0)
        {
            transform.GetChild(2).gameObject.SetActive(false);
            transform.GetChild(3).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(2).gameObject.SetActive(true);
            transform.GetChild(3).gameObject.SetActive(true);
        }

        QuestManager.instance.UpdateQuestUI();
    }    
}
