using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BookEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject bookPrefab;
   
    Node[,] nodes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BookEventTrigger()
    {
        nodes = TileDataManager.instance.GetNodes();

        int x = UnityEngine.Random.Range(1, 103);
        int y = UnityEngine.Random.Range(1, 103);
        if (TileDataManager.instance.GetTileType(x, y) == 3)
        {
            bool validLocation = true;

           for(int i = 0; i < 5; i++)
            {
                for(int j = 0; j < 5; j++)
                {
                    if (TileDataManager.instance.GetTileType(x + i, y + j) == 1)
                    {
                        validLocation = false;
                        break;
                    }
                   
                }
                if(!validLocation)
                {
                    Debug.Log("책이 소환할 수 없는 위치입니다.");
                    BookEventTrigger();
                }
            }
            
           if(validLocation)
           {
               Instantiate(bookPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                return;
           }
        }
        else
        {
            BookEventTrigger();
        }    



    }

}
