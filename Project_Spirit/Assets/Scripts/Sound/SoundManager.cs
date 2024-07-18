using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public partial class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] sounds;
    public AudioClip[] roadSFX;

    //플레이 배경음 이름
    public string bgmName = "";

    public AudioSource audioSource;
    public AudioMixer audioMixer;

    private Coroutine currentFadeOutCoroutine;

    #region singleton
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        //DontDestroyOnLoad(gameObject);

        AudioMixerGroup[] audioMixerGroup = audioMixer.FindMatchingGroups("Master");

        foreach (Sound s in sounds)
        {
            s.source = this.gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if (s.source.loop)
            {
                s.source.outputAudioMixerGroup = audioMixerGroup[0];//BGM
            }
            else
            {
                s.source.outputAudioMixerGroup = audioMixerGroup[1];//SFX
            }
        }
    }
    #endregion
    #region Main BGM 설치 로직
    public void Play(string name)
    {
        Sound sound = FindSound(name);
        if (sound == null)
        {
            Debug.Log("Sound not found: " + name);
            return;
        }

        sound.source.Play();
    }

    IEnumerator FadeOutAndStop(Sound sound, float fadeDuration)
    {
        float startVolume = sound.source.volume;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            sound.source.volume = Mathf.Lerp(startVolume, 0f, timer / fadeDuration);
            yield return null;
        }

        sound.source.Stop();
        sound.source.volume = startVolume;
    }

    public void StopBgm()
    {
        Sound sound = FindSound(bgmName);
        if (sound == null)
        {
            return;
        }

        StartCoroutine(FadeOutAndStop(sound, 0.2f));
    }


    IEnumerator PlayBgmDelayed(string name, float delay)
    {
        yield return new WaitForSeconds(delay);

        Sound sound = FindSound(name);
        if (sound == null)
        {
            Debug.Log("BGM not found: " + name);
            yield break;
        }

        sound.volume = 0.7f;
        bgmName = sound.name;

        sound.source.Play();
    }

    public void PlayBgm(string name)
    {
        if (bgmName == name)
        {
            return;
        }

        StopBgm();

        StartCoroutine(PlayBgmDelayed(name, 0.1f));
    }

    Sound FindSound(string name)
    {
        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                return s;
            }
        }
        return null;
    }

    #endregion
}


public partial class SoundManager
{
    #region 길 설치 사운드

    public void SetRoadSoundByCount(int count)
    {
        if(count >= 0 && count < 2)
        {
            RoadSoundPlayLogic(0);
        }
        else if(count < 4)
        {
            RoadSoundPlayLogic(1);
        }
        else if(count < 6)
        {
            RoadSoundPlayLogic(2);

        }
        else if(count < 8)
        {
            RoadSoundPlayLogic(3);
        }
        else if(count < 10)
        {
            RoadSoundPlayLogic(4);
        }
        else if (count < 12)
        {
            RoadSoundPlayLogic(5);
        }
        else if (count < 14)
        {
            RoadSoundPlayLogic(6);
        }
        else if (count < 16)
        {
            RoadSoundPlayLogic(7);
        }
        else if(count < 18)
        {
            RoadSoundPlayLogic(8);

        }
        else if (count < 20)
        {
            RoadSoundPlayLogic(9);

        }
        else if (count < 22)
        {
            RoadSoundPlayLogic(10);

        }
        else if (count < 24)
        {
            RoadSoundPlayLogic(11);

        }
        else if (count < 26)
        {
            RoadSoundPlayLogic(12);

        }
        else if (count < 28)
        {
            RoadSoundPlayLogic(13);

        }
        else
            RoadSoundPlayLogic(14);

        
     
     

    }

    void RoadSoundPlayLogic(int count)
    {
        AudioClip clip = roadSFX[count];
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume *= 0.3f;
        //source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        source.Play();
        Destroy(source, clip.length);
    }
    #endregion
}

