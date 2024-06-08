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
        Quest quest = clone.GetComponent<Quest>();
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
        Quest quest = clone.GetComponent<Quest>();
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
            Quest quest = QuestUI_Transform.GetChild(i).GetComponent<Quest>();
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
            QuestUI_Transform.transform.GetChild(index).GetComponent<Quest>().ClearQuest();
        }
        UpdateQuestUI();
    }

    public void GainTree(int count)
    {
        GainItem(0, count);
    }

    // For Debug
    public void Start()
    {
        InstantiateQuest("Quest2", "Quest2 Body", "Quest2 Tree 10");
    }

    public void Update()
    {
        UpdateQuestUI();
    }
}
