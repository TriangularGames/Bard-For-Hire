using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Button to go back to Main Menu.")]
    [SerializeField] private Button mainMenuButton;
    [Tooltip("Button to open the Options Menu.")]
    [SerializeField] private Button optionsMenuButton;
    [Tooltip("Button to close the Pause Menu.")]
    [SerializeField] private Button backButton;

    private void Start()
    {
        gameObject.SetActive(false); // Ensure the pause menu is hidden at the start
    }

    public void ReturnToMainMenu()
    {
        MenuManager.Instance.SwitchState(new MainMenuState());
        GameManager.Instance.SwitchState(new DefaultGameState());
    }

    public void OptionsMenu()
    {
        EventBus.Publish(new KeepPauseEvent());
        MenuManager.Instance.SwitchState(new OptionsMenuState());
        PauseManager.Instance.IsPaused = true; // Ensure the game remains paused when opening options
    }

    public void Back()
    {
        MenuManager.Instance.ResumePreviousState();
    }
}

public struct KeepPauseEvent { };