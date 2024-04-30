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
    public GameObject[] RockObject;
    public GameObject[] WoodObject;
    List<KeyValuePair<Vector2Int, int>> removedBuilding;
    enum ResourceType
    {
        None = 0,
        Rock = 1,
        Wood = 2,
        Elemetntal = 3
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
            }
            Tuple<Vector2Int, Vector2Int> TwoRoads = isTwoRoadAttachedResource();
            SetConnectedRoad(TwoRoads);

        }
    }

    public void CalculateTotalamountOfResoucre()
    {
        int total = 0;
        foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
        {
            int value = pair.Value;
            total += value;
        }
        Resource_reserves = total;
        if (Resource_reserves == 0)
            resourceType = ResourceType.None;
    }

    public void DecreaseLeastColony()
    {
        int minValue = resourceBuilding[0].Value;
        Vector2Int minCoord = resourceBuilding[0].Key;
        foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
        {
            int value = pair.Value;
            if (value < minValue)
            {
                minValue = value;
                minCoord = pair.Key;
            }
        }
        if (minValue < 50)
        {
            KeyValuePair<Vector2Int, int> updatedPair = new KeyValuePair<Vector2Int, int>(minCoord, minValue - 1);

            for (int i = 0; i < resourceBuilding.Count; i++)
            {
                if (resourceBuilding[i].Key == minCoord)
                {
                    if (updatedPair.Value <= 0)
                    {
                        Vector3 col = new Vector3(minCoord.x + 0.5f, minCoord.y + 0.5f, 0);
                        Collider[] colliders = Physics.OverlapSphere(col, 0.2f);
                        foreach (Collider collider in colliders)
                        {
                            Destroy(collider.gameObject);
                        }
                        SetTileType(minCoord.x, minCoord.y, 0);
                        resourceBuilding.RemoveAt(i);
                        return;
                    }
                    resourceBuilding.RemoveAt(i);
                    break;
                }
            }
            resourceBuilding.Add(updatedPair);
            RelocateTile(minCoord, updatedPair.Value, TileType());
        }
        else if(minValue == 50)
        {
            SearchTileForRemove();
        }

    }

    void SetTileType(int x, int y, int typeNum)
    {
        TileDataManager.instance.SetTileType(x, y, typeNum);
        TileDataManager.instance.nodes[x,y].SetNodeType(typeNum);
        TileDataManager.instance.nodes[x,y].isWalk = true;
    }
    GameObject[] TileType()
    {
        if (resourceType == ResourceType.Rock)
            return RockObject;
        else
            return WoodObject;
    }
    void RelocateTile(Vector2Int minCoord, int _updateValue, GameObject[] _gameobject)
    {
        Vector3 col = new Vector3(minCoord.x + 0.5f, minCoord.y + 0.5f, 0);
        Collider[] colliders = Physics.OverlapSphere(col, 0.2f);
        foreach (Collider collider in colliders)
        {
            Destroy(collider.gameObject);
        }

        if (_updateValue > 0 && _updateValue < 10)
        {
            GameObject obj = Instantiate(_gameobject[0], col, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
        else if (_updateValue < 20)
        {
            GameObject obj = Instantiate(_gameobject[1], col, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
        else if (_updateValue < 30)
        {
            GameObject obj = Instantiate(_gameobject[2], col, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
        else if (_updateValue < 40)
        {
            GameObject obj = Instantiate(_gameobject[3], col, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
        else
        {
            GameObject obj = Instantiate(_gameobject[4], col, Quaternion.identity);
            obj.transform.SetParent(transform);
        }
    }
    
    void SearchTileForRemove()
    {
        foreach(KeyValuePair<Vector2Int, int> varpos in resourceBuilding)
        {
            int[] dirx = { 0, 0, 1, -1 };
            int[] diry = { 1, -1 ,0 ,0 };

            KeyValuePair<Vector2Int, int> inValuePair = varpos;
            
            for(int i = 0; i < 4; i++)
            {
                if(TileDataManager.instance.GetTileType(inValuePair.Key.x + dirx[i], inValuePair.Key.y + diry[i]) != 3)
                {
                    removedBuilding.Add(inValuePair);
                }

            }

        }

    }


    #region 자원 - 길 연결

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

        public void UpdateFieldStatus(int _type)
        {
            Tuple<Vector2Int, Vector2Int> TwoRoads = isTwoRoadAttachedResource();
            SetConnectedRoad(TwoRoads);
            resourceType = (ResourceType)_type;
        }

        Tuple<Vector2Int, Vector2Int> isTwoRoadAttachedResource()
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
                        foreach (Vector2Int var in result)
                        {
                            Debug.Log(var);
                            Debug.Log("총 갯수는 : " + result.Count);
                        }
                    }

                }
            }

            if (result.Count == 2)
                return new Tuple<Vector2Int, Vector2Int>(result[0], result[1]);
            else
                return null;
        }

        #endregion

    } 




