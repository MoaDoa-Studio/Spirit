using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class QuestManager : MonoBehaviour
{   
    public GameObject QuestPrefab;
    public Transform QuestUI_Transform;
    public List<GameObject> QuestList;
    // 퀘스트 창 생성 (기능 부여는 eventHandler로 해보자)
    public void InstantiateQuest(string name, string explain, string progress)
    {
        GameObject clone = Instantiate(QuestPrefab, QuestUI_Transform);
        clone.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        clone.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = explain;
        clone.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = progress;        
        QuestList.Add(clone);        
        clone = null;
    }

    public void ExpandQuest()
    {

    }
    public void ClearQuest(int idx)
    {        
        // 보상 지급 로직

        //
        Destroy(QuestList[idx]);
        QuestList.RemoveAt(idx);
    }

    // For Debug
    public void GainItem(int targetNum, int count)
    {
        // targetNum의 아이템을 count개 획득했다.        
        Stack<int> clearIndex = new Stack<int>(); 
        for(int i = 0; i < QuestList.Count; i++)
        {
            Quest quest = QuestList[i].GetComponent<Quest>();
            if (quest.ConditionTarget == targetNum)
            {
                quest.CurrentConditionAchieve += count;
                if (quest.CheckClear())
                    clearIndex.Push(i);
            }
        }

        while (clearIndex.Count != 0)
        {
            ClearQuest(clearIndex.Pop());
        }
        UpdateQuestUI();
    }
    
    public void UpdateQuestUI()
    {        
    }

    Quest quest1 = new Quest(0, "Quest1 Name", "Quest1 Body", "Quest1 ConditionText", 0, 10, 60, 10);    
    Quest quest2 = new Quest(0, "Quest2 Name", "Quest2 Body", "Quest2 ConditionText", 0, 10, 60, 10);
    public void Start()
    {
        quest1.SetCondition(0, 0, 5f);
        quest2.SetCondition(1, 1, 10f);
    }    
}
