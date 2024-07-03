using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterFallEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject RainDropEventUI;
    [SerializeField]
    private GameObject NewsPaperEventUI;
    private bool rainEventOccured;

    bool newsPaperOpened = false;

    public void NewsPaperEventTrigger()
    {
        StartCoroutine(ShownewsText());
        Time.timeScale = 0.01f;
    }

 
    public void RainDropEventTrigger()
    {
        RainDropEventUI.SetActive(true);
        rainEventOccured = true;
    }

    public void RainDropEventEnd()
    {
        RainDropEventUI.SetActive(false);
        rainEventOccured = false;
    }    

    private void Update()
    {
        if (newsPaperOpened)
        {
            // 물 정령 스폰시간 절반으로 줄이기
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                NewsPaperEventUI.SetActive(false);
                Time.timeScale = 1f;

                // 비 이벤트 발생
                RainDropEventTrigger();
            }
        }
    }

        IEnumerator ShownewsText()
        {
            yield return new WaitForSecondsRealtime(0.7f);
            SetActiveRecursively(NewsPaperEventUI, true);

            yield return new WaitForSecondsRealtime(3f);
            newsPaperOpened = true;
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

