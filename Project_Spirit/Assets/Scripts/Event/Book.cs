using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField]
    private GameObject bookEventUI;
    private GameObject bookOpenimg;
    [SerializeField]
    private GameObject playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        bookEventUI = GameObject.Find("BookEvent");
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMouseDown()
    {
       // 1. 검은색 레이어가 깔린다. v
       bookEventUI.SetActive(true);
       // 2. 카메라 시점이 떨어진 책을 중심으로 함
       //playerCamera.transform.position = transform.position;
        // 3. 카메라 줌 아웃 효과 11 => 14로 변경
        GameObject.FindAnyObjectByType<Camera>().orthographicSize = 14f;
        // 4. 떨어진 곳의 타일이 흙 타일로 바뀜
        int ypos = (int)transform.position.y;
        int xpos = (int)transform.position.x;
        TileDataManager.instance.SetTileType(xpos, ypos, 0);

        // 5. 일시정지 상태
        Time.timeScale = 0f;
        // 6. 0.6초 이후에 검은색 레이어 위로 책이 등장
        StartCoroutine(ShowBookText());
        // 7. 연출을 보여준다.
        // 8. esc 눌러 뒤로 나갈 수 있게 함
        // 9. 연구소 퀘스트 창이 뜨게 함
        
        

    }

    IEnumerator ShowBookText()
    {
        yield return new WaitForSeconds(0.8f);
        SetActiveRecursively(bookEventUI, true);

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
