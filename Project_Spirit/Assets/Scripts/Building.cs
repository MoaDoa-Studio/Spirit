using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{    
    private Vector2Int upperRight;
    private Vector2Int bottomLeft;
    public Tuple<Vector2Int, Vector2Int> connectedRoads;

    private void Start()
    {
        connectedRoads = null;
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
}
