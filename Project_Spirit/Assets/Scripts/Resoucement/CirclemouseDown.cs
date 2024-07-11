using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CirclemouseDown : MonoBehaviour
{
    public void OnMouseEnter()
    {
        ToggleOnObject(transform.parent, "Detail"); // 마우스가 UI 위에 있을 때 하위 UI 활성화
        
    }

    public void OnMouseExit()
    {
        ToggleOffbject(transform.parent, "Detail"); // 마우스가 UI를 벗어날 때 하위 UI 비활성화
    }

    void ToggleOnObject(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                child.gameObject.SetActive(true);
            }
        }
    }

    void ToggleOffbject(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
            {
                child.gameObject.SetActive(false);
            }
        }
    }
}
