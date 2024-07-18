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

            s.source.volume = s.volume * 0.5f;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;

            if (s.source.loop)
            {
                s.source.outputAudioMixerGroup = audioMixerGroup[1];//BGM
            }
            else
            {
                s.source.outputAudioMixerGroup = audioMixerGroup[2];//SFX
            }
        }
    }

    public void Play(string name)
    {
        Sound sound = null;

        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                sound = s;
                break;
            }
        }

        if (sound == null)
        {
            Debug.Log("Sound : " + name + "File not found!!!");
            return;
        }

        sound.source.Play();
    }

    public void StopBgm()
    {
        Sound sound = null;

        foreach (Sound s in sounds)
        {
            if (s.name == bgmName)
            {
                sound = s;
                break;
            }
        }

        if (sound == null)
        {
            //Debug.Log("Stop Sound : " + name + "File not found!!!");
            return;
        }

        bgmName = "";
        sound.source.Stop();
    }

    public void PlayBgm(string name)
    {
        //기존에 플레이 되는 배경음과 새로운 배경음이 같을때
        if (bgmName == name)
        {
            return;
        }

        //기존 배경음 중단
        StopBgm();

        //새로운 배경음 플레이
        Sound sound = null;

        foreach (Sound s in sounds)
        {
            if (s.name == name)
            {
                sound = s;
                break;
            }
        }

        if (sound == null)
        {
            Debug.Log("Play Sound : " + name + "File not found!!!");
            return;
        }

        sound.volume = 0.7f;
        bgmName = sound.name;

        sound.source.Play();
    }

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
        //source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        source.Play();
        Destroy(source, clip.length);
    }
    #endregion
}

