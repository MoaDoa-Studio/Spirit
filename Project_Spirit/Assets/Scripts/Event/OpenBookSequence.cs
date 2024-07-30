using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenBookSequence : MonoBehaviour
{
    public Sprite[] sprites; // 스프라이트 배열
    public Sprite[] Hotsprites; // 스프라이트 배열
    public float frameRate = 0.1f; // 프레임 속도
    public float rainDropInterval = 1.5f; // BookDrop 메서드 호출 간격
    public GameObject eventmanager;
    private Image image; // 이미지 컴포넌트
    private float timer; // 타이머
    private int currentFrame; // 현재 프레임

    private bool isBookDropPlay;
    private float rainDropTimer; // BookDrop 호출 타이머

    void Start()
    {
        if (!eventmanager.GetComponent<BookEvent>().Hoteventhasoccured)
        {
            image = GetComponent<Image>();
            if (sprites.Length > 0)
            {
                image.sprite = sprites[0];
            }
        }
        else
        {
            image = GetComponent<Image>();
                image.sprite = Hotsprites[0];
        }

        
    }

    void Update()
    {
        if(!eventmanager.GetComponent<BookEvent>().Hoteventhasoccured)
        {

            if (currentFrame < sprites.Length - 1)
            {
                timer += Time.unscaledDeltaTime;

                if (timer >= frameRate)
                {
                    timer -= frameRate;
                    currentFrame = Mathf.Min(currentFrame + 1, sprites.Length - 1);
                    image.sprite = sprites[currentFrame];
                }
            }

            rainDropTimer += Time.unscaledDeltaTime;
            if (!isBookDropPlay && rainDropTimer >= rainDropInterval)
            {
                int count = Random.Range(3, 9);
                StartCoroutine(PlayRainDrop(count));
                rainDropTimer = 0f; // 타이머 리셋
            }



        }
    }

    private IEnumerator PlayRainDrop(int count)
    {
        Debug.Log("재호출");
        isBookDropPlay = true;
        AudioClip clip = SoundManager.instance.EventSFX[count];
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume *= 0.3f;
        source.Play();
        yield return new WaitForSeconds(clip.length);
        Destroy(source);
        isBookDropPlay = false;
    }
}
