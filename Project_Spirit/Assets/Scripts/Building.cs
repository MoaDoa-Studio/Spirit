using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{    
    private Vector2Int upperRight;
    private Vector2Int bottomLeft;
    private Tuple<Vector2Int, Vector2Int> connectedRoad;

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
        if (connectedRoad == null)
        {
            Debug.Log("도로가 2개가 아닙니다.");
            return null;
        }
        return connectedRoad;
    }

    public void SetConnectedRoad(Vector2Int a, Vector2Int b)
    {
        connectedRoad = new Tuple<Vector2Int, Vector2Int>(a, b);
    }
}
