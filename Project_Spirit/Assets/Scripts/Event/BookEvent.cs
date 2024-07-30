using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BookEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject bookPrefab;
    [SerializeField]
    private GameObject bookEventUI;
    [SerializeField]
    private Sprite HotbookImage;

    private bool eventhasoccured;
    private bool Hothasoccured;
    public bool Hoteventhasoccured;
    bool bookSpawned = false;
    Node[,] nodes;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(eventhasoccured)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                // 원래 시간으로 되돌아옴
                Time.timeScale = 1f;
                bookEventUI.SetActive(false);
                GameObject.FindAnyObjectByType<Camera>().orthographicSize = 11f;

                // 연구소 퀘스트 창 나오게 한다.
            }
        }

        if(Hothasoccured)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Time.timeScale = 1f;
                bookEventUI.SetActive(false );
                GameObject.FindAnyObjectByType<Camera>().orthographicSize = 11f;
            }
        }
    }

    // BookEvent 활성화 버튼
    public void BookEventTrigger()
    {
        nodes = TileDataManager.instance.GetNodes();

        bool validLocation = true;

        if (bookSpawned) return;

        int x = UnityEngine.Random.Range(2, 100);
        int y = UnityEngine.Random.Range(2, 100);
        if (TileDataManager.instance.GetTileType(x, y) == 3)
        {

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
                    SoundManager.instance.BookDrop(0);
                }
            }
            
           if(validLocation)
           {
               Instantiate(bookPrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.identity);
                bookSpawned = true;
                return;
           }
        }
        else
        {
            BookEventTrigger();
        }    



    }


    // 책 눌렀을 때 실행되는 메서드.
    public void BookMouseOn()
    { 
        SoundManager.instance.UIButtonclick();
        bookEventUI.SetActive(true);

        // 2. 카메라 시점이 떨어진 책을 중심으로 함
        //playerCamera.transform.position = transform.position;
        // 3. 카메라 줌 아웃 효과 11 => 14로 변경
        GameObject.FindAnyObjectByType<Camera>().orthographicSize = 14f;
        // 4. 떨어진 곳의 타일이 흙 타일로 바뀜
        int ypos = (int)transform.position.y;
        int xpos = (int)transform.position.x;
        TileDataManager.instance.SetTileType(xpos, ypos, 0);

        // 5. 0.6초 이후에 검은색 레이어 위로 책이 등장
        StartCoroutine(ShowBookText());
        // 6. 일시정지 상태
        Time.timeScale = 0.01f;
        // 7. 연출을 보여준다.

        // 8. esc 눌러 뒤로 나갈 수 있게 함
        
        //if(Input.GetButton)
        // 9. 연구소 퀘스트 창이 뜨게 함



    }

    public void WeatherHotEvent()
    {
        bookEventUI.SetActive(true);
        Hoteventhasoccured = true;
        foreach(Transform transform in bookEventUI.transform)
        {
            if(transform.gameObject.name == "BookOpen_img")
            {
                transform.gameObject.GetComponent<Image>().sprite = HotbookImage;
            }
        }
        StartCoroutine(ShowWarnHotText());
        Time.timeScale = 0.01f;
    }


    IEnumerator ShowBookText()
    {
        yield return new WaitForSecondsRealtime (0.8f);
        SetActiveRecursively(bookEventUI, true);

        yield return new WaitForSecondsRealtime(7f);
       eventhasoccured = true;
    }

    IEnumerator ShowWarnHotText()
    {
        yield return new WaitForSecondsRealtime(0.8f);
        SetActiveRecursively(bookEventUI, true);

        yield return new WaitForSecondsRealtime(7f);
        Hothasoccured = true;
    }

    private void SetActiveRecursively(GameObject obj, bool state)
    {
        obj.SetActive(state);
        foreach (Transform child in obj.transform)
        {
            child.gameObject.SetActive(state);
        }
    }

}
