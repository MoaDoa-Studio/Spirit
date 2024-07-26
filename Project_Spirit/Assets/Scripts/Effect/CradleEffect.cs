using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CradleEffect : MonoBehaviour
{
    public Sprite[] sprites; // 스프라이트 배열
    public float frameRate = 0.1f; // 프레임 속도

    private SpriteRenderer image; // 이미지 컴포넌트
    private float timer; // 타이머
    private int currentFrame; // 현재 프레임

    void Start()
    {
        image = GetComponent<SpriteRenderer>();
        if (sprites.Length > 0)
        {
            image.sprite = sprites[0];
        }
    }

    void Update()
    {
       
    timer += Time.unscaledDeltaTime;

        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame = (currentFrame + 1) % sprites.Length; // 배열 끝에 도달하면 처음으로 돌아감
            image.sprite = sprites[currentFrame];
        }
    }
    
}
