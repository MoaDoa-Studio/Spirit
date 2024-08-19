using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance = null;

    public GameObject QuestPrefab;
    public Transform QuestUI_Transform;

    // 퀘스트 시작 체크
    public bool Research;
    public bool rain;
    public bool GainR;
    public bool Storage;
    public bool ResearchMode;
    public bool Spirit;

    // 퀘스트 클리어 조건
    public int researchSettlement;  // 연구소 설치
    public bool OverRain;       // 폭우 기간
    public bool GainResource;   // 자원 획득
    public bool StorageSettlement; // 저장소 건설
    public bool ResearchMode2;  // 연구소 2단계 해금
    public bool SpiritKing; // 정령왕의 성장기


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }
    
    public void InstantiateQuest(int QuestID)
    {
        GameObject clone = Instantiate(QuestPrefab, QuestUI_Transform);
        QuestPrefab quest = clone.GetComponent<QuestPrefab>();
        quest.SetQuest(QuestID);

        UpdateQuestUI();
    }        
    
    public void UpdateQuestUI()
    {
        float y_pos = 0;
        for (int i = 0; i < QuestUI_Transform.childCount; i++)
        {
            RectTransform childRect = QuestUI_Transform.GetChild(i).GetComponent<RectTransform>();
            float offset = childRect.sizeDelta.y / 2;
            y_pos -= offset;
            childRect.anchoredPosition = new Vector2(0, y_pos);
            y_pos -= offset;
        }
    }
        
    // For Debug. 이후, 구독 발행 모델로 퀘스트 컨디션 체크하면 될듯.
    public void GainItem()
    {
        // targetNum의 아이템을 count개 획득했다.        
        Stack<int> clearIndex = new Stack<int>();
        for (int i = 0; i < QuestUI_Transform.childCount; i++)
        {
            QuestPrefab quest = QuestUI_Transform.GetChild(i).GetComponent<QuestPrefab>();
            if (quest.CheckClear())
                clearIndex.Push(i);
        }

        while (clearIndex.Count != 0)
        {
            int index = clearIndex.Pop();            
            QuestUI_Transform.transform.GetChild(index).GetComponent<QuestPrefab>().ClearQuest();
        }
        UpdateQuestUI();
    }    

    // For Debug    

    public void Update()
    {
        UpdateQuestUI();
    }
}
