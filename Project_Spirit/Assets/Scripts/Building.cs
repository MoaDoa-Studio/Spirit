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
    public int MaxPlayer = 4;
    private void Start()
    {
        connectedRoads = null;
        gameObjectList = new List<GameObject>();
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
    {
        return gameObjectList.Count < 4;
    }

    public void AddWorkingSprit(GameObject _gameObject)
    {
        gameObjectList.Add(_gameObject);
        Debug.Log(gameObjectList.Count + " 갯수");
    }
    public void DeleteWorkingSprit(GameObject _gameObject)
    {
        gameObjectList.Remove(_gameObject);
    }
}
