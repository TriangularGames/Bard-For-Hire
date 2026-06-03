using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// Manages game options such as audio volumes, resolution, and fullscreen mode,
/// providing methods to save, load, and apply these settings.
/// </summary>
public class OptionsManager : Singleton<OptionsManager>
{
    [Header("Audio Components")]
    [Tooltip("Assign the AudioMixer in the inspector.")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Settings")]
    [Tooltip("Master volume level.")]
    [SerializeField] private float masterVolume;

    [Tooltip("Music volume level.")]
    [SerializeField] private float musicVolume;
    [Tooltip("SFX volume level.")]
    [SerializeField] private float sfxVolume;

    [Header("Video Settings")]
    [Tooltip("Quality level.")]
    [SerializeField] private int qualityLevel;
    [Tooltip("Game resolution.")]
    [SerializeField] private Resolution gameResolution;
    [Tooltip("Fullscreen mode.")]
    [SerializeField] private bool isFullScreen;
    [Tooltip("Options menu UI.")]
    [SerializeField] private OptionsMenuUI optionsMenuUI;

    [Header("GamePlay Settings")]
    [Tooltip("Speed of Gameplay.")]
    [SerializeField] private int gameSpeed;

    public float MasterVolume { get { return masterVolume; } private set { masterVolume = value; } }
    public float MusicVolume { get { return musicVolume; } private set { musicVolume = value; } }
    public float SfxVolume { get { return sfxVolume; } private set { sfxVolume = value; } }
    public int QualityLevel { get { return qualityLevel; } private set { qualityLevel = value; } }
    public Resolution GameResolution { get { return gameResolution; } private set { gameResolution = value; } }
    public bool IsFullScreen { get { return isFullScreen; } private set { isFullScreen = value; } }
    public int GameSpeed { get { return gameSpeed; } private set { gameSpeed = value; } }


    public override void Awake()
    {
        base.Awake();
        if (audioMixer == null)
        {
            audioMixer = AudioManager.Instance.Master;
        }
        if (optionsMenuUI == null)
        {
            optionsMenuUI = GetComponentInChildren<OptionsMenuUI>();
        }
        LoadAllSettings();
        ToggleOptionsMenuVisibility(false);
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /// <summary>
    /// Loads all settings from PlayerPrefs and applies them.
    /// </summary>
    public void LoadAllSettings()
    {
        LoadAudioSettings();
        LoadIsFullScreen();
        LoadResolution();
        LoadQualityLevel();
        LoadGameplaySettings();
        ApplySettings();
    }

    /// <summary>
    /// Loads the audio settings from PlayerPrefs.
    /// </summary>
    private void LoadAudioSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("masterVolume");
        MusicVolume = PlayerPrefs.GetFloat("musicVolume");
        SfxVolume = PlayerPrefs.GetFloat("sfxVolume");
    }

    /// <summary>
    /// Saves the audio settings to PlayerPrefs and applies them.
    /// </summary>
    /// <param name="master">Master volume level.</param>
    /// <param name="music">Music volume level.</param>
    /// <param name="sfx">SFX volume level.</param>
    public void SaveAudioSettings(float master, float music, float sfx)
    {
        MasterVolume = master;
        MusicVolume = music;
        SfxVolume = sfx;
        PlayerPrefs.SetFloat("masterVolume", MasterVolume);
        PlayerPrefs.SetFloat("musicVolume", MusicVolume);
        PlayerPrefs.SetFloat("sfxVolume", SfxVolume);
        PlayerPrefs.Save();
        ApplySettings();
    }

    private void LoadIsFullScreen()
    {
        IsFullScreen = PlayerPrefs.GetInt("isFullScreen", 1) == 1;
    }

    public void SaveIsFullScreen(bool fullscreen)
    {
        PlayerPrefs.SetInt("isFullScreen", fullscreen ? 1 : 0);
        IsFullScreen = fullscreen;
        PlayerPrefs.Save();
        ApplySettings();
    }

    private void LoadResolution()
    {
        int width = PlayerPrefs.GetInt("resolutionWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("resolutionHeight", Screen.currentResolution.height);
        GameResolution = new Resolution { width = width, height = height };
    }

    public void SaveResolution(int width, int height, bool fullscreen)
    {
        PlayerPrefs.SetInt("resolutionWidth", width);
        PlayerPrefs.SetInt("resolutionHeight", height);
        SaveIsFullScreen(fullscreen);
        GameResolution = new Resolution { width = width, height = height };
        IsFullScreen = fullscreen;
        PlayerPrefs.Save();
        ApplySettings();
    }

    private void LoadQualityLevel()
    {
        QualityLevel = PlayerPrefs.GetInt("qualityLevel", QualitySettings.GetQualityLevel());
    }

    public void SaveQualityLevel(int qualityLevel)
    {
        QualityLevel = qualityLevel;
        PlayerPrefs.SetInt("qualityLevel", QualityLevel);
        PlayerPrefs.Save();
        ApplySettings();
    }

    private void LoadGameplaySettings()
    {
        gameSpeed = PlayerPrefs.GetInt("GameSpeed", 0);
    }

    public void SaveGameplaySettings(int GameSpeed)
    {
        gameSpeed = GameSpeed;
        PlayerPrefs.SetInt("GameSpeed", gameSpeed);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Applies current settings to the game.
    /// </summary>
    public void ApplySettings()
    {
        //AudioListener.volume = MasterVolume;
        if (audioMixer != null)
        {
            audioMixer.SetFloat("masterVolume", LinearToDecibel(MasterVolume));
            audioMixer.SetFloat("musicVolume", LinearToDecibel(MusicVolume));
            audioMixer.SetFloat("sfxVolume", LinearToDecibel(SfxVolume));
        }
        Screen.SetResolution(GameResolution.width, GameResolution.height, IsFullScreen);
        QualitySettings.SetQualityLevel(QualityLevel);
    }

    /// <summary>
    /// Converts linear volume scale (0 to 1) to decibels for the AudioMixer.
    /// </summary>
    /// <param name="linear">Linear scale volume.</param>
    /// <returns>Volume in decibels.</returns>
    private float LinearToDecibel(float linear)
    {
        return linear > 0 ? 20f * Mathf.Log10(linear) : -80f;
    }

    /// <summary>
    /// Handles the event to show the Options Menu.
    /// </summary>
    /// <param name="eventData">The event data associated with showing the options menu. Currently unused but can be extended for future use.</param>
    private void OnShowOptionsMenu(ShowOptionsMenuEvent eventData)
    {
        // Code to show the Options Menu
        ToggleOptionsMenuVisibility(true);
    }

    /// <summary>
    /// Handles the event to hide the Options Menu.
    /// </summary>
    /// <param name="eventData">The event data associated with hiding the options menu. Currently unused but can be extended for future use.</param>
    private void OnHideOptionsMenu(HideOptionsMenuEvent eventData)
    {
        // Code to hide the Options Menu
        ToggleOptionsMenuVisibility(false);
    }
    /// <summary>
    /// Subscribes to necessary events on the event bus.
    /// </summary>
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ShowOptionsMenuEvent>(OnShowOptionsMenu);
        EventBus.Subscribe<HideOptionsMenuEvent>(OnHideOptionsMenu);
    }

    /// <summary>
    /// Unsubscribes from events on the event bus.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<ShowOptionsMenuEvent>(OnShowOptionsMenu);
        EventBus.Unsubscribe<HideOptionsMenuEvent>(OnHideOptionsMenu);
    }

    /// <summary>
    /// Toggles the visibility of the options menu UI.
    /// </summary>
    /// <param name="isVisible">Whether the options menu should be visible.</param>
    public void ToggleOptionsMenuVisibility(bool isVisible)
    {
        optionsMenuUI?.gameObject.SetActive(isVisible);
    }

}

/// <summary>
/// Event struct for showing the options menu.
/// </summary>
public struct ShowOptionsMenuEvent { }

/// <summary>
/// Event struct for hiding the options menu.
/// </summary>
public struct HideOptionsMenuEvent { }