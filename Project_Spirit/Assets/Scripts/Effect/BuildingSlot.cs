using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class BuildingSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,IPointerClickHandler
{
    // Start is called before the first frame update
    [SerializeField]
    GameObject itemTooltip;

    BuildTooltipUI buildTooltipUI;
    Transform go_base;
    Button button;

    UnityEvent myEvent;
    void Start()
    {  
        buildTooltipUI = itemTooltip.GetComponent<BuildTooltipUI>();
        button = GetComponent<Button>();
        // onClick 이벤트를 가져옴
        myEvent = button.onClick;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Get the target object of the first listener
        Object targetObject = myEvent.GetPersistentTarget(2);

        if(targetObject is GameObject)
        {
            GameObject gameObject = (GameObject)targetObject;
          //  Debug.Log(gameObject.GetComponent<Building>().BuildID);
            buildTooltipUI.ShowToolTip(gameObject.GetComponent<Building>().BuildID, transform.position);
        }
        //Debug.Log("Building UI : imformation " + targetObject.name);
        
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        buildTooltipUI.HideToolTip();
    }


}
