using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class ResouceManager : MonoBehaviour
{
    public List<List<KeyValuePair<Vector2Int, int>>> WoodAllpaired = new List<List<KeyValuePair<Vector2Int, int>>>();
    public List<List<KeyValuePair<Vector2Int, int>>> RockAllpaired = new List<List<KeyValuePair<Vector2Int, int>>>();
    public List<KeyValuePair<Vector2Int, int>> wpariedCoordinates = new List<KeyValuePair<Vector2Int, int>>();
    public List<KeyValuePair<Vector2Int, int>> rpariedCoordinates = new List<KeyValuePair<Vector2Int, int>>();
    [HideInInspector]
    GameObject[] RockSprite;
    [HideInInspector]
    GameObject[] WoodSprite;

    [HideInInspector]
    public List<GameObject> WoodObjects;
    [HideInInspector]
    public List<GameObject> RockObjects;
    [HideInInspector]
    public bool resourceDeployed = false;
    [HideInInspector]
    public GameObject resourceShowbox;

    public float Element_reserves { get; set; }
    public float Timber_reserves { get; set; }
    public float Rock_reserves { get; set; }
    
    Node[,] nodes;
    float IncreasingTime = 5f;
    float naturalTime = 5f;

    // 자원 표시 UI
    
    public GameObject TimberTxt_UI;
    
    public GameObject RockTxt_UI;
    [HideInInspector]
    public GameObject ElementTxt_UI;
    private void Update()
    {
        ShowTotalResource();
        if (resourceDeployed)
        {
            naturalTime -= Time.deltaTime;
            if (naturalTime <= 0)
            {
                IncresementRock();
                IncresementWood();
                naturalTime = IncreasingTime;
               
            }
        }
    }
    private void Start()
    {
        RockSprite = GetComponent<ResourceDeployment>().RockSprite;
        WoodSprite = GetComponent<ResourceDeployment>().WoodSprite;
    }

    #region 돌 / 나무 자원 증률 타일 동기화 및 변환 
    void IncresementRock()
    {
        int randomlyselectednum = UnityEngine.Random.Range(0, 4);

        List<KeyValuePair<Vector2Int, int>> selectedPairedList = RockObjects[randomlyselectednum].GetComponent<ResourceBuilding>().resourceBuilding;
        Vector2Int minCoord = selectedPairedList[0].Key;
        int minvalue = selectedPairedList[0].Value;
        foreach (KeyValuePair<Vector2Int, int> pair in selectedPairedList)
        {
            int value = pair.Value;
            if (value < minvalue)
            {
                minvalue = value;
                minCoord = pair.Key;
            }

        }

        if (minvalue < 50)
        {
            // 수정된 값 할당
            KeyValuePair<Vector2Int, int> updatedPair = new KeyValuePair<Vector2Int, int>(minCoord, minvalue + 1);
            for (int i = 0; i < selectedPairedList.Count; i++)
            {
                if (selectedPairedList[i].Key == minCoord)
                {
                    selectedPairedList.RemoveAt(i);
                    break;
                }
            }
            selectedPairedList.Add(updatedPair);
            RelocateTileasRock(minCoord, updatedPair.Value, RockObjects[randomlyselectednum]);
           
           
        }
        else if (minvalue == 50)
        {
           // Debug.Log("Over 50 resource has occured!");
            Vector2Int ValidLocation = FindNewTileisPossible(selectedPairedList);
            TileDataManager.instance.SetTileType(ValidLocation.x, ValidLocation.y, 6);
            nodes = TileDataManager.instance.GetNodes();
            nodes[ValidLocation.x, ValidLocation.y].resourceBuilding = RockObjects[randomlyselectednum].GetComponent<ResourceBuilding>();
            nodes[ValidLocation.x, ValidLocation.y].resourceBuilding.UpdateFieldStatus(1);
            nodes[ValidLocation.x, ValidLocation.y].SetNodeType(6);
            

            KeyValuePair<Vector2Int, int> newPair = new KeyValuePair<Vector2Int, int>(ValidLocation, 1);
            selectedPairedList.Add(newPair);
        }
    }

    void RelocateTileasRock(Vector2Int minCoord, int _updateValue, GameObject _setparents)
    {
        Vector3 col = new Vector3(minCoord.x + 0.5f, minCoord.y + 0.5f, 0);
        Collider[] colliders = Physics.OverlapSphere(col, 0.2f);
        foreach (Collider collider in colliders)
        {
            Destroy(collider.gameObject);
        }

        if (_updateValue > 0 && _updateValue < 10)
        {
            GameObject obj = Instantiate(RockSprite[0], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else if (_updateValue < 20)
        {
            GameObject obj = Instantiate(RockSprite[1], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else if (_updateValue < 30)
        {
            GameObject obj = Instantiate(RockSprite[2], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else if (_updateValue < 40)
        {
            GameObject obj = Instantiate(RockSprite[3], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else
        {
            GameObject obj = Instantiate(RockSprite[4], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
    }


    void IncresementWood()
    {
        int randomlyselectednum = UnityEngine.Random.Range(0, 4);

        List<KeyValuePair<Vector2Int, int>> selectedPairedList = WoodObjects[randomlyselectednum].GetComponent<ResourceBuilding>().resourceBuilding;
        Vector2Int minCoord = selectedPairedList[0].Key;
        int minvalue = selectedPairedList[0].Value;
        foreach (KeyValuePair<Vector2Int, int> pair in selectedPairedList)
        {
            int value = pair.Value;
            if (value < minvalue)
            {
                minvalue = value;
                minCoord = pair.Key;
            }

        }

        if (minvalue < 50)
        {
            // 수정된 값 할당
            KeyValuePair<Vector2Int, int> updatedPair = new KeyValuePair<Vector2Int, int>(minCoord, minvalue + 1);
            for (int i = 0; i < selectedPairedList.Count; i++)
            {
                if (selectedPairedList[i].Key == minCoord)
                {
                    selectedPairedList.RemoveAt(i);
                    break;
                }
            }
            selectedPairedList.Add(updatedPair);
            RelocateTileasWood(minCoord, updatedPair.Value, WoodObjects[randomlyselectednum]);


        }
        else if (minvalue == 50)
        {
            // Debug.Log("Over 50 resource has occured!");
            Vector2Int ValidLocation = FindNewTileisPossible(selectedPairedList);
            TileDataManager.instance.SetTileType(ValidLocation.x, ValidLocation.y, 7);
            nodes = TileDataManager.instance.GetNodes();
            nodes[ValidLocation.x, ValidLocation.y].resourceBuilding = WoodObjects[randomlyselectednum].GetComponent<ResourceBuilding>();
            nodes[ValidLocation.x, ValidLocation.y].resourceBuilding.UpdateFieldStatus(2);
            nodes[ValidLocation.x, ValidLocation.y].SetNodeType(7);


            KeyValuePair<Vector2Int, int> newPair = new KeyValuePair<Vector2Int, int>(ValidLocation, 1);
            selectedPairedList.Add(newPair);
        }
    }

    void RelocateTileasWood(Vector2Int minCoord, int _updateValue, GameObject _setparents)
    {
        Vector3 col = new Vector3(minCoord.x + 0.5f, minCoord.y + 0.5f, 0);
        Collider[] colliders = Physics.OverlapSphere(col, 0.2f);
        foreach (Collider collider in colliders)
        {
            Destroy(collider.gameObject);
        }

        if (_updateValue > 0 && _updateValue < 10)
        {
            GameObject obj = Instantiate(WoodSprite[0], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else if (_updateValue < 20)
        {
            GameObject obj = Instantiate(WoodSprite[1], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else if (_updateValue < 30)
        {
            GameObject obj = Instantiate(WoodSprite[2], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else if (_updateValue < 40)
        {
            GameObject obj = Instantiate(WoodSprite[3], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
        else
        {
            GameObject obj = Instantiate(WoodSprite[4], col, Quaternion.identity);
            obj.transform.SetParent(_setparents.transform);
        }
    }
    #endregion

    Vector2Int FindNewTileisPossible(List<KeyValuePair<Vector2Int, int>> findValue)
    {
        Vector2Int newLocation = Vector2Int.zero;
        foreach (KeyValuePair<Vector2Int, int> pair in findValue)
        {
            Vector2Int implicitPair = pair.Key;

            bool conditionMet = false;

            // 각 방향에 대해 시도
            for (int direction = 0; direction < 4; direction++)
            {
                int randomDirection = direction; 

                // 조건에 따라 방향을 수정합니다.
                switch (randomDirection)
                {
                    case 0:
                        implicitPair.y += 1;
                        break;
                    case 1:
                        implicitPair.y -= 1;
                        break;
                    case 2:
                        implicitPair.x += 1;
                        break;
                    case 3:
                        implicitPair.x -= 1;
                        break;
                }

                bool innercondition = false;
                // 조건을 만족하는지 확인합니다.
                for (int i = 1; i <= 8; i++)
                {
                    if (TileDataManager.instance.GetTileType(implicitPair.x, implicitPair.y) == i)
                    {
                        innercondition = true;
                        break;
                    }
                    else
                        conditionMet = true;
                }

                if(innercondition)
                {
                    break;
                }
                // 조건을 만족하면 루프를 종료합니다.
                if (conditionMet)
                {
                    // 새로운 곳 찾음!
                    newLocation = implicitPair;

                    if (ValidLocationisNearbyPath(newLocation))
                    {
                        newLocation = implicitPair;
                        //Debug.Log("새로 찾은 Location : " + newLocation);
                        return newLocation;
                    }
                    else
                        continue;
                }

                // 조건을 만족하지 않으면 다음 방향을 시도합니다.
                implicitPair = pair.Key; // 원래 위치로 되돌려놓고 다음 방향을 시도합니다.
            }

            // 모든 방향을 했음에도 다 실패하면 다음 pair
            if (!conditionMet)
            {
                continue; // 다음 pair로 이동합니다.
            }

        }

        return newLocation;
    }

    // 자원 증가 가능여부 위치 파악
    bool ValidLocationisNearbyPath(Vector2Int _validLocation)
    {
        Vector2Int checkingValid = _validLocation;
        for(int direction =  0; direction < 4; direction++)
        {
            int NSEW = direction;
            switch(NSEW)
            {
                case 0:
                    checkingValid.y += 1;
                    break;
                case 1:
                    checkingValid.y -= 1;
                    break;
                case 2: 
                    checkingValid.x += 1;
                    break;
                case 3:
                    checkingValid.x -= 1;
                    break;

            }

            if (TileDataManager.instance.GetTileType(checkingValid.x, checkingValid.y) == 3)
            {
                return false;
            }
            
        }
        return true;
    }


    // UI 초기화
    private void ShowTotalResource()
    {

        ElementTxt_UI.GetComponent<TextMeshProUGUI>().text = Element_reserves.ToString();
        TimberTxt_UI.GetComponent<TextMeshProUGUI>().text = Timber_reserves.ToString();
        RockTxt_UI.GetComponent<TextMeshProUGUI>().text = Rock_reserves.ToString();
    }

    public void AddTimer(float num)
    {
        Timber_reserves += num;
    }
    public void AddRock(float num)
    {
        Rock_reserves += num;
    }
}
