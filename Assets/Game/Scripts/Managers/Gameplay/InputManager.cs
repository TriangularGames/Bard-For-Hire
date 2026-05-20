using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    public InputSystem_Actions inputActions;

    public override void Awake()
    {
        base.Awake();
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.UI.Enable();
        inputActions.UI.Pause.performed += OnPause;
    }

    private void OnDisable()
    {
        inputActions.UI.Pause.performed -= OnPause;
        inputActions.UI.Disable();
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        // If the game is already paused, we don't want to open the pause menu again or interfere with the current menu state.
        if (PauseManager.Instance.IsPaused) return;

        // Check the current menu state to prevent opening the pause menu in inappropriate contexts (like already being in a menu).
        IMenuState currentState = MenuManager.Instance.GetCurrentState();
        if (currentState == null)
        {
            Debug.LogWarning("No current menu state found. Cannot open pause menu.");
            return;
        }
        if (currentState is PauseMenuState
            || currentState is MainMenuState
            || currentState is OptionsMenuState)
        {
            Debug.Log("Already in a menu state that cant be paused, ignoring pause input.");
            return;
        }

        MenuManager.Instance.SwitchState(new PauseMenuState());
    }

    // Call these from GameManager or states to swap action maps
    public void EnablePlayerActions() => inputActions.Player.Enable();
    public void DisablePlayerActions() => inputActions.Player.Disable();
    public void EnableUIActions() => inputActions.UI.Enable();
    public void DisableUIActions() => inputActions.UI.Disable();
}