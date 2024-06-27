using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenBookSequence : MonoBehaviour
{
    public Sprite[] sprites; // 스프라이트 배열
    public float frameRate = 0.1f; // 프레임 속도

    private Image image; // 이미지 컴포넌트
    private float timer; // 타이머
    private int currentFrame; // 현재 프레임

    void Start()
    {
        image = GetComponent<Image>();
        if (sprites.Length > 0)
        {
            image.sprite = sprites[0];
        }
    }

    void Update()
    {
        if (currentFrame < sprites.Length - 1)
        {
            timer += Time.deltaTime;

            if (timer >= frameRate)
            {
                timer -= frameRate;
                currentFrame = Mathf.Min(currentFrame + 1, sprites.Length - 1);
                image.sprite = sprites[currentFrame];
            }
        }
    }
}
