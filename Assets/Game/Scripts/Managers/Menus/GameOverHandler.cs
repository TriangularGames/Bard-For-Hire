using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    [Tooltip("Game over menu UI.")]
    [SerializeField] private EndScreenUI gameOverUI;

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
    private void OnShowGameOverMenu(ShowGameOverMenuEvent eventData)
    {
        // Code to show the Pause Menu
        ToggleGameOverMenuVisibility(true);
        if (eventData.GameOver)
        {
            gameOverUI.SetGameOver();
        }

        if (eventData.GameWon)
        {
            gameOverUI.SetGameWon();
        }

        gameOverUI.SetText();
        gameOverUI.SetShowcase();
        gameOverUI.ButtonDisplay();


        // Game is paused!
        Time.timeScale = 0;
    }

    /// <summary>
    /// Handles the event to hide the Pause Menu.
    /// </summary>
    /// <param name="eventData">The event data associated with hiding the pause menu. Currently unused but can be extended for future use.</param>
    private void OnHideGameOverMenu(HideGameOverMenuEvent eventData)
    {
        // Code to hide the Pause Menu
        ToggleGameOverMenuVisibility(false);
        gameOverUI.Clear();

        // Game is unpaused!
        Time.timeScale = OptionsManager.Instance.GetTimeScale();
    }
    /// <summary>
    /// Subscribes to necessary events on the event bus.
    /// </summary>
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ShowGameOverMenuEvent>(OnShowGameOverMenu);
        EventBus.Subscribe<HideGameOverMenuEvent>(OnHideGameOverMenu);
    }

    /// <summary>
    /// Unsubscribes from events on the event bus.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<ShowGameOverMenuEvent>(OnShowGameOverMenu);
        EventBus.Unsubscribe<HideGameOverMenuEvent>(OnHideGameOverMenu);
    }

    /// <summary>
    /// Toggles the visibility of the victory menu UI.
    /// </summary>
    /// <param name="isVisible">Whether the victory menu should be visible.</param>
    public void ToggleGameOverMenuVisibility(bool isVisible)
    {
        gameOverUI?.gameObject.SetActive(isVisible);
    }
}

public struct ShowGameOverMenuEvent
{
    public bool GameOver;
    public bool GameWon;

    public ShowGameOverMenuEvent(bool _GameOver, bool _GameWon)
    {
        GameOver = _GameOver;
        GameWon = _GameWon;
    }

}

public struct HideGameOverMenuEvent { }