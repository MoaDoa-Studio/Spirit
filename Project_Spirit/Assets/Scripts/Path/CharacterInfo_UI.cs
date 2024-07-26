using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInfo_UI : MonoBehaviour
{
    public void ToogleButton()
    {
        Debug.Log("버튼 눌림");
        foreach (Transform t in transform)
        {
            if (t.gameObject.name == "1" || t.gameObject.name == "2" || t.gameObject.name == "3" || t.gameObject.name == "4")
            {
                t.gameObject.SetActive(false);
            }
        }
        gameObject.SetActive(false);
    }
}