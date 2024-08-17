using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFallEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject RainDropEventUI;
    [SerializeField]
    private GameObject RainDropimgUI;
    [SerializeField]
    private GameObject NewsPaperEventUI;
   
    [SerializeField]
    bool newsPaperOpened = false;
    public bool waterFallEvent = false;

    public void NewsPaperEventTrigger()
    {
        StartCoroutine(ShownewsText());
       
    }


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

    public void RainDropEventEnd()
    {
        RainDropEventUI.SetActive(false);
        waterFallEvent = false;
        // 물 정령 생산소 출력양 복구하게 출력하게 하는 매서드
    }    

    public void HotEventTrigger()
    {
        RainDropEventUI.SetActive(true);
        
         RainDropimgUI.gameObject.SetActive(true);
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

                Destroy(NewsPaperEventUI);
                Time.timeScale = 1f;
                newsPaperOpened =false;
                // 비 이벤트 발생
                RainDropEventTrigger();
            }
        }
    }

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
    } 

