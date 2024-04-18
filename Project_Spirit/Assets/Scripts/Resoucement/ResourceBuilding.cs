using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceBuilding : MonoBehaviour
{
    public int Capacity;
    public int Resource_reserves;

    public List<KeyValuePair<Vector2Int, int>> resourceBuilding;
    public Tuple<Vector2Int, Vector2Int> connectedRoads;

    enum ResourceType
    {
        None = 0,
        Rock = 1,
        Wood = 2
    }

    ResourceType resourceType = ResourceType.None;

    public void SetResourceType(int type)
    {
        resourceType = (ResourceType)type;
    }

    public int GetResourceType()
    {
        return (int)resourceType;
    }

    private void Update()
    {
        if (resourceBuilding != null)
        {
            foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
            {
                int pairX = pair.Key.x;
                int pairY = pair.Key.y;

                TileDataManager.instance.nodes[pairX, pairY].resourceBuilding = this;
                TileDataManager.instance.nodes[pairX, pairY].isWalk = true;
            }
            Tuple<Vector2Int, Vector2Int> TwoRoads = isTwoRoadAttachedResource();
            SetConnectedRoad(TwoRoads);
            Debug.Log(GetConnectedRoad());
        }
    }

    void CalculateTotalamountOfResoucre()
    {
        int total = 0;
        foreach(KeyValuePair<Vector2Int, int> pair in resourceBuilding)
        {
            int value = pair.Value;
            total += value;
        }
        Resource_reserves = total;
        if(Resource_reserves == 0)
            resourceType = ResourceType.None;
    }

    public Tuple<Vector2Int, Vector2Int> GetConnectedRoad()
    {
        if (connectedRoads == null)
        {
            Debug.Log("µµ·Î°¡ 2°³°¡ ¾Æ´Õ´Ï´Ù.");
            return null;
        }
        return connectedRoads;
    }

    void SetConnectedRoad(Tuple<Vector2Int, Vector2Int> connectedRoads)
    {
        this.connectedRoads = connectedRoads;
    }

    public void UpdateFieldStatus(int _type)
    {
        Tuple<Vector2Int, Vector2Int> TwoRoads = isTwoRoadAttachedResource();
        SetConnectedRoad(TwoRoads);
        resourceType = (ResourceType)_type;
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
                int newX = pairX + dx[i];
                int newY = pairY + dy[i];

                if (!result.Contains(new Vector2Int(newX, newY)) && TileDataManager.instance.GetTileType(pairX + dx[i], pairY + dy[i]) == 3)
                {
                    result.Add(new Vector2Int(pairX + dx[i], pairY + dy[i]));
                    foreach(Vector2Int var in result)
                    {
                    Debug.Log(var);
                        Debug.Log("ÃÑ °¹¼ö´Â : " + result.Count);
                    }
                }

            }
        }

        if(result.Count == 2)
        return new Tuple<Vector2Int, Vector2Int>(result[0], result[1]);
        else
        return null;
    }
}
