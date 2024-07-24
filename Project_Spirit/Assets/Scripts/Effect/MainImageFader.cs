using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainImageFader : MonoBehaviour
{
    public Image image1;
    public Image image2;
    public float crossfadeDuration = 2.0f;
    public float displayDuration = 10.0f;

    private void Start()
    {
        // 처음 시작할 때 하나의 이미지만 보이도록 설정
        image1.color = new Color(image1.color.r, image1.color.g, image1.color.b, 1f);
        image2.color = new Color(image2.color.r, image2.color.g, image2.color.b, 0f);

        // 교차 효과 시작
        StartCoroutine(CrossfadeImages());
    }

    private IEnumerator CrossfadeImages()
    {
        while (true)
        {
            // 이미지 1에서 이미지 2로 페이드
            yield return StartCoroutine(Fade(image1, image2));

            // 10초간 대기
            yield return new WaitForSeconds(displayDuration);

            // 이미지 2에서 이미지 1로 페이드
            yield return StartCoroutine(Fade(image2, image1));

            // 10초간 대기
            yield return new WaitForSeconds(displayDuration);
        }
    }

    private IEnumerator Fade(Image from, Image to)
    {
        float elapsedTime = 0f;

        while (elapsedTime < crossfadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / crossfadeDuration);

            from.color = new Color(from.color.r, from.color.g, from.color.b, 1f - alpha);
            to.color = new Color(to.color.r, to.color.g, to.color.b, alpha);

            yield return null;
        }

        from.color = new Color(from.color.r, from.color.g, from.color.b, 0f);
        to.color = new Color(to.color.r, to.color.g, to.color.b, 1f);
    }
}
