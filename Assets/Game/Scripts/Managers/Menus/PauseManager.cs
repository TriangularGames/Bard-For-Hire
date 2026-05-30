using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PauseManager : Singleton<PauseManager>
{
    [Tooltip("Pause menu UI.")]
    [SerializeField] private PauseMenuUI pauseMenuUI;

    /// <summary>
    /// Variable to keep track if the game is currently Paused
    /// </summary>
    public bool IsPaused = false;

    // Renewed each time the game unpauses — awaiting this suspends tasks while paused
    private TaskCompletionSource<bool> _pauseTCS = new TaskCompletionSource<bool>();

    public override void Awake()
    {
        base.Awake();

        _pauseTCS.TrySetResult(true);  // Start in a "completed" state so tasks don't block before first pause
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
        IsPaused = true;

        // Replace with a fresh incomplete TCS — tasks awaiting this will now suspend
        _pauseTCS = new TaskCompletionSource<bool>();
    }

    /// <summary>
    /// Handles the event to hide the Pause Menu.
    /// </summary>
    /// <param name="eventData">The event data associated with hiding the pause menu. Currently unused but can be extended for future use.</param>
    private void OnHidePauseMenu(HidePauseMenuEvent eventData)
    {
        // Code to hide the Pause Menu
        TogglePauseMenuVisibility(false);

        // Game is unpaused!
        Time.timeScale = 1;
        IsPaused = false;

        _pauseTCS.TrySetResult(true);
    }
    /// <summary>
    /// Await this in any async task to suspend execution while the game is paused.
    /// </summary>
    public Task WaitWhilePausedAsync() => _pauseTCS.Task;

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

/// <summary>
/// Helper class used for pausing async tasks
/// </summary>
public static class PauseExtensions
{
    /// <summary>
    /// Yields until the game is unpaused. Safe to call even when not paused.
    /// </summary>
    public static Task WaitWhilePausedAsync(this CancellationToken ct)
    {
        return PauseManager.Instance.WaitWhilePausedAsync();
    }

    /// <summary>
    /// Waits for the given duration in milliseconds, suspending the timer while paused.
    /// </summary>
    public static async Task DelayRespectingPause(int milliseconds, CancellationToken ct = default)
    {
        // Suspend here for the entire duration of any pause before starting the delay
        await PauseManager.Instance.WaitWhilePausedAsync();
        await Task.Delay(milliseconds, ct);
    }
}