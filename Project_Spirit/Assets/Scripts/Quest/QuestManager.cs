using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class QuestManager : MonoBehaviour
{
    public static QuestManager instance = null;

    public GameObject QuestPrefab;
    public Transform QuestUI_Transform;        

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
    public void GainItem(int targetNum, int count)
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
