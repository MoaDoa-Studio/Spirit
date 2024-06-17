using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class ResourceBuilding : MonoBehaviour
{   
    public int Resource_reserves;
    public int yOffset = 2;
    public List<KeyValuePair<Vector2Int, int>> resourceBuilding;
    public Tuple<Vector2Int, Vector2Int> connectedRoads;
    
    public GameObject[] RockObject;
    public GameObject[] WoodObject;

    [SerializeField]
    public bool CanUse = false;
    [SerializeField]
    List<GameObject> gameObjectList;
    ResouceManager resourceManager;
    
    int decreasedamount = 0;
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

    private void Start()
    {
        gameObjectList = new List<GameObject>(4);
        InitializeResourceManger(); // => 자원 정보 초기화
       
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
            if (TwoRoads != null) SetConnectedRoad(TwoRoads);
            CalculateTotalamountOfResoucre();
           
        }
        gameObjectList.RemoveAll(item => item == null);
    }

    #region 자원 총량 계산.
    void CalculateTotalamountOfResoucre()
    {
        int total = 0;
        foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
        {
            int value = pair.Value;
            total += value;

        }
        Resource_reserves = total;
        if (Resource_reserves <= 0)
        {
            resourceType = ResourceType.None;
            foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
            {
                ResetTileType(pair.Key.x, pair.Key.y, 0);

            }
            gameObjectList.Clear();
            Destroy(this.gameObject);
        }
    }
    void ResetTileType(int x, int y, int typeNum)
    {
        TileDataManager.instance.SetTileType(x, y, typeNum);
        TileDataManager.instance.nodes[x, y].SetNodeType(typeNum);
        TileDataManager.instance.nodes[x, y].isWalk = false;
    }

    #endregion

    #region 정령 자원 소모 로직.
    public void GetDecreasement(int num)
    {
        DecreaseLeastColony(num);           // 자원 타일 초기화
        CalculateTotalamountOfResoucre();   // 자원 총 수량 계산
        Addamount(num);                     // ResouceManager 값 더하기
        Debug.Log(Resource_reserves);
    }
    void DecreaseLeastColony(int num)
    {   
       // Debug.Log(Resource_reserves);
        decreasedamount += num;
        if (decreasedamount % resourceBuilding.Count == 0)
        {
            int tempResourceamount = Resource_reserves / resourceBuilding.Count;

            foreach (KeyValuePair<Vector2Int, int> pair in resourceBuilding)
            {  
                RelocateTile(pair.Key, tempResourceamount, TileType());
            }
            decreasedamount = 0;
        }
        
        int randomIndex = UnityEngine.Random.Range(0, resourceBuilding.Count);
        // 차감하는 값이 0일때, 타일 계산 재할당.
        if (resourceBuilding[randomIndex].Value <= 0)
        {
            randomIndex = UnityEngine.Random.Range(0, resourceBuilding.Count);
        }
        Vector2Int randPos = resourceBuilding[randomIndex].Key;
        int posses = resourceBuilding[randomIndex].Value;
        KeyValuePair<Vector2Int, int> updatedPair = new KeyValuePair<Vector2Int, int>(randPos, posses - num);
        resourceBuilding.RemoveAt(randomIndex);
        resourceBuilding.Add(updatedPair);
        

    }
    // 타일 재동기화
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

        if (_updateValue < 10)
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

    #endregion

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
                        // Debug.Log(var);
                        //Debug.Log("총 갯수는 : " + result.Count);
                    }
                }

            }
        }

        if (result.Count == 2)
        {
            CanUse =true;
            return new Tuple<Vector2Int, Vector2Int>(result[0], result[1]);
        }
        else
        {
            CanUse=false;
            return null;
        }
    }

    #endregion

    public bool CheckForCapacity()
    {
        if(connectedRoads == null) return false;
        if (gameObjectList.Count >= 0 && gameObjectList.Count < 4)
        {
            return true;
        }
        else
            return false;
    }

    public void AddWorkingSprit(GameObject _gameObject)
    {
        if(!gameObjectList.Contains(_gameObject)) 
        {
         gameObjectList.Add(_gameObject);
        }
    }
    public void DeleteWorkingSprit(GameObject _gameObject)
    {
        gameObjectList.Remove(_gameObject);
    }

    private void InitializeResourceManger()
    {
        // GameManager 오브젝트 찾기
        GameObject gameManager = GameObject.Find("GameManager");

        if (gameManager != null)
        {
            Transform resourceManagerTransform = gameManager.transform.Find("[ResourceManager]");

            if (resourceManagerTransform != null)
            {
                resourceManager = resourceManagerTransform.GetComponent<ResouceManager>();

            }
        }
    }

    public void Addamount(int num)
    {
        if (resourceType == ResourceType.Rock)
        {
            resourceManager.AddRock(num);
        }
        else
            resourceManager.AddTimer(num);
               
    }

}

