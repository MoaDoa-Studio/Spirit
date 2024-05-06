using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResouceCollider : MonoBehaviour
{
    GameObject uiObject;
    GameObject ParentObject;
    private void Start()
    {
        uiObject = GameObject.Find("[ResourceManager]");
        ParentObject = transform.parent.gameObject;
    }
    private void OnMouseEnter()
    {
        Vector3 mousePosition;
        mousePosition = Input.mousePosition;
        Debug.Log(uiObject.name);

        uiObject.GetComponent<ResouceManager>().resourceShowbox.SetActive(true);
        uiObject.GetComponent<ResouceManager>().resourceShowbox.GetComponentInChildren<Text>().text = ParentObject.GetComponent<ResourceBuilding>().Resource_reserves.ToString();
        uiObject.GetComponent<ResouceManager>().resourceShowbox.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y + 0.5f, mousePosition.z));
    }
    private void OnMouseExit()
    {
        uiObject.GetComponent<ResouceManager>().resourceShowbox.SetActive(false);
    }
}
