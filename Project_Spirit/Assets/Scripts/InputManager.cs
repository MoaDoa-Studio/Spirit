using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{

    public GameObject tutorialObject;
    public GameObject guide;

    void Update()
    {
        // F11 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // targetObject가 null이 아닌지 확인
            if (tutorialObject != null)
            {
                // 오브젝트의 활성 상태를 토글
                tutorialObject.SetActive(!tutorialObject.activeSelf);
            }
            else
            {
               
            }
        }

        // F11 키가 눌렸을 때
        if (Input.GetKeyDown(KeyCode.F2))
        {
            // targetObject가 null이 아닌지 확인
            if (guide != null)
            {
                // 오브젝트의 활성 상태를 토글
                guide.SetActive(!guide.activeSelf);
            }
            else
            {
               
            }
        }
    }


}
