using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTheme : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SetMainThemeBGM();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 메인테마 사운드
    public void SetMainThemeBGM()
    {

        SoundManager.instance.PlayBgm("MainTheme");
    }
}
