using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceDeployment : MonoBehaviour
{
    public Sprite[] RockSprite;
    public Sprite[] WoodSprite;

    Node[,] nodes;
    int[] clusterRockGroup = new int[4];
    int[] clusterWoodGroup = new int[4];
    int resourceCount = 1200;

    int clusterCount = 4;
    int minResource = 100;
    int maxResource = 600;

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
    private void Start()
    {
        SetDefaultMapResource();
        RandomlySetRockResources();
        RandomlySetWoodResources();

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

    public (int,int) MakeRamdom()
    {
        int RandomX = UnityEngine.Random.Range(0, 103);
        int RandomY = UnityEngine.Random.Range(0, 103);
        
        return (RandomX, RandomY);
    }
    public void PlaceTiles()
    {
        (int, int) result = MakeRamdom();

        if (BinaryCheckOfThisTile(result.Item1, result.Item2))
        {
            for (int i = 0; i < clusterRockGroup.Length; i++)
            {
                int HavingResource = clusterRockGroup[i];
                int checks = HavingResource / 50;
                if (HavingResource % 50 != 0) checks += 1;
                Debug.Log(checks);
                (int, int) resultvalue = MakeRamdom();
                Vector2Int inmethodRandom = GetChangeVector2Int(resultvalue.Item1, resultvalue.Item2);

                if (BinaryCheckOfThisTile(inmethodRandom.x, inmethodRandom.y))
                {

                    List<Vector2Int> movedCoordinates = MoveCoordinatesDFS(inmethodRandom, checks, new HashSet<Vector2Int>());
                    Debug.Log("Found Path : ");
                    // 얻은 좌표들을 출력
                    foreach (Vector2Int coord in movedCoordinates)
                    {
                        {
                            Debug.Log(coord);
                        }


                        //while (HavingResource >= 0)
                        //{

                        //    int Repeatvalues = HavingResource / 50;
                        //    TileDataManager.instance.SetTileType(RandomX, RandomY, 6);
                        //    //if (HavingResource >= 50)
                        //{ HavingResource -= 50;
                        //    nodes[RandomX, RandomY].rock_reserve += 50;
                        //    Debug.Log($"{RandomX} + , + {RandomY}" + nodes[RandomX, RandomY].rock_reserve);

                        //}
                        //else
                        //{
                        //    int previousResouce = HavingResource;
                        //    HavingResource = 0;
                        //    nodes[RandomX, RandomY].rock_reserve += previousResouce; 
                        //    Debug.Log($"{RandomX} + , + {RandomY}" + nodes[RandomX, RandomY].rock_reserve);
                        //}

                        //}
                    }
                }
                else
                    PlaceTiles();
                // 어느타일에 설치할지 수행할 로직
            }
        }
        else
            PlaceTiles();
    }

    List<Vector2Int> MoveCoordinatesDFS(Vector2Int currentPos, int remainingMoves, HashSet<Vector2Int> visited)
    {
        List<Vector2Int> movedCoords = new List<Vector2Int>();

        if (remainingMoves == 1)
        {   
            movedCoords.Add(currentPos);
            return movedCoords;
        }
        visited.Add(currentPos);

        // 랜덤하게 상하좌우중 하나로 이동하며 DFS 수행
        Direction[] directions = (Direction[])System.Enum.GetValues(typeof(Direction));
        ShuffleArray(directions); // 방향을 랜덤하게 섞음
        foreach (Direction dir in directions)
        {
            Vector2Int nextPos = Move(currentPos, dir);
            // 방문한 적이 없는 좌표라면
            if (!visited.Contains(nextPos))
            {
                if (BinaryCheckOfThisTile(nextPos.x, nextPos.y))
                {
                    List<Vector2Int> nextCoords = MoveCoordinatesDFS(nextPos, remainingMoves - 1, visited);
                    if (nextCoords != null)
                    {
                        movedCoords.Add(currentPos);
                        movedCoords.AddRange(nextCoords);
                        // 단 하나의 경우의 수만 보고 싶으므로, 첫 번째 경우의 수를 찾았으면 더 이상의 탐색은 중단
                        break;
                    }
                }
            }
        }

            // DFS 종료되면 현재 위치를 다시 방문 가능 상태로 변경
            visited.Remove(currentPos);
        return movedCoords;
    }

    Vector2Int Move(Vector2Int _currentPos, Direction dir)
    {
        switch (dir)
        {
            case Direction.Up:
                return new Vector2Int(_currentPos.x, _currentPos.y + 1);
            case Direction.Down:
                return new Vector2Int(_currentPos.x, _currentPos.y - 1);
            case Direction.Left:
                return new Vector2Int(_currentPos.x - 1, _currentPos.y);
            case Direction.Right:
                return new Vector2Int(_currentPos.x + 1, _currentPos.y);
            default:
                return _currentPos;
        }
    }

    void ShuffleArray<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
    Vector2Int GetChangeVector2Int(int a, int b)
    {
        return new Vector2Int(a, b);
    }

    // 100칸 이내 요람, 원소 생산건물 . 장애물이 발견되지 않으면 true 반환
    private bool BinaryCheckOfThisTile(int x, int y)
        {
            bool foundObstacle = false;
             if (x < 0 || y < 0)
             {
                 return false;
             }
            int startX = x - 10;
            if (startX <= 0) startX = 0;
            int startY = y - 10;
            if (startY <= 0) startY = 0;
            int endX = x + 10;
            if (endX >= 103) endX = 102;
            int endY = y + 10;
            if (endY >= 103) endY = 102;


            while (startX <= endX)
            {
                if (TileDataManager.instance.GetTileType(startX, y) == 1 || TileDataManager.instance.GetTileType(startX, y) == 2)
                {
                    foundObstacle = true;
                    break;
                }
                else
                {
                    startX += 1;
                    if (startX >= 102 || startX < 0)
                    {
                        foundObstacle = true;
                        break;
                    }
                }
            }

            if (!foundObstacle)
            {
                while (startY <= endY)
                {
                    if (TileDataManager.instance.GetTileType(x, startY) == 1 || TileDataManager.instance.GetTileType(x, startY) == 2)
                    {
                        foundObstacle = true;
                        break;
                    }
                    else
                    {
                        startY += 1;
                        if (startY >= 102 || startY < 0)
                        {
                            foundObstacle = true;
                            break;
                        }
                    }
                }

            }
            return !foundObstacle;
        }

        #region 자원배치
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
        #endregion
    }

