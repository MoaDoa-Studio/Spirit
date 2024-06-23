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

    public void InstantiateQuest(string name, string explain, string progress)
    {
        GameObject clone = Instantiate(QuestPrefab, QuestUI_Transform);
        QuestPrefab quest = clone.GetComponent<QuestPrefab>();
        quest.SetQuest(0, name, explain, progress, 0, 10, 30, 10);                        

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


    // For Debug.
    public void TempInstantiateQuest()
    {
        GameObject clone = Instantiate(QuestPrefab, QuestUI_Transform);
        QuestPrefab quest = clone.GetComponent<QuestPrefab>();
        quest.SetQuest(0, "Quest1", "Quest1 Body", "Quest1 ConditionText", 0, 10, 10, 10);
        
        clone = null;

        UpdateQuestUI();
    }
    // For Debug
    public void GainItem(int targetNum, int count)
    {
        // targetNum¿« æ∆¿Ã≈€¿ª count∞≥ »πµÊ«ﬂ¥Ÿ.        
        Stack<int> clearIndex = new Stack<int>();
        for (int i = 0; i < QuestUI_Transform.childCount; i++)
        {
            QuestPrefab quest = QuestUI_Transform.GetChild(i).GetComponent<QuestPrefab>();
            if (quest.ConditionTarget == targetNum)
            {
                quest.CurrentConditionAchieve += count;
                if (quest.CheckClear())
                    clearIndex.Push(i);
            }
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
