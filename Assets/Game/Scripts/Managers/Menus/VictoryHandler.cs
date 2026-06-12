using UnityEngine;

public class VictoryHandler : MonoBehaviour
{
    [Tooltip("Victory menu UI.")]
    [SerializeField] private VictoryUI victoryUI;

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
    private void OnShowVictoryMenu(ShowVictoryMenuEvent eventData)
    {
        // Code to show the Pause Menu
        ToggleVictoryMenuVisibility(true);

        // Game is paused!
        Time.timeScale = 0;
    }

    /// <summary>
    /// Handles the event to hide the Pause Menu.
    /// </summary>
    /// <param name="eventData">The event data associated with hiding the pause menu. Currently unused but can be extended for future use.</param>
    private void OnHideVictoryMenu(HideVictoryMenuEvent eventData)
    {
        // Code to hide the Pause Menu
        ToggleVictoryMenuVisibility(false);

        // Game is unpaused!
        Time.timeScale = OptionsManager.Instance.GetTimeScale();
    }
    /// <summary>
    /// Subscribes to necessary events on the event bus.
    /// </summary>
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ShowVictoryMenuEvent>(OnShowVictoryMenu);
        EventBus.Subscribe<HideVictoryMenuEvent>(OnHideVictoryMenu);
    }

    /// <summary>
    /// Unsubscribes from events on the event bus.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<ShowVictoryMenuEvent>(OnShowVictoryMenu);
        EventBus.Unsubscribe<HideVictoryMenuEvent>(OnHideVictoryMenu);
    }

    /// <summary>
    /// Toggles the visibility of the victory menu UI.
    /// </summary>
    /// <param name="isVisible">Whether the victory menu should be visible.</param>
    public void ToggleVictoryMenuVisibility(bool isVisible)
    {
        victoryUI?.gameObject.SetActive(isVisible);
    }
}

/// <summary>
/// Event struct for showing the pause menu.
/// </summary>
public struct ShowVictoryMenuEvent { }

/// <summary>
/// Event struct for hiding the pause menu.
/// </summary>
public struct HideVictoryMenuEvent { }