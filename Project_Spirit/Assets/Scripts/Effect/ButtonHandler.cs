using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 originalPosition;
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f); // 커졌을 때의 크기
    public float hoverYOffset = 10f;

    private void Start()
    {
        originalScale = transform.localScale; // 원래 크기를 저장합니다.
        originalPosition = transform.localPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = hoverScale; // 커서가 버튼 위에 올려졌을 때 크기를 변경합니다.
        transform.localPosition = new Vector3(originalPosition.x, originalPosition.y + hoverYOffset, originalPosition.z);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale; // 커서가 버튼에서 벗어났을 때 원래 크기로 되돌립니다.
        transform.localPosition = originalPosition;
    }
}
