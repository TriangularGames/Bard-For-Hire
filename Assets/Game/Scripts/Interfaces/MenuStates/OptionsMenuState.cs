using UnityEngine;

public class OptionsMenuState : IMenuState
{
    public void EnterState(MenuManager gm)
    {
        Debug.Log("Entering OptionsMenuState");
        EventBus.Publish(new ShowOptionsMenuEvent());
    }

    public void ExitState(MenuManager gm)
    {
        Debug.Log("Exiting OptionsMenuState");
        EventBus.Publish(new HideOptionsMenuEvent());
    }

    public void UpdateState(MenuManager gm)
    {
    }
}
