using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class QuestPrefab : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Quest currentQuest;
    QuestManager questManager;
    ResouceManager ResouceManager;
    private int remainTime;

    public int CurrentConditionAchieve;
    
    [SerializeField]
    private TextMeshProUGUI TimeText;

    private readonly float[] QuestPrefabHeight = new float[] { 50, 180 };
    private readonly float[] QuestNamePos = new float[] { 0, 70 };
    
    private bool isCleared;
    private float time;

    private int QuestID;

    // For Debug.
    public void Start()
    {
        CurrentConditionAchieve = 0;
        isCleared = false;        
        time = 0f;
        
        TimeText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        questManager = GameObject.Find("QuestManager").GetComponent<QuestManager>();
        ResouceManager = GameObject.Find("[ResourceManager]").GetComponent <ResouceManager>();
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
                if (remainTime <= 0)
                    FailQuest();
                else
                    remainTime -= 1;
            }
        }
    }

    // Todo. 데이터 테이블 불러오는 기능 구현되면 데이터 ID 값을 통해 모든 정보 불러올 수 있도록 수정할거.
    Quest GetQuest(int QuestID)
    {
        if (!DatabaseManager.instance.Quests.ContainsKey(QuestID))
            return null;
        return DatabaseManager.instance.Quests[QuestID];
    }    
    public void SetQuest(int questID)
    {
        Quest quest = GetQuest(questID);
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = quest.QuestName;
        transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = quest.QuestBody;
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = quest.QuestConditionMent;
        QuestID = quest.QuestID;
        remainTime = quest.QuestClearTime;

        SetTime();
    }    
    public void SetTime()
    {
        int min = remainTime / 60;
        int sec = remainTime % 60;        
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
        // 연구소 설치 퀘스트
        if (QuestManager.instance.Research)
        {
            if (QuestID == 1002)
            {
                if (QuestManager.instance.researchSettlement > 0)
                {
                    Debug.Log("성공햇잖이");

                    // 성공 보수 주어지면 될듯
                    QuestManager.instance.Research = false;
                    return true;
                }
            }
        }

        if (QuestManager.instance.rain)
        {
            if (QuestID == 1001)    // 폭우 피해 축소
            {
                if (QuestManager.instance.OverRain)
                {
                    QuestManager.instance.rain = false;
                    return true;
                }
            }
        }

        if (QuestManager.instance.Spirit)
        {
            if (QuestID == 1003)    // 정령왕의 성장기
            {
                if (QuestManager.instance.SpiritKing)
                {
                    ResouceManager.Essence_reserves += 1;
                    QuestManager.instance.Spirit = false;
                    return true;
                }
            }
        }

        if (QuestManager.instance.GainR)
        {
            if (QuestID == 1004)    // 자원 흭득
            {
                if (QuestManager.instance.GainResource)
                {
                    ResouceManager.Timber_reserves += 100;
                    // 저장소 건설 연달은 퀘스트
                    QuestManager.instance.GainR = false;
                    QuestManager.instance.InstantiateQuest(1005);
                    QuestManager.instance.Storage = true;
                    return true;
                }
            }
        }

        if (QuestManager.instance.Storage)
        {
            if (QuestID == 1005)    // 저장소 건설
            {
                if (QuestManager.instance.StorageSettlement)
                {
                    QuestManager.instance.Storage = false;
                    return true;
                }
            }
        }

        if (QuestManager.instance.ResearchMode)
        {
            if (QuestID == 1006)    // 연구소 2단계 해금
            {
                if (QuestManager.instance.ResearchMode2)
                {
                    QuestManager.instance.ResearchMode = false;
                    return true;
                }
            }
        }

        return false; // 모든 조건에 해당하지 않으면 false 반환
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
