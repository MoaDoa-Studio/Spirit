using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaterFallEvent : MonoBehaviour
{
    [Header("날짜 이벤트 UI 세팅")]
    [SerializeField]
    private GameObject RainDropEventUI;
    [SerializeField]
    private GameObject RainDropimgUI;
    [SerializeField]
    private GameObject NewsPaperEventUI;
    [SerializeField]
    private GameObject NewsPaper;
    [SerializeField]
    private Sprite HotbookImage;

    [Header("폭염 이벤트 UI 세팅")]
   
    [SerializeField]
    private GameObject magicStatue;

    bool newsPaperOpened = false;
    bool HotnewsPaperOpened = false;
    public bool waterFallEvent = false;

    // 폭우 경고창
    public void NewsPaperEventTrigger()
    {
        StartCoroutine(ShownewsText());
       
    }

    // 폭염 경고창
    public void HotnewsEventTrigger()
    {
       NewsPaperEventUI.SetActive(true);
        NewsPaper.GetComponent<Image>().sprite = HotbookImage;
       
        StartCoroutine(ShowHotnewsText());
    }

    // 폭우 이벤트 생성
    public void RainDropEventTrigger()
    {
        RainDropEventUI.SetActive(true);
        waterFallEvent = true;

        foreach (Transform t in RainDropEventUI.transform)
        {
            if (t.gameObject.name == "RainDrop_img")
            {
                t.GetComponent<HotDay>().enabled = false;
            }
        }

        // 물 정령 생산소 두배로 출력하게 하는 매서드

    }

    // 폭우 이벤트 종료
    public void RainDropEventEnd()
    {
        RainDropEventUI.SetActive(false);
        waterFallEvent = false;
        // 물 정령 생산소 출력양 복구하게 출력하게 하는 매서드
    }    

    //폭염 vfX 재생
    public void HotEventTrigger()
    {
        RainDropEventUI.SetActive(true);
       
        RainDropimgUI.GetComponent<RainDrop>().enabled = false;
        RainDropimgUI.GetComponent<HotDay>().enabled = true;
            
        
    }

    public void HotEventEventEnd()
    {
        RainDropEventUI.SetActive(false);
    }


    private void FixedUpdate()
    {
        if (newsPaperOpened)
        {
            // 물 정령 스폰시간 절반으로 줄이기
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SoundManager.instance.UIButtonclick();
                Debug.Log("Escape key pressed, hiding NewsPaperEventUI");

                NewsPaperEventUI.SetActive(false);
                Time.timeScale = 1f;
                newsPaperOpened =false;
                // 비 이벤트 발생
                RainDropEventTrigger();
            }
        }

        if (HotnewsPaperOpened)
        {
            // 물 정령 스폰시간 절반으로 줄이기
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SoundManager.instance.UIButtonclick();
                Debug.Log("Escape key pressed, hiding NewsPaperEventUI");

                Destroy(NewsPaperEventUI);
                Time.timeScale = 1f;
                HotnewsPaperOpened = false;

                // 폭염 대비 이벤트 발생

                // 회복 창 생김

                // 마법의 동상 생김
                magicStatue.SetActive(true);
            }
        }

    }

    // 폭우 이벤트 경고창
        IEnumerator ShownewsText()
        {
            yield return new WaitForSeconds(0.7f);
            SetActiveRecursively(NewsPaperEventUI, true);

            yield return new WaitForSeconds(3f);
          
            newsPaperOpened = true;
        }

        private void SetActiveRecursively(GameObject obj, bool state)
        {
            if(obj != null)
            {
                obj.SetActive(state);
                foreach (Transform child in obj.transform)
                {
                    child.gameObject.SetActive(state);
                }

            }
        }

    IEnumerator ShowHotnewsText()
    {
        yield return new WaitForSeconds(0.7f);
        SetActiveRecursively(NewsPaperEventUI, true);

      

        yield return new WaitForSeconds(3f);

        HotnewsPaperOpened = true;
    }
} 

