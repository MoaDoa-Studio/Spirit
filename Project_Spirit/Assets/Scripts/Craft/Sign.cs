using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Sign : MonoBehaviour, IPointerClickHandler
{
    public GameObject DeleteUI;
    public bool first = false;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!first)
        {
            first = true;
            return;
        }
        DeleteUI.SetActive(true);
    }

    public void OnClickDelete()
    {
        TileDataManager.instance.SetTileType((int)transform.position.x, (int)transform.position.y, 3);
        Destroy(this.gameObject);
    }

    public void OnClickDeleteCancel()
    {
        DeleteUI.SetActive(false);
    }
}
