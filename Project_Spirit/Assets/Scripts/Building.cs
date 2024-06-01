using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [HideInInspector]
    public Vector2Int upperRight;
    [HideInInspector]
    public Vector2Int bottomLeft;
    public Tuple<Vector2Int, Vector2Int> connectedRoads;
    List<GameObject> gameObjectList;

    // 공장 운영 상태를 나타냄
    public enum BuildOperator
    {
        None,
        Construct,
        Done
    }

    public enum BuildState
    {
        Rest,
        isWork
    }
    [SerializeField]
    BuildOperator buildOperator = BuildOperator.None;
    [SerializeField]
    BuildState buildState = BuildState.Rest;
    
    public int MaxPlayer = 4;
    private void Start()
    {
        connectedRoads = null;
        gameObjectList = new List<GameObject>();
    }
    private void Update()
    {
        gameObjectList.RemoveAll(item => item == null);
        BuildOperation();
        BuildStater();
    }

    void BuildOperation()
    {
        switch(buildOperator)
        {
            case BuildOperator.None:

            break;

            case BuildOperator.Construct:

            break;

            case BuildOperator.Done:
            
            break;
        }
    }

    void BuildStater()
    {
        switch(buildState)
        {
            case BuildState.Rest:
                break;
            case BuildState.isWork:
                break;
        }
    }

    public Tuple<Vector2Int, Vector2Int> GetBuildingPos()
    {
        return new Tuple<Vector2Int, Vector2Int>(upperRight, bottomLeft);
    }

    public void SetBuildingPos(Vector2Int upperRight, Vector2Int bottomLeft)
    {
        this.upperRight = upperRight;
        this.bottomLeft = bottomLeft;
    }

    public Tuple<Vector2Int, Vector2Int> GetConnectedRoad()
    {
        if (connectedRoads == null)
        {
            Debug.Log("도로가 2개가 아닙니다.");
            return null;
        }
        return connectedRoads;
    }

    public void SetConnectedRoad(Tuple<Vector2Int, Vector2Int> connectedRoads)
    {
        this.connectedRoads = connectedRoads;
    }

    public bool CheckForCapacity()
    {   if (connectedRoads == null) return false;
        if (gameObjectList.Count >= 0 && gameObjectList.Count < 4)
        {
            return true;
        }
        else
            return false;
    }

    public void AddWorkingSprit(GameObject _gameObject)
    {
        gameObjectList.Add(_gameObject);
    }
    public void DeleteWorkingSprit(GameObject _gameObject)
    {
        gameObjectList.Remove(_gameObject);
    }
}
