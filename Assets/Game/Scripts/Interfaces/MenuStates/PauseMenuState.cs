using UnityEngine;

public class PauseMenuState : IMenuState
{
    public void EnterState(MenuManager gm)
    {
        Debug.Log("Entering PauseState");
        EventBus.Publish(new ShowPauseMenuEvent());
    }

    public void ExitState(MenuManager gm)
    {
        Debug.Log("Exiting PauseState");
        EventBus.Publish(new HidePauseMenuEvent());
    }

    public void UpdateState(MenuManager gm)
    {
    }
}
