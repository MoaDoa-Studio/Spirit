using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Flask_Ui : MonoBehaviour
{
    Image image;
    public Sprite[] sprites;

    public float frameRate = 0.1f;  // 프레임 속도 (초 단위)

    private int currentFrame;
    private float timer;

    void Start()
    {
        if (sprites == null)
        {
            image = GetComponent<Image>();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame = (currentFrame + 1) % sprites.Length;
            gameObject.GetComponent<Image>().sprite = sprites[currentFrame];
        }
    }
}
