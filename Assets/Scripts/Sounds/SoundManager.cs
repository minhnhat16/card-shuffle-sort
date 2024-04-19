using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public enum Music
    {
        NULL,
        MainScreenMusic,
        GamplayMusic
    }

    public enum SFX
    {
        NULL,
        DropCircleSFX,
        LandedSFX,
        PopSFX,
        PopSFX_2,
        PopSFX_3,
        PopSFX_4,
        UIClickSFX,
        UIClickSFX_2,
        UIClickSFX_3,
        UIPopSFX,
        SpinSFX,
    }

    [SerializeField] public SoundFactory soundFactory;

    private Dictionary<SFX, float> sfxTimerDictionary;

    public Dictionary<SFX, float> sfxTimerDespawnDictionary;

    public MusicGameObject musicObject;

    public bool musicSetting;
    public bool sfxSetting;

    public void Init()
    {
        soundFactory = ConfigFileManager.Instance.SoundFactory;
        sfxTimerDictionary = new Dictionary<SFX, float>();
        sfxTimerDespawnDictionary = new Dictionary<SFX, float>();

        for (int i = 0; i < soundFactory.sfxList.Count; i++)
        {
            sfxTimerDictionary.Add(soundFactory.sfxList[i].sfx, soundFactory.sfxList[i].timer);
        }
        for (int i = 0; i < soundFactory.sfxList.Count; i++)
        {
            sfxTimerDespawnDictionary.Add(soundFactory.sfxList[i].sfx, soundFactory.sfxList[i].timeToDespawn);
        }
        musicSetting = true;
        sfxSetting = true;
    }

    public void VolumeSetting(bool musicSetting, bool sfxSetting)
    {
        this.musicSetting = musicSetting;
        this.sfxSetting = sfxSetting;
        SettingMusicVolume(this.musicSetting);
        SettingSFXVolume(this.sfxSetting);
    }

    private bool CanPlaySFX(SFX sfx)
    {
        switch (sfx)
        {
            case SFX.DropCircleSFX:
                if (sfxTimerDictionary.ContainsKey(sfx))
                {
                    float lastTimePlayed = sfxTimerDictionary[sfx];
                    float mainBackgroundMaxTimer = 0.5f;

                    if (lastTimePlayed + mainBackgroundMaxTimer < Time.time)
                    {
                        sfxTimerDictionary[sfx] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case SFX.LandedSFX:
                if (sfxTimerDictionary.ContainsKey(sfx))
                {
                    float lastTimePlayed = sfxTimerDictionary[sfx];
                    float mainBackgroundMaxTimer = 0.5f;

                    if (lastTimePlayed + mainBackgroundMaxTimer < Time.time)
                    {
                        sfxTimerDictionary[sfx] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case SFX.PopSFX:
                if (sfxTimerDictionary.ContainsKey(sfx))
                {
                    float lastTimePlayed = sfxTimerDictionary[sfx];
                    float mainBackgroundMaxTimer = 0.1f;

                    if (lastTimePlayed + mainBackgroundMaxTimer < Time.time)
                    {
                        sfxTimerDictionary[sfx] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            case SFX.PopSFX_2:
                if (sfxTimerDictionary.ContainsKey(sfx))
                {
                    float lastTimePlayed = sfxTimerDictionary[sfx];
                    float mainBackgroundMaxTimer = 0.1f;

                    if (lastTimePlayed + mainBackgroundMaxTimer < Time.time)
                    {
                        //Debug.Log("case SFX.PopSFX_2:");
                        sfxTimerDictionary[sfx] = Time.time;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            default:
                return true;
        }
    }

    public void PlayMusic(Music music)
    {
        musicObject.music = music;
        AudioSource audioSource = musicObject.GetComponent<AudioSource>();
        SettingMusicVolume(musicSetting);
        audioSource.clip = GetMusicAudioClip(music);
        audioSource.Play();
        //Debug.Log("Music " + music + " played!");
    }

    public void PlaySFX(SFX sfx)
    {
        if (CanPlaySFX(sfx))
        {
            SFXGameObj soundGameObj = SoundGameObjPool.instance.pool.SpawnNonGravity();
            soundGameObj.AutoDespawnSFX(sfx);
            soundGameObj.sfx = sfx;
            AudioSource audioSource = soundGameObj.gameObject.GetComponent<AudioSource>();
            //Debug.Log(soundGameObj.name.ToString());
            SettingSFXVolume(sfxSetting);
            audioSource.PlayOneShot(GetSFXAudioClip(sfx));
            //Debug.Log("SFX " + sfx + " played!");
        }
    }
    public void PlaySFXWithVolume(SFX sfx,float value)
    {
        if (CanPlaySFX(sfx))
        {
            SFXGameObj soundGameObj = SoundGameObjPool.instance.pool.SpawnNonGravity();
            soundGameObj.sfx = sfx;
            AudioSource audioSource = soundGameObj.gameObject.GetComponent<AudioSource>();
            //Debug.Log(soundGameObj.name.ToString());
            SettingSFXVolumeWithValue(sfxSetting,value);
            audioSource.PlayOneShot(GetSFXAudioClip(sfx));
            //Debug.Log("SFX " + sfx + " played!");
        }
    }
    public void StopMusic()
    {
        AudioSource audioSource = musicObject.GetComponent<AudioSource>();
        audioSource.Stop();
    }

    public void StopSFX(SFX sfx)
    {
        foreach (SFXGameObj obj in SoundGameObjPool.instance.pool.list)
        {
            if (obj.sfx == sfx && obj.gameObject.activeSelf)
            {
                //Debug.Log("Stop " + sfx + " sfx");
                SoundGameObjPool.instance.pool.DeSpawnNonGravity(obj);
            }
        }
    }

    public void SettingMusicVolume(bool valid)
    {
        AudioSource audioSource = musicObject.GetComponent<AudioSource>();

        if (valid)
        {
            Debug.Log("UnMute music");
            audioSource.volume = 1f;
        }
        else
        {
            Debug.Log("mute music");
            audioSource.volume = 0f;
        }
    }

    public void SettingSFXVolume(bool valid)
    {
        foreach (SFXGameObj obj in SoundGameObjPool.instance.pool.list)
        {
            if (obj.sfx != SFX.NULL && obj.gameObject.activeSelf)
            {
                if (valid)
                {
                    //Debug.Log("UnMute SFX");
                    obj.GetComponent<AudioSource>().volume = 1f;
                }
                else
                {
                    //Debug.Log("Mute SFX");
                    obj.GetComponent<AudioSource>().volume = 0f;
                }
            }
        }
    }
    public void SettingSFXVolumeWithValue(bool valid,float value)
    {
        foreach (SFXGameObj obj in SoundGameObjPool.instance.pool.list)
        {
            if (obj.sfx != SFX.NULL && obj.gameObject.activeSelf)
            {
                if (valid)
                {
                    //Debug.Log("UnMute SFX");
                    obj.GetComponent<AudioSource>().volume = value;
                }
                else
                {
                    //Debug.Log("Mute SFX");
                    obj.GetComponent<AudioSource>().volume = 0f;
                }
            }
        }
    }
    public AudioClip GetMusicAudioClip(Music music)
    {
        foreach (SoundFactory.Music_SFX item in soundFactory.musicList)
        {
            if (item.music == music)
            {
                return item.audioClip;
            }
        }
        Debug.LogError("Music " + music + " not found!");
        return null;
    }

    public AudioClip GetSFXAudioClip(SFX sfx)
    {
        foreach (SoundFactory.Sound_SFX item in soundFactory.sfxList)
        {
            if (item.sfx == sfx)
            {
                return item.audioClip;
            }
        }
        Debug.LogError("SFX " + sfx + " not found!");
        return null;
    }
}
