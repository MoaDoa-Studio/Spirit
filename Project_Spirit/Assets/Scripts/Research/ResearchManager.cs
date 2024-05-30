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
    private GameObject TaskDetail;
    [SerializeField]
    private GameObject TaskComplete;
    [SerializeField]
    private GameObject StepButton;
    [SerializeField]
    private GameObject BuildingTree;    

    [Header("스크립트")]
    [SerializeField]
    private TaskData taskData;
    
    private GameObject currentClickObj;
    private Task currentTask;
    private int currentWork;
    private bool inProgress;

    private void Start()
    {
        inProgress = false;
        currentClickObj = null;
        currentTask = null;        
    }
    
    public void ShowResearchUI()
    {
        if (inProgress)
        {
            UpdateTaskDetailInfo();
        }

        Research_UI.SetActive(true);
    }

    public void OnClickEachTask(string TaskID)
    {
        if (inProgress) return;

        Task _task = taskData.GetTask(TaskID);
        if (_task == null || _task.isComplete) return;

        currentClickObj = EventSystem.current.currentSelectedGameObject;
        SetTaskDetailInfo(_task);
    }

    void SetTaskDetailInfo(Task _task)
    {
        TextMeshProUGUI TaskDetailName = TaskDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Button StartBtn = TaskDetail.transform.GetChild(4).GetComponent<Button>();
        Slider progressSlider = TaskDetail.transform.GetChild(5).GetComponent<Slider>();

        TaskDetailName.text = _task.name;                
        StartBtn.gameObject.SetActive(true);
        StartBtn.onClick.RemoveAllListeners();
        StartBtn.onClick.AddListener(() => OnClickResearchStartButton(_task));
        progressSlider.gameObject.SetActive(false);

        //if (_task.WoodRequire < (현재 나무) || _task.StoneRequire < (현재 돌))
        // StartBtn.interactable = false;

        TaskDetail.SetActive(true);
    }

    void UpdateTaskDetailInfo()
    {
        TextMeshProUGUI TaskDetailName = TaskDetail.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
        Button StartBtn = TaskDetail.transform.GetChild(4).GetComponent<Button>();
        Slider progressSlider = TaskDetail.transform.GetChild(5).GetComponent<Slider>();
        
        TaskDetailName.text = currentTask.name;
        progressSlider.gameObject.SetActive(true);
        progressSlider.value = (float)currentWork / currentTask.WorkRequire;
        StartBtn.gameObject.SetActive(false);

        TaskDetail.SetActive(true);
    }
    
    public void OnClickResearchStartButton(Task _task)
    {
        inProgress = true;
        currentTask = _task;
        currentWork = 0;
        currentClickObj.transform.Find("InProgressMark").gameObject.SetActive(true);        

        UpdateTaskDetailInfo();
    }

    public void CompleteTask()
    {
        currentTask.isComplete = true;        

        ShowTaskComplete();
        UpdateTaskLock();

        currentClickObj.GetComponent<Button>().interactable = false;
        currentClickObj.transform.Find("InProgressMark").gameObject.SetActive(false);

        currentTask = null;
        currentClickObj = null;
        inProgress = false;        
    }

    public void ShowTaskComplete()
    {        
        TaskComplete.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = currentTask.name;        

        TaskComplete.SetActive(true);
        TaskDetail.SetActive(false);
    }
    
    #region For Debug
    void UpdateTaskLock()
    {
        if (taskData.Tasks["B1"].isComplete)        
            Blurry.transform.GetChild(0).gameObject.SetActive(false);        
        if (taskData.Tasks["B2"].isComplete)        
            Blurry.transform.GetChild(1).gameObject.SetActive(false);        
        if (taskData.Tasks["B3"].isComplete)        
            Blurry.transform.GetChild(2).gameObject.SetActive(false);        
        if (taskData.Tasks["B4"].isComplete)        
            Blurry.transform.GetChild(3).gameObject.SetActive(false);        

        if (taskData.Tasks["B11"].isComplete &&
            !taskData.Tasks["B2"].isComplete)
            StepButton.transform.GetChild(1).GetComponent<Button>().interactable = true;
        if (taskData.Tasks["B21"].isComplete && taskData.Tasks["B22"].isComplete &&
            !taskData.Tasks["B3"].isComplete)        
            StepButton.transform.GetChild(2).GetComponent<Button>().interactable = true;
        if (taskData.Tasks["B31"].isComplete &&
            !taskData.Tasks["B4"].isComplete)
            StepButton.transform.GetChild(3).GetComponent<Button>().interactable = true;
    }
    
    // 정령이 연구소에서 일할 때 호출 될 함수 로직.
    public void OnClickWork()
    {
        if (currentTask == null)
            return;

        currentWork += 2;
        UpdateTaskDetailInfo();
        if (currentWork >= currentTask.WorkRequire)
        {
            currentWork = 0;            
            CompleteTask();            
        }
    }
    #endregion
}