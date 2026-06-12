using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    [Tooltip("Pause menu UI.")]
    [SerializeField] private PauseMenuUI pauseMenuUI;

    /// <summary>
    /// Variable to keep track if the game is currently Paused
    /// </summary>
    public bool IsPaused = false;

    public override void Awake()
    {
        base.Awake();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
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
    /// Handles the event to show the Pause Menu.
    /// </summary>
    /// <param name="eventData">The event data associated with showing the options menu. Currently unused but can be extended for future use.</param>
    private void OnShowPauseMenu(ShowPauseMenuEvent eventData)
    {
        // Code to show the Pause Menu
        TogglePauseMenuVisibility(true);

        // Game is paused!
        Time.timeScale = 0;
        AudioListener.pause = true;
        IsPaused = true;
    }

    /// <summary>
    /// Handles the event to hide the Pause Menu.
    /// </summary>
    /// <param name="eventData">The event data associated with hiding the pause menu. Currently unused but can be extended for future use.</param>
    private void OnHidePauseMenu(HidePauseMenuEvent eventData)
    {
        TogglePauseMenuVisibility(false);
        if (OptionsManager.Instance.GetTimeScale() > -1)
        {
            Time.timeScale = OptionsManager.Instance.GetTimeScale();
        }
        AudioListener.pause = false;
        IsPaused = false;
    }

    /// <summary>
    /// Subscribes to necessary events on the event bus.
    /// </summary>
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ShowPauseMenuEvent>(OnShowPauseMenu);
        EventBus.Subscribe<HidePauseMenuEvent>(OnHidePauseMenu);
    }

    /// <summary>
    /// Unsubscribes from events on the event bus.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<ShowPauseMenuEvent>(OnShowPauseMenu);
        EventBus.Unsubscribe<HidePauseMenuEvent>(OnHidePauseMenu);
    }

    /// <summary>
    /// Toggles the visibility of the pause menu UI.
    /// </summary>
    /// <param name="isVisible">Whether the pause menu should be visible.</param>
    public void TogglePauseMenuVisibility(bool isVisible)
    {
        pauseMenuUI?.gameObject.SetActive(isVisible);
    }
}

/// <summary>
/// Event struct for showing the pause menu.
/// </summary>
public struct ShowPauseMenuEvent { }

/// <summary>
/// Event struct for hiding the pause menu.
/// </summary>
public struct HidePauseMenuEvent { }