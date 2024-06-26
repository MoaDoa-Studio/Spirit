using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance = null;
    public Dictionary<int, Study> Studies = new Dictionary<int, Study>();
    public Dictionary<int, Effect> Effects = new Dictionary<int, Effect>();
    public Dictionary<int, Quest> Quests = new Dictionary<int, Quest>();

    public string StudyTableFileName;
    public string EffectTableFileName;
    public string QuestTableFileName;
    void Awake()
    {
        if (instance == null)
            instance = this; 
        else
            Destroy(this);

        Studies = RefineStudyData(ReadData(StudyTableFileName));
        Effects = RefineEffectData(ReadData(EffectTableFileName));
        Quests = RefineQuestData(ReadData(QuestTableFileName));
    }
    

    // CSV파일을 읽어 반환합니다.
    protected List<Dictionary<string, object>> ReadData(string CSVName)
    {
        return CSVReader.Read(CSVName);
    }

    // 연구 CSV파일을 정리하여 딕셔너리로 반환합니다.
    protected Dictionary<int, Study> RefineStudyData(List<Dictionary<string, object>> texts)
    {
        Dictionary<int, Study> temp = new Dictionary<int, Study>();
        for (int i = 0; i < texts.Count; i++)
        {
            Study tempStudy = new Study();
            tempStudy.StudyID = int.Parse(texts[i]["StudyID"].ToString());
            tempStudy.StudyName = texts[i]["StudyName"].ToString();
            tempStudy.KindofStudy = texts[i]["KindofStudy"].ToString() == "TRUE" ? true : false;
            tempStudy.CategoryofStudy = int.Parse(texts[i]["CategoryofStudy"].ToString());
            tempStudy.StudyContent = texts[i]["StudyContent"].ToString();
            tempStudy.PhaseofStudy = int.Parse(texts[i]["PhaseofStudy"].ToString());
            tempStudy.StoneRequirement = int.Parse(texts[i]["StoneRequirementS"].ToString());
            tempStudy.WoodRequirement = int.Parse(texts[i]["WoodRequirementS"].ToString());
            tempStudy.EssenceRequirement = int.Parse(texts[i]["EssenceRequirementS"].ToString());
            tempStudy.WorkRequirement = int.Parse(texts[i]["WorkRequirementS"].ToString());
            tempStudy.StudyEffect = int.Parse(texts[i]["StudyEffect"].ToString());
            tempStudy.PriorResearch = texts[i]["PriorResearch"].ToString() == "null" ? 0 : int.Parse(texts[i]["PriorResearch"].ToString());            
            temp.Add(tempStudy.StudyID, tempStudy);
        }
        return temp;
    }

    // 효과 CSV
    protected Dictionary<int, Effect> RefineEffectData(List<Dictionary<string, object>> texts)
    {
        Dictionary<int, Effect> temp = new Dictionary<int, Effect>();
        for (int i = 0; i < texts.Count; i++)
        {
            Effect tempStudy = new Effect();            
        }
        return temp;
    }

    // 퀘스트 CSV
    protected Dictionary<int, Quest> RefineQuestData(List<Dictionary<string, object>> texts)
    {
        Dictionary<int, Quest> temp = new Dictionary<int, Quest>();
        for (int i = 0; i < texts.Count; i++)
        {
            Quest tempQuest = new Quest();
            tempQuest.QuestID = int.Parse(texts[i]["QuestID"].ToString());
            tempQuest.QuestName = texts[i]["QuestName"].ToString();
            tempQuest.QuestBody = texts[i]["QuestBody"].ToString();
            tempQuest.QuestCondition = int.Parse(texts[i]["QuestCondition"].ToString());
            tempQuest.QuestClearCondition = int.Parse(texts[i]["QuestCCondition"].ToString());
            tempQuest.QuestConditionMent = texts[i]["QuestConditionMent"].ToString();
            tempQuest.QuestClearTime = int.Parse(texts[i]["QuestClearTime"].ToString());
            tempQuest.QuestReward = int.Parse(texts[i]["QuestReward"].ToString());
            tempQuest.QuestDisadvantage = int.Parse(texts[i]["QuestDisadvantage"].ToString());
            temp.Add(tempQuest.QuestID, tempQuest);
        }
        return temp;
    }
}
