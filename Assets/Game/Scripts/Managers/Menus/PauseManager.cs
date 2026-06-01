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

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameLifetime.Cancel(); // Cancels ALL async tasks across every class
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
        TogglePauseMenuVisibility(false);
        Time.timeScale = 1;
        IsPaused = false;

        var tcs = _pauseTCS;
        // Create a new completed TCS BEFORE resolving the old one so the next pause gets a fresh incomplete TCS to block on
        _pauseTCS = new TaskCompletionSource<bool>();
        _pauseTCS.TrySetResult(true);
        tcs.TrySetResult(true); // resume any currently waiting tasks
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
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(ct, GameLifetime.Token);

        await PauseManager.Instance.WaitWhilePausedAsync();

        // Breaks delay into 50ms chunks so pause can interrupt mid-delay instead of pre-delay
        int elapsed = 0;
        const int step = 50;
        while (elapsed < milliseconds)
        {
            await Task.Delay(Mathf.Min(step, milliseconds - elapsed), linked.Token);
            elapsed += step;
            await PauseManager.Instance.WaitWhilePausedAsync();
        }
    }
}