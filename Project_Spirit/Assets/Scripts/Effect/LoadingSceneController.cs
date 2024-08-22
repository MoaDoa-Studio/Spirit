using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class LoadingSceneController : MonoBehaviour
{
    static string nextScene = "AlphaScene";

    [SerializeField]
    Image progressbar;

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene("Loading");
    }

    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }
    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);
        op.allowSceneActivation = false;

        float timer = 0f;

        while(!op.isDone)
        {

            yield return null;
            if(op.progress < 0.8f)
            {
                progressbar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressbar.fillAmount = Mathf.Lerp(0.8f, 1f, timer);

                if(progressbar.fillAmount >= 1f)
                {
                    op.allowSceneActivation=true;
                    yield break;
                }
            }
        }
    }
}
