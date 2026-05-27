using UnityEngine;

public class VictoryMenuState : IMenuState
{
    public void EnterState(MenuManager mm)
    {
        Debug.Log("Entering Victory State");
        EventBus.Publish(new ShowVictoryMenuEvent());
    }

    public void ExitState(MenuManager mm)
    {
        Debug.Log("Exiting Victory State");
        EventBus.Publish(new HideVictoryMenuEvent());
    }

    public void UpdateState(MenuManager mm)
    {
        
    }
}
