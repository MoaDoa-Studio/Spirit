using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceBuilding : MonoBehaviour
{
    public List<KeyValuePair<Vector2Int, int>> resourceBuilding;
    public Tuple<Vector2Int, Vector2Int> connectedRoads;

    private void Start()
    {
       foreach(KeyValuePair<Vector2Int, int> pair in resourceBuilding)
       {
            int pairX = pair.Key.x;
            int pairY = pair.Key.y;

            TileDataManager.instance.nodes[pairX, pairY].resourceBuilding = this;
       }
        
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

    void SetConnectedRoad(Tuple<Vector2Int, Vector2Int> connectedRoads)
    {
        this.connectedRoads = connectedRoads;
    }

    public void UpdateFieldStatus()
    {
        Tuple<Vector2Int, Vector2Int> TwoRoads = isTwoRoadAttachedResource();
        SetConnectedRoad(TwoRoads);
    }

    Tuple<Vector2Int,Vector2Int> isTwoRoadAttachedResource()
    {
        List<Vector2Int> result = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
        {
            Node[,] nodes = TileDataManager.instance.GetNodes();
            int pairX = pair.Key.x;
            int pairY = pair.Key.y;

            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { 1, -1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                if (nodes[pairX + dx[i], pairY + dy[i]].GetNodeType() == 3 || nodes[pairX + dx[i], pairY + dy[i]].GetNodeType() == 3)
                {
                    result.Add(new Vector2Int(pairX + dx[i], pairY + dy[i]));
                }

            }
        }

        if(result.Count == 2)
        return new Tuple<Vector2Int, Vector2Int>(result[0], result[1]);

        return null;
    }
}
