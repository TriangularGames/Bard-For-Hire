using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    //Audio Mixer for Volume Controls
    public AudioMixer Master;

    [Header("AudioClips")]
    [SerializeField] private AudioClip[] _musicClips;
    [SerializeField] private AudioClip[] _sfxClips;
    // Perhaps we might need this? dont have an audio channel for it
    [SerializeField] private AudioClip[] _ambienceClips;

    [Header("AudioSources")]
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    // Funcs to control Audio
    private void Start()
    {
        PlayClip(0, _musicClips[0]);
    }
    public void PlayClip(int audioType, int clipVal)
    {
        AudioSource source = new AudioSource();
        switch (audioType)
        {
            case 0:
                source = _musicAudioSource;
                break;
            case 1:
                source = _sfxAudioSource;
                break;
        }

        if (clipVal < 0 || clipVal >= _sfxClips.Length)
        {
            source.Pause();
            return;
        }

        source.clip = _sfxClips[clipVal];
        source.Play();
    }

    public void PlayClip(int audioType, AudioClip clip)
    {
        AudioSource source = new AudioSource();
        switch (audioType)
        {
            case 0:
                source = _musicAudioSource;
                break;
            case 1:
                source = _sfxAudioSource;
                break;
        }

        if (clip != null)
        {
            source.clip = clip;
            source.Play();
        }
    }

    public void Stop(int audioType)
    {
        AudioSource source = new AudioSource();
        switch (audioType)
        {
            case 0:
                source = _musicAudioSource;
                break;
            case 1:
                source = _sfxAudioSource;
                break;
        }

        source.Stop();
    }

    //Volume adjustment for Volume Sliders for Options Menu
    public void setMaster(float sliderValue)
    {
        Master.SetFloat("MasterVol", Mathf.Log10(sliderValue) * 20);
    }

    public void setMusic(float sliderValue)
    {
        Master.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
    }

    public void setSFX(float sliderValue)
    {
        Master.SetFloat("SFXVol", Mathf.Log10(sliderValue) * 20);
    }
}
