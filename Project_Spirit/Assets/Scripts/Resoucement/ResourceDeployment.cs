using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceDeployment : MonoBehaviour
{

    [SerializeField]
    Tilemap tilemap;
    [SerializeField]
    Transform Wood_Parent;
    [SerializeField]
    Transform Rock_Parent;
    [SerializeField]
    GameObject ResourceBuildingPrefab;

    Node[,] nodes;
    List<GameObject> WoodObjects;
    List<GameObject> RockObjects;

    [HideInInspector]
    public GameObject[] RockSprite;
    [HideInInspector]
    public GameObject[] WoodSprite;
    int[] clusterRockGroup = new int[4];
    int[] clusterWoodGroup = new int[4];
    
    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    private void Start()
    {
        WoodObjects = GetComponent<ResouceManager>().WoodObjects;
        RockObjects = GetComponent<ResouceManager>().RockObjects;
        
        SetDefaultMapResource();
        RandomlySetRockResources();
        RandomlySetWoodResources();
        PlaceRockTiles();
        PlaceWoodTiles();
        GetComponent<ResouceManager>().resourceDeployed = true;
    }

    private void SetDefaultMapResource()
    {
        for (int i = 49; i <= 53; i++)
        { for (int j = 49; j <= 53; j++)
            { TileDataManager.instance.SetTileType(i, j, 2);
            }
        }

        for (int i = 0; i <= 2; i++)
        { for (int j = 50; j <= 52; j++)
                TileDataManager.instance.SetTileType(i, j, 1);  // 임시적 building
        }

        for (int i = 100; i <= 102; i++)
        { for (int j = 50; j <= 52; j++)
                TileDataManager.instance.SetTileType(i, j, 1);
        }
        for (int i = 50; i <= 52; i++)
        { for (int j = 100; j <= 102; j++)
                TileDataManager.instance.SetTileType(i, j, 1);
        }
        for (int i = 50; i <= 52; i++) {
            for (int j = 0; j <= 2; j++)
                TileDataManager.instance.SetTileType(i, j, 1);
        }
        
    }
     
    public (int, int) MakeRamdom()
    {
        int RandomX = UnityEngine.Random.Range(10, 93);
        int RandomY = UnityEngine.Random.Range(10, 93);

        return (RandomX, RandomY);
    }
    public void PlaceRockTiles()
    {
        nodes = TileDataManager.instance.GetNodes();
        (int, int) result = MakeRamdom();

        for (int i = 0; i < clusterRockGroup.Length; i++)
        {
            // 각 반복마다 새로운 리스트를 생성
            List<KeyValuePair<Vector2Int, int>> rpariedCoordinates = new List<KeyValuePair<Vector2Int, int>>();

            int HavingResource = clusterRockGroup[i];
            int checks = HavingResource / 50;
            if (HavingResource % 50 != 0) checks += 1;
           
            (int, int) resultvalue = MakeRamdom();
            Vector2Int inmethodRandom = GetChangeVector2Int(resultvalue.Item1, resultvalue.Item2);

            List<Vector2Int> movedCoordinates = GetBFSPositions(inmethodRandom, checks);
           
            // 얻은 좌표들을 출력
            foreach (Vector2Int coord in movedCoordinates)
            {
                int calculating = HavingResource;
                //Debug.Log(coord);
                int x = coord.x;
                int y = coord.y;
                if (HavingResource >= 50)
                {
                    nodes[x, y].rock_reserve += 50;
                    if (nodes[x,y].rock_reserve > 0) 
                    {
                        TileDataManager.instance.SetTileType(x, y, 6);
                        nodes[x, y].SetNodeType(6);
                       
                        rpariedCoordinates.Add(new KeyValuePair<Vector2Int, int>(coord, 50));
                    }
                    HavingResource -= 50;

                }
                else
                {
                    int previousResource = HavingResource;
                    HavingResource = 0;
                    nodes[x, y].rock_reserve += previousResource;
                    if (nodes[x, y].rock_reserve > 0)
                    {
                        TileDataManager.instance.SetTileType(x, y, 6);
                        nodes[x, y].SetNodeType(6);
                        
                        rpariedCoordinates.Add(new KeyValuePair<Vector2Int, int>(coord, previousResource));
                    }
                             
                }
           
            }
            GameObject prefabInstance = Instantiate(ResourceBuildingPrefab);
            prefabInstance.transform.SetParent(Rock_Parent);
            prefabInstance.GetComponent<ResourceBuilding>().resourceBuilding = rpariedCoordinates;
            prefabInstance.GetComponent<ResourceBuilding>().UpdateFieldStatus(1);
            

            RockObjects.Add(prefabInstance);
            AllocateRock_Placement(prefabInstance);
        }
       
    }
    
    public void PlaceWoodTiles()
    {
        nodes = TileDataManager.instance.GetNodes();
        (int, int) result = MakeRamdom();

        for (int i = 0; i < clusterWoodGroup.Length; i++)
        {
            // 각 반복마다 새로운 리스트를 생성
            List<KeyValuePair<Vector2Int, int>> wpariedCoordinates = new List<KeyValuePair<Vector2Int, int>>();

            int HavingResource = clusterWoodGroup[i];
            int checks = HavingResource / 50;
            if (HavingResource % 50 != 0) checks += 1;
            
            (int, int) resultvalue = MakeRamdom();
            Vector2Int inmethodRandom = GetChangeVector2Int(resultvalue.Item1, resultvalue.Item2);

            List<Vector2Int> movedCoordinates = GetBFSPositions(inmethodRandom, checks);
            
            // 얻은 좌표들을 출력
            foreach (Vector2Int coord in movedCoordinates)
            {   
                int calculating = HavingResource;
                
                int x = coord.x;
                int y = coord.y;
                if (HavingResource >= 50)
                {
                    nodes[x, y].wood_reserve += 50;
                    if (nodes[x, y].wood_reserve > 0)
                    {
                        TileDataManager.instance.SetTileType(x, y, 7);
                        nodes[x,y].SetNodeType(7);
                       
                        wpariedCoordinates.Add(new KeyValuePair<Vector2Int, int>(coord, 50));
                    }
                    HavingResource -= 50;

                }
                else
                {
                    int previousResource = HavingResource;
                    HavingResource = 0;
                    nodes[x, y].wood_reserve += previousResource;
                    if (nodes[x, y].wood_reserve > 0)
                    {
                        TileDataManager.instance.SetTileType(x, y, 7);
                        nodes[x, y].SetNodeType(7);   
                        
                        wpariedCoordinates.Add(new KeyValuePair<Vector2Int, int>(coord, previousResource));
                    }

                }
                
            }
            GameObject prefabInstance = Instantiate(ResourceBuildingPrefab);
            prefabInstance.transform.SetParent(Wood_Parent);
            prefabInstance.GetComponent<ResourceBuilding>().resourceBuilding = wpariedCoordinates;
            prefabInstance.GetComponent<ResourceBuilding>().UpdateFieldStatus(2);

            WoodObjects.Add(prefabInstance);
            AllocateWood_Placement(prefabInstance);
        }

    }

    #region 랜덤 생성
    // 나무자원 배분
    private void RandomlySetWoodResources()
    {
        int total = 1200;
        int minPerGroup = 100;
        int maxPerGroup = 400;

        // 랜덤하게 그룹에 숫자 할당
        int remaingTotal = total;
        int[] samplegroups = new int[4];

        for (int i = 0; i < samplegroups.Length - 1; i++)
        {
            samplegroups[i] += UnityEngine.Random.Range(minPerGroup, maxPerGroup + 1);
            remaingTotal -= samplegroups[i];

        }
        samplegroups[samplegroups.Length - 1] = remaingTotal;
        if (samplegroups[samplegroups.Length - 1] >= 600)
            RandomlySetWoodResources();
        else
        {
            // 결과 출력 
            for (int i = 0; i < samplegroups.Length; i++)
            {
                //Debug.Log("나무Group의 갯수는 : " + samplegroups[i]);
                clusterWoodGroup = samplegroups;
            }

        }
    }

    // 돌 자원
    private void RandomlySetRockResources()
    {
        int total = 1200;
        int minPerGroup = 100;
        int maxPerGroup = 400;

        // 랜덤하게 그룹에 숫자 할당
        int remaingTotal = total;
        int[] samplegroups = new int[4];

        for (int i = 0; i < samplegroups.Length - 1; i++)
        {
            samplegroups[i] += UnityEngine.Random.Range(minPerGroup, maxPerGroup + 1);
            remaingTotal -= samplegroups[i];

        }
        samplegroups[samplegroups.Length - 1] = remaingTotal;
        if (samplegroups[samplegroups.Length - 1] >= 600)
            RandomlySetRockResources();
        else
        {
            // 결과 출력 
            for (int i = 0; i < samplegroups.Length; i++)
            {
                //Debug.Log("돌Group의 갯수는 : " + samplegroups[i]);
                clusterRockGroup = samplegroups;
            }

        }
    }
    private void AllocateWood_Placement(GameObject Woodparent)
    {
        for (int i = 0; i < 103; i++)
        {
            for (int j = 0; j < 103; j++)
            {
                if (nodes[i, j].wood_reserve > 0)
                {

                    Vector3Int tilePosition = new Vector3Int(i, j, 0);
                    Vector3 vector3 = new Vector3 { x = tilePosition.x + 0.5f, y = tilePosition.y + 0.5f, z = tilePosition.z };

                    Collider[] colliders = Physics.OverlapSphere(vector3, 0.2f);
                    if (colliders != null && colliders.Length > 0)
                    {
                        continue;
                    }

                    int rock_Posses = nodes[i, j].wood_reserve;
                    if (rock_Posses > 0 && rock_Posses < 10)
                    {
                        GameObject obj = Instantiate(WoodSprite[0], vector3, Quaternion.identity);
                        obj.transform.SetParent(Woodparent.transform);
                    }
                    else if (rock_Posses < 20)
                    {
                        GameObject obj = Instantiate(WoodSprite[1], vector3, Quaternion.identity);
                        obj.transform.SetParent(Woodparent.transform);
                    }
                    else if (rock_Posses < 30)
                    {
                        GameObject obj = Instantiate(WoodSprite[2], vector3, Quaternion.identity);
                        obj.transform.SetParent(Woodparent.transform);
                    }
                    else if (rock_Posses < 40)
                    {
                        GameObject obj = Instantiate(WoodSprite[3], vector3, Quaternion.identity);
                        obj.transform.SetParent(Woodparent.transform);
                    }
                    else
                    {
                        GameObject obj = Instantiate(WoodSprite[4], vector3, Quaternion.identity);
                        obj.transform.SetParent(Woodparent.transform);
                    }
                }
            }
        }
    }

    private void AllocateRock_Placement(GameObject Rockparent)
    {
        for (int i = 0; i < 103; i++)
        {
            for (int j = 0; j < 103; j++)
            {
                if (nodes[i, j].rock_reserve > 0)
                {
                    Vector3Int tilePosition = new Vector3Int(i, j, 0);
                    Vector3 vector3 = new Vector3 { x = tilePosition.x + 0.5f, y = tilePosition.y + 0.5f, z = tilePosition.z };

                    Collider[] colliders = Physics.OverlapSphere(vector3, 0.2f);
                    if (colliders != null && colliders.Length > 0)
                    {
                        continue;
                    }

                    int rock_Posses = nodes[i, j].rock_reserve;
                    if (rock_Posses > 0 && rock_Posses < 10)
                    {
                        GameObject obj = Instantiate(RockSprite[0], vector3, Quaternion.identity);
                        obj.transform.SetParent(Rockparent.transform);
                    }
                    else if (rock_Posses < 20)
                    {
                        GameObject obj = Instantiate(RockSprite[1], vector3, Quaternion.identity);
                        obj.transform.SetParent(Rockparent.transform);

                    }
                    else if (rock_Posses < 30)
                    {
                        GameObject obj = Instantiate(RockSprite[2], vector3, Quaternion.identity);
                        obj.transform.SetParent(Rockparent.transform);

                    }
                    else if (rock_Posses < 40)
                    {
                        GameObject obj = Instantiate(RockSprite[3], vector3, Quaternion.identity);
                        obj.transform.SetParent(Rockparent.transform);
                    }
                    else
                    {
                        GameObject obj = Instantiate(RockSprite[4], vector3, Quaternion.identity);
                        obj.transform.SetParent(Rockparent.transform);
                    }
                    
                }
            }
        }
    }

    #endregion
    
    #region 최적 자원배치.

    // BFS 알고리즘을 사용하여 이동할 위치를 저장할 리스트
    List<Vector2Int> visitedPositions = new List<Vector2Int>();

    // BFS 알고리즘을 사용하여 이동한 위치를 반환하는 함수
    List<Vector2Int> GetBFSPositions(Vector2Int startPosition, int moveCount)
    {
        visitedPositions = new List<Vector2Int>();
        MoveCoordinateToBFS(startPosition, moveCount);
        return visitedPositions;
    }

    // BFS 알고리즘 구현
    public void MoveCoordinateToBFS(Vector2Int start, int moveCount)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        visitedPositions.Add(start);
        
        while (queue.Count > 0 && visitedPositions.Count < moveCount)
        {
            Vector2Int current = queue.Dequeue();
            bool isValidPath = false;

            foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
            {
                Vector2Int next = GetNextPosition(current, dir);
                if (BinaryCheckOfThisTile(next.x, next.y) && IsAdjacentToVisitedPosition(next))
                {
                    if (!visitedPositions.Contains(next))
                    {
                        visitedPositions.Add(next);
                        queue.Enqueue(next);
                        isValidPath = true;
                    }

                }
            }

            // 현재 위치에서 유효한 경로가 없으면 방문한 위치를 모두 비우고 다시 시작
            if (!isValidPath)
            {
                visitedPositions.Clear();
                visitedPositions.Add(start);
                queue.Clear();
                queue.Enqueue(start);
                int restartX = UnityEngine.Random.Range(10, 93);
                int restartY = UnityEngine.Random.Range(10, 93);

                start = new Vector2Int(restartX, restartY);
            }
        }
    }

    // 다음 위치 계산.
    Vector2Int GetNextPosition(Vector2Int current, Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return current + Vector2Int.up;
            case Direction.Down:
                return current + Vector2Int.down;
            case Direction.Left:
                return current + Vector2Int.left;
            case Direction.Right:
                return current + Vector2Int.right;
            default:
                return current;
        }
    }


    // 새로운 위치가 이미 방문한 위치에 인접한지 확인하는 함수
    private bool IsAdjacentToVisitedPosition(Vector2Int position)
    {
        foreach (Vector2Int visitedPos in visitedPositions)
        {
            if (Mathf.Abs(position.x - visitedPos.x) <= 1 && Mathf.Abs(position.y - visitedPos.y) <= 1)
            {
                return true;
            }
        }
        return false;
    }
    private bool BinaryCheckOfThisTile(int x, int y)
    {
        bool foundObstacle = false;
        if (x < 0 || y < 0 || x >= 102 || y >= 102) // 맵 범위를 벗어난 경우
        {
            return false;
        }

        for (int i = x - 10; i <= x + 10; i++)
        {
            for (int j = y - 10; j <= y + 10; j++)
            {
                if (i < 0 || j < 0 || i >= 102 || j >= 102) // 맵 범위를 벗어난 경우
                {
                    continue;
                }

                if (TileDataManager.instance.GetTileType(i, j) == 1 || TileDataManager.instance.GetTileType(i, j) == 2)
                {
                    foundObstacle = true;
                    break;
                }
            }
            if (foundObstacle)
            {
                break;
            }
        }

        for (int i = x - 4; i <= x + 4; i++)
        {
            for (int j = y - 4; j <= y + 4; j++)
            {
                if (TileDataManager.instance.GetTileType(i, j) == 6 || TileDataManager.instance.GetTileType(i, j) == 7)
                {
                    foundObstacle = true;
                    break;
                }
            }
            if (foundObstacle)
                break;
        }

        
        return !foundObstacle;
    }
    Vector2Int GetChangeVector2Int(int a, int b)
    {
        return new Vector2Int(a, b);
    }

    #endregion

}



