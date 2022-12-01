using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public enum SoundType
{
    SFX,
    BGM,
    BGM2,
    END
}

public class SoundManager : SingletonDontDestroy<SoundManager>
{
    public Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
    public Dictionary<SoundType, AudioSource> audioSources = new Dictionary<SoundType, AudioSource>();
    public float[] audioVolumes = new float[(int)SoundType.END];
    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "#2")
        {
            PlaySoundClip("003_RainSound", SoundType.BGM2);
        }
        if (sceneName == "#5")
        {
            audioSources[SoundType.BGM2].Stop();
            audioSources[SoundType.BGM].Stop();
        }
    }
    public override void OnCreate()
    {
        AudioClip[] clips = Resources.LoadAll<AudioClip>("Sounds/");
        foreach (AudioClip clip in clips)
        {
            audioClips[clip.name] = clip;
        }

        string[] enumNames = System.Enum.GetNames(typeof(SoundType));
        for (int i = 0; i < (int)SoundType.END; i++)
        {
            GameObject AudioSourceObj = new GameObject(enumNames[i]);
            AudioSourceObj.transform.SetParent(transform);
            audioSources[(SoundType)i] = AudioSourceObj.AddComponent<AudioSource>();
            audioVolumes[i] = 0.5f;
        }

        audioSources[SoundType.BGM].loop = true;
        audioSources[SoundType.BGM2].loop = true;

        PlaySoundClip("001_Title", SoundType.BGM);
    }


    public AudioClip PlaySoundClip(string clipName, SoundType type, float volume = 0.5f, float pitch = 1)
    {
        AudioClip clip = audioClips[clipName];
        return PlaySoundClip(clip, type, volume, pitch);
    }
    public AudioClip PlaySoundClip(AudioClip clip, SoundType type, float volume = 0.5f, float pitch = 1)
    {
        audioSources[type].pitch = pitch;

        float curVolume = volume * audioVolumes[(int)type];
        if (type == SoundType.BGM)
        {
            audioSources[SoundType.BGM].clip = clip;
            audioSources[SoundType.BGM].volume = curVolume;
            audioSources[SoundType.BGM].Play();
        }
        else if (type == SoundType.BGM2)
        {
            audioSources[SoundType.BGM].clip = clip;
            audioSources[SoundType.BGM].volume = curVolume;
            audioSources[SoundType.BGM].Play();
        }
        else
        {
            audioSources[type].PlayOneShot(clip, curVolume);
        }

        return clip;
    }

}
