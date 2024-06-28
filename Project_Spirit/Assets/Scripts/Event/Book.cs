using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book : MonoBehaviour
{
    [SerializeField]
    private GameObject bookEventUI;
    private GameObject bookOpenimg;
    [SerializeField]
    private GameObject playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        bookEventUI = GameObject.Find("[EventManager]");

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnMouseDown()
    {
        // 1. 검은색 레이어가 깔린다. v
        bookEventUI.GetComponent<BookEvent>().BookMouseOn();
        Destroy(this.gameObject);
    }

}

