using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task
{
    public string name = null;    
    public int StoneRequire = 10;
    public int WoodRequire = 10;
    public int EssenceRequire = 10;
    public int WorkRequire = 10;

    public bool isComplete { get; set; }
    public Task(string _name)
    {
        name = _name;        
        isComplete = false;
    }
}

public class TaskData : MonoBehaviour
{
    public Dictionary<string, Task> Tasks = new Dictionary<string, Task>();

    private void Start()
    {        
        // For Debug.
        // 나중에 CSV 데이터 테이블 읽어오는 방식으로 변경 예정.
        Tasks.Add("B11", new Task("B11"));
        Tasks.Add("B21", new Task("B21"));
        Tasks.Add("B22", new Task("B22"));
        Tasks.Add("B31", new Task("B31"));
        Tasks.Add("B41", new Task("B41"));
        Tasks.Add("B1", new Task("B1"));
        Tasks.Add("B2", new Task("B2"));
        Tasks.Add("B3", new Task("B3"));
        Tasks.Add("B4", new Task("B4"));
    }

    public Task GetTask(string _taskID)
    {
        if (!Tasks.ContainsKey(_taskID))
            return null;
        return Tasks[_taskID];
    }

    public void SetTask(string _taskID)
    {
        Tasks.Add(_taskID, new Task(_taskID));
    }
}
