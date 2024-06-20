using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance = null;
    public Dictionary<int, Study> Studies;

    [SerializeField]
    string TaskFileName;
    void Awake()
    {
        if (instance == null)
            instance = this; 
        else
            Destroy(this);

        Studies = RefineTaskData(ReadData("Studies"));
    }
    

    // CSV파일을 읽어 반환합니다.
    protected List<Dictionary<string, object>> ReadData(string CSVName)
    {
        return CSVReader.Read(CSVName);
    }

    // 대화 CSV파일을 정리하여 딕셔너리로 반환합니다.
    protected Dictionary<int, Study> RefineTaskData(List<Dictionary<string, object>> texts)
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
}
