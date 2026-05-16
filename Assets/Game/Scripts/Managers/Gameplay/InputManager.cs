using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private InputSystem_Actions inputActions;

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
        if (PauseManager.Instance.IsPaused) return;

        IMenuState currentState = MenuManager.Instance.GetCurrentState();

        // Only allow pausing if current state is NOT MainMenuState
        if (currentState is IMenuState menuState && menuState is not MainMenuState)
        {
            MenuManager.Instance.SwitchState(new PauseMenuState());
        }
    }

    // Call these from GameManager or states to swap action maps
    public void EnablePlayerActions() => inputActions.Player.Enable();
    public void DisablePlayerActions() => inputActions.Player.Disable();
    public void EnableUIActions() => inputActions.UI.Enable();
    public void DisableUIActions() => inputActions.UI.Disable();
}