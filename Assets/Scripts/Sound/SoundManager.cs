using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Sound;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    private float bgmVolume = 0.5f;

    [SerializeField, Range(0f, 1f)]
    private float sfxVolume = 0.5f;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    [SerializeField]
    private AudioClipWrapper[] bgmClips;

    [SerializeField]
    private AudioClipWrapper[] sfxClips;

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private GameManager gameManager;

    public float BGMVolume
    {
        get { return bgmVolume; }
        set
        {
            bgmVolume = Mathf.Clamp01(value);
            // map 0.0-1.0 to -80dB to 0dB
            audioMixer?.SetFloat("BGMVolume", Mathf.Log10(bgmVolume) * 20);
            PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        }
    }

    public float SFXVolume
    {
        get { return sfxVolume; }
        set
        {
            sfxVolume = Mathf.Clamp01(value);
            audioMixer?.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        }
    }

    public float BGMLowpassCutoff
    {
        get
        {
            float value = 0f;
            audioMixer?.GetFloat("BGMLowpassCutoff", out value);
            return value;
        }
        set
        {
            audioMixer?.SetFloat("BGMLowpassCutoff", value);
        }
    }

    private void Awake()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("BGM")[0];
            bgmSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        }

        bgmSource.volume = bgmVolume;
        sfxSource.volume = sfxVolume;
    }

    private void Start()
    {
        foreach(var clipWrapper in bgmClips)
        {
            if(GameObject.Find(clipWrapper.Name) != null)
            {
                playBgm(clipWrapper.Name);
                break;
            }
        }

        gameManager = GetComponent<GameManager>();
        gameManager.GamePaused += ToggleLowpassOn;

        gameManager.GameResumed += ToggleLowpassOff;

        gameManager.LevelFailed += ToggleLowpassOn;

        gameManager.LevelWon += ToggleLowpassOn;
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.GamePaused -= ToggleLowpassOn;
            gameManager.GameResumed -= ToggleLowpassOff;
            gameManager.LevelFailed -= ToggleLowpassOn;
            gameManager.LevelWon -= ToggleLowpassOn;
        }
    }

    public void playBgm(string key)
    {
        AudioClip clip = bgmClips.Where(clip => clip.Name== key).FirstOrDefault()?.AudioClip;
        if (clip != null)
        {
            if (bgmSource == null)
            {
                bgmSource = gameObject.AddComponent<AudioSource>();
                bgmSource.loop = true;
                bgmSource.volume = bgmVolume;
            }
            bgmSource.clip = clip;
            bgmSource.Play();
        }
    }

    public void playSfx(string key)
    {
        AudioClip clip = sfxClips.Where(clip => clip.Name == key).FirstOrDefault()?.AudioClip;
        if (clip != null)
        {
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.volume = sfxVolume;
            }
            sfxSource.PlayOneShot(clip);
        }
    }

    public void ToggleLowpassOn()
    {
        ToggleLowpass(true);
    }

    public void ToggleLowpassOff()
    {
        ToggleLowpass(false);
    }

    public void ToggleLowpass(bool active)
    {
        if (!active)
        {
            audioMixer.SetFloat("lowpassCutoffFreqBGM", 22000f); // disable lowpass
        }
        else
        {
            audioMixer.SetFloat("lowpassCutoffFreqBGM", 500f); // enable lowpass
        }
    }
}
