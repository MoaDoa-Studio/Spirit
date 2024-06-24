using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject bookPrefab;
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
        // 1. 검은색 레이어가 깔린다.
        // 2. 카메라 시점이 떨어진 책을 중심으로 함
        // 3. 카메라 줌 아웃 효과 11 => 14로 변경
        // 4. 떨어진 곳의 타일이 흙 타일로 바뀜
        // 5. 일시정지 상태
        // 6. 0.6초 이후에 검은색 레이어 위로 책이 등장
        // 7. 연출을 보여준다.
        // 8. esc 눌러 뒤로 나갈 수 있게 함
        // 9. 연구소 퀘스트 창이 뜨게 함
        
    }

}
