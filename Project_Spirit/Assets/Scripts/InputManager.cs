using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class InputManager : MonoBehaviour
{
    [Header("입력 키 세팅")]
    public GameObject tutorialObject;
    public GameObject guide;
    public GameObject pauseUI;
    public GameObject WinUI;
    public GameObject LoseUI;
    void Update()
    {
        // F11 키가 눌렸을 때 => 듀토리얼
        if (Input.GetKeyDown(KeyCode.F1))
        {
          
            if (tutorialObject != null)
            {
             
                tutorialObject.SetActive(!tutorialObject.activeSelf);
            }
            else
            {
               
            }
        }

        // F11 키가 눌렸을 때 => 가이드
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (guide != null)
            {
                
                guide.SetActive(!guide.activeSelf);
            }
            else
            {
               
            }
        }

        // 중지
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (pauseUI != null)
            {
                // 오브젝트의 활성 상태를 토글
                pauseUI.SetActive(!pauseUI.activeSelf);
            }
            else
            {

            }
        }




    }







    public void LoadMainScene()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitApplication()
    {
        Application.Quit();
    }
}
