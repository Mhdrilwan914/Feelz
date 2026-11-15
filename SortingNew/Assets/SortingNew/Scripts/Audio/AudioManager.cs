using System.Collections.Generic;
using UnityEngine;
// Add to any GameObject. Clips live in Resources/Audio (select, deselect, invalid_move, button_tap, liquid_pour, liquid_land,
// bottle_complete_lid_close, confetti, bottle_fly, order_delivery, level_complete, power_up, hint_ping, undo, music_loop).

[DisallowMultipleComponent]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource _soundEffectsSource;
    private AudioSource _musicSource;
    private readonly Dictionary<string, AudioClip> _clipCache = new Dictionary<string, AudioClip>();

    public bool SoundOn
    {
        get => PlayerPrefs.GetInt("snd", 1) == 1;
        set => PlayerPrefs.SetInt("snd", value ? 1 : 0);
    }

    public bool MusicOn
    {
        get => PlayerPrefs.GetInt("mus", 1) == 1;
        set
        {
            PlayerPrefs.SetInt("mus", value ? 1 : 0);
            if (_musicSource)
            {
                _musicSource.mute = !value;
            }
        }
    }

    public float Volume
    {
        get => PlayerPrefs.GetFloat("vol", .6f);
        set
        {
            PlayerPrefs.SetFloat("vol", value);
            AudioListener.volume = value;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        _soundEffectsSource = gameObject.AddComponent<AudioSource>();
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.clip = Resources.Load<AudioClip>("Audio/music_loop");
        _musicSource.loop = true;
        _musicSource.volume = .35f;
        _musicSource.mute = !MusicOn;
        _musicSource.Play();
        AudioListener.volume = Volume;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateInstance()
    {
        if (Instance == null)
        {
            var go = new GameObject(nameof(AudioManager));
            DontDestroyOnLoad(go);
            Instance = go.AddComponent<AudioManager>();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    public static void Play(string clipName, float volumeMultiplier = 1f)
    {
        if (Instance == null || !Instance.SoundOn)
        {
            return;
        }

        if (!Instance._clipCache.TryGetValue(clipName, out var audioClip))
        {
            Instance._clipCache[clipName] = audioClip = Resources.Load<AudioClip>($"Audio/{clipName}");
        }

        if (audioClip)
        {
            Instance._soundEffectsSource.PlayOneShot(audioClip, volumeMultiplier);
        }
    }
}
