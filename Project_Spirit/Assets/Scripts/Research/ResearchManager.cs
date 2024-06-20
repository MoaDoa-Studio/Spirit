using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ResearchManager : MonoBehaviour
{
    [Header("오브젝트")]
    [SerializeField]
    private GameObject Research_UI;    
    [SerializeField]
    private GameObject Blurry;
    [SerializeField]
    private GameObject StudyDetail;
    [SerializeField]
    private GameObject StudyComplete;
    [SerializeField]
    private GameObject StepButton;
    [SerializeField]
    private GameObject BuildingTree;

    private GameObject currentClickedObj;
    private Study currentStudy;
    private int currentWork;
    private bool inProgress;

    private void Start()
    {
        inProgress = false;
        currentStudy = null;
    }
    
    public void ShowResearchUI()
    {
        if (inProgress)
            UpdateStudyDetail();
        Research_UI.SetActive(true);
    }

    Study GetTask(int StudyID)
    {
        if (!DatabaseManager.instance.Studies.ContainsKey(StudyID))
            return null;
        return DatabaseManager.instance.Studies[StudyID];
    }    

    void OnClickStudyButton(int StudyID)
    {
        Study _study = GetTask(StudyID);
        if (_study == null || _study.isComplete)
            return;
        
        currentClickedObj = EventSystem.current.currentSelectedGameObject;
        SetStudyDetail(_study);
    }
    void SetStudyDetail(Study _study)
    {        
        TextMeshProUGUI Detail_StudyName = StudyDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Button Detail_StartButton = StudyDetail.transform.GetChild(4).GetComponent<Button>();
        Slider Detail_ProgressSlider = StudyDetail.transform.GetChild(5).GetComponent<Slider>();

        Detail_StudyName.text = _study.StudyName;        
        Detail_StartButton.gameObject.SetActive(true);
        Detail_StartButton.onClick.RemoveAllListeners();
        Detail_StartButton.onClick.AddListener(() => OnClickResearchStartButton(_study));
        Detail_ProgressSlider.gameObject.SetActive(false);


        //if (_study.WoodRequire < (현재 나무) || _study.StoneRequire < (현재 돌))
        // StartBtn.interactable = false;       
        StudyDetail.SetActive(true);
    }

    void UpdateStudyDetail()
    {
        if (currentStudy == null)
            return;

        TextMeshProUGUI Detail_StudyName = StudyDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Button Detail_StartButton = StudyDetail.transform.GetChild(4).GetComponent<Button>();
        Slider Detail_ProgressSlider = StudyDetail.transform.GetChild(5).GetComponent<Slider>();

        Detail_StudyName.text = currentStudy.StudyName;
        Detail_ProgressSlider.gameObject.SetActive(true);
        Detail_ProgressSlider.value = (float)currentWork / currentStudy.WorkRequirement;
        Detail_StartButton.gameObject.SetActive(false);

        StudyDetail.SetActive(true);
    }
    
    public void OnClickResearchStartButton(Study _study)
    {
        inProgress = true;
        currentStudy = _study;
        currentWork = 0;
        currentClickedObj.transform.Find("InProgressMark").gameObject.SetActive(true);        

        UpdateStudyDetail();
    }

    public void CompleteStudy()
    {
        currentStudy.isComplete = true;        

        ShowTaskComplete();
        UpdateStudyLock();

        currentClickedObj.GetComponent<Button>().interactable = false;
        currentClickedObj.transform.Find("InProgressMark").gameObject.SetActive(false);

        currentStudy = null;
        currentClickedObj = null;
        inProgress = false;        
    }

    public void ShowTaskComplete()
    {        
        StudyComplete.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = currentStudy.name;        

        StudyComplete.SetActive(true);
        StudyDetail.SetActive(false);
    }
    
    #region For Debug
    void UpdateStudyLock()
    {
        Dictionary<int, Study> Studies = DatabaseManager.instance.Studies;
    }
    
    // 정령이 연구소에서 일할 때 호출 될 함수 로직.
    public void OnClickWork()
    {
        if (currentStudy == null)
            return;

        currentWork += 2;
        UpdateStudyDetail();
        if (currentWork >= currentStudy.WorkRequirement)
        {
            currentWork = 0;            
            CompleteStudy();            
        }
    }
    #endregion
}