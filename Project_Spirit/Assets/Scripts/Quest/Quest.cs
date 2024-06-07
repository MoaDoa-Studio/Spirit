using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class Quest : MonoBehaviour, IPointerEnterHandler
{
    public int QuestID;
    public string QuestName;
    public string QuestBody;
    public string QuestConditionText;
    public int QuestCondition;
    public int QuestClearCondition;
    public int QuestClearTime;
    public int QuestDisadvantage;

    public int ConditionID;
    public int ConditionTarget;
    public float ConditionStandard;

    public int CurrentConditionAchieve;

    private readonly int[] QuestPrefabSize = new int[2] { 50, 100 };
    private readonly int[] QuestNamePos = new int[2] { 0, 25 };
    // For Debug.
    public Quest(int _id, string _name, string _body, string _conditionText,
        int _condition, int _clearCondition, int _clearTime, int _disadvantage)
    {
        QuestID = _id;
        QuestName = _name;
        QuestBody = _body;
        QuestConditionText = _conditionText;
        QuestCondition = _condition;
        QuestClearCondition = _clearCondition;
        QuestClearTime = _clearTime;
        QuestDisadvantage = _disadvantage;
    }

    public void SetCondition(int _cID, int _cTarget, float _cStandard)
    {
        ConditionID = _cID;
        ConditionTarget = _cTarget;
        ConditionStandard = _cStandard;
    }

    public bool CheckClear()
    {
        if (CurrentConditionAchieve >= ConditionStandard)
            return true;
        return false;
    }
    public void Start()
    {
        CurrentConditionAchieve = 0;
    }

    public delegate void OnPointerHandler();
    public OnPointerHandler pHandler;
    public void OnPointerEnter(PointerEventData eventData)
    {
        pHandler?.Invoke();
    }    


}
