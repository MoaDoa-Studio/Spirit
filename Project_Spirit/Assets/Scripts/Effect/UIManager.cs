using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("UI 세팅")]
    [SerializeField]
    private GameObject researchUI;
    [SerializeField]
    private GameObject CraftModeUI;
    [SerializeField]
    private GameObject SpiritSpawnerUI;
    [SerializeField]
    private GameObject CharacterUI;
    [SerializeField]
    private GameObject BuildingInfo;
    [SerializeField]
    private GameObject pauseUI;
    [SerializeField]
    private List<GameObject> UI;

    bool UI_init;

    // Update is called once per frame
    void Update()
    {
      if(researchUI.activeSelf || CraftModeUI.activeSelf || SpiritSpawnerUI.activeSelf)
        {
            CharacterUI.SetActive(false);
            foreach(Transform info in BuildingInfo.transform)
            {
                info.gameObject.SetActive(false);
            }
        }
            

      // 초기 상태일때 esc를 누르면
      
      if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(pauseUI.activeSelf)
            {
                pauseUI.SetActive(false);
                return;
            }

            UI_init = true; // 기본적으로 true로 설정하고 조건에 따라 false로 변경

            for (int i = 0; i < UI.Count; i++)
            {
                if (i == 1 || i == 5 || i == 7 || i == 10)
                {
                    // i가 1, 5, 7, 10, 11인 경우 오브젝트가 켜져있는지 확인
                    if (!UI[i].gameObject.activeSelf)
                    {
                        UI_init = false;
                        break;
                    }
                }
                else
                {
                    // 나머지 i의 경우 오브젝트가 꺼져있는지 확인
                    if (UI[i].gameObject.activeSelf)
                    {
                        UI_init = false;
                        break;
                    }
                }
            }

            if (UI_init)
                pauseUI.SetActive(true);
            else
                Debug.Log("UI 키지 마시오");
        }
    
    }


    public void ToMain()
    {
        // 일시정지 화면 off
        pauseUI.SetActive(false);
        UI[23].SetActive(true);

    }

    public void ToDesktop()
    {
        // 일시정지 화면 off
        pauseUI.SetActive(false);
        UI[24].SetActive(true);
    }

    public void RetainTopause()
    {
        // 취소하기
        pauseUI.SetActive(true);
        UI[23].SetActive(false);
        UI[24].SetActive(false);
    }
}
