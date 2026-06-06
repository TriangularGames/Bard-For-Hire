using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Manages the UI interactions for the Options Menu, allowing players to adjust settings such as resolution,
/// fullscreen mode, and audio volumes. It communicates changes through a generic event bus system.
/// </summary>
public class OptionsMenuUI : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Dropdown for resolution selection.")]
    [SerializeField] private TMP_Dropdown resolutionDropwdon;
    [Tooltip("Dropdown for quality selection.")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [Tooltip("Toggle for fullscreen mode.")]
    [SerializeField] private Toggle fullscreenToggle;
    [Tooltip("Slider for master volume.")]
    [SerializeField] private Slider masterVolumeSlider;
    [Tooltip("Slider for music volume.")]
    [SerializeField] private Slider musicVolumeSlider;
    [Tooltip("Slider for SFX volume.")]
    [SerializeField] private Slider sfxVolumeSlider;
    [Tooltip("Dropdown for GameSpeed.")]
    [SerializeField] private TMP_Dropdown gamespeedDropdown;
    [SerializeField] private string[] gamespeedOptions;
    [Tooltip("Button to close the options menu.")]
    [SerializeField] private Button backButton;
    [Header("Other Components")]
    private Resolution[] resolutions;
    private string[] qualityOptions;
    

    private void Awake()
    {
        InitializeUIState();
        InitializeUIComponents();
    }

    private void Start()
    {
        Debug.Log(Equals(OptionsManager.Instance, null) ? "OptionsManager instance is null in OptionsMenuUI Start" : "OptionsManager instance found in OptionsMenuUI Start");
        UpdateUIWithCurrentSettings();
    }

    private void OnEnable()
    {
        Debug.Log("OptionsMenuUI enabled, updating UI with current settings.");
        UpdateUIWithCurrentSettings();
        backButton.onClick.AddListener(() => AudioManager.Instance.Back());
    }

    private void OnDisable()
    {
        backButton.onClick.RemoveListener(() => AudioManager.Instance.Back());
    }

    /// <summary>
    /// Initializes UI components and registers callbacks for UI interactions.
    /// </summary>
    private void InitializeUIComponents()
    {
        // Back Button
        backButton.onClick.AddListener(() =>
        {
            MenuManager.Instance.ResumePreviousState();
        });

        // Fullscreen Toggle
        fullscreenToggle.onValueChanged.AddListener(isOn =>
        {
            OptionsManager.Instance.SaveIsFullScreen(isOn);
        });

        // Master Volume Slider
        masterVolumeSlider.onValueChanged.AddListener(volume =>
        {
            OptionsManager.Instance.SaveAudioSettings(
                volume,
                musicVolumeSlider.value,
                sfxVolumeSlider.value
            );
        });

        // Music Volume Slider
        musicVolumeSlider.onValueChanged.AddListener(volume =>
        {
            OptionsManager.Instance.SaveAudioSettings(
                masterVolumeSlider.value,
                volume,
                sfxVolumeSlider.value
            );
        });

        // SFX Volume Slider
        sfxVolumeSlider.onValueChanged.AddListener(volume =>
        {
            OptionsManager.Instance.SaveAudioSettings(
                masterVolumeSlider.value,
                musicVolumeSlider.value,
                volume
            );
        });

        // Assuming resolutionDropdown is correctly set up elsewhere
        resolutionDropwdon.onValueChanged.AddListener(index =>
        {
            OptionsManager.Instance.SaveResolution(
                resolutions[index].width,
                resolutions[index].height,
                fullscreenToggle.isOn
            );
        });

        qualityDropdown.onValueChanged.AddListener(index =>
        {
            OptionsManager.Instance.SaveQualityLevel(index);
        });

        gamespeedDropdown.onValueChanged.AddListener(index =>
        {
            OptionsManager.Instance.SaveGameplaySettings(index);
        });

    }

    /// <summary>
    /// Sets the initial state of the UI based on current game settings.
    /// </summary>
    private void InitializeUIState()
    {
        LoadResolutions();
        LoadQualityOptions();
        LoadGameSpeedOptions();
    }

    /// <summary>
    /// Updates the UI with the current settings from the OptionsManager.
    /// </summary>
    private void UpdateUIWithCurrentSettings()
    {
        // Ensure OptionsManager instance is available
        if (OptionsManager.Instance != null)
        {
            // Update sliders
            masterVolumeSlider.value = OptionsManager.Instance.MasterVolume;
            musicVolumeSlider.value = OptionsManager.Instance.MusicVolume;
            sfxVolumeSlider.value = OptionsManager.Instance.SfxVolume;

            // Update toggle
            fullscreenToggle.isOn = OptionsManager.Instance.IsFullScreen;

            // Update resolution dropdown - find the current resolution index
            if (resolutions != null)
            {
                int currentResolutionIndex = Array.FindIndex(resolutions, r => r.width == OptionsManager.Instance.GameResolution.width && r.height == OptionsManager.Instance.GameResolution.height);
                resolutionDropwdon.value = resolutionDropwdon.value = currentResolutionIndex;
            }

            if (qualityOptions != null)
            {
                qualityDropdown.value = OptionsManager.Instance.QualityLevel;
            }

            if (gamespeedDropdown != null)
            {
                gamespeedDropdown.value = OptionsManager.Instance.GameSpeed;
            }
        }
        else
        {
            Debug.LogWarning("OptionsManager instance not found. Make sure it's initialized before accessing it.");
        }
    }

    /// <summary>
    /// Loads and populates the resolution dropdown with available screen resolutions.
    /// </summary>
    private void LoadResolutions()
    {
        resolutions = Screen.resolutions.Select(
            resolution => new Resolution { width = resolution.width, height = resolution.height })
            .DistinctBy(res => new { res.width, res.height })
            .ToArray();
        Array.Reverse(resolutions); // Optional: reverse to have the highest resolution at the top
        List<string> options = new List<string>();
        var currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var resolution = resolutions[i];
            var option = $"{resolution.width}x{resolution.height}";
            options.Add(option);

            if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropwdon.ClearOptions();
        resolutionDropwdon.AddOptions(options);
        resolutionDropwdon.value = currentResolutionIndex;
        resolutionDropwdon.RefreshShownValue();
    }

    /// <summary>
    /// Loads and populates the quality dropdown with available quality levels.
    /// </summary>
    private void LoadQualityOptions()
    {
        qualityOptions = QualitySettings.names;
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(qualityOptions.ToList());
        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();
    }

    private void LoadGameSpeedOptions()
    {
        gamespeedDropdown.ClearOptions();
        gamespeedDropdown.AddOptions(gamespeedOptions.ToList());
        gamespeedDropdown.value = OptionsManager.Instance.GameSpeed;
        gamespeedDropdown.RefreshShownValue();
    }
}