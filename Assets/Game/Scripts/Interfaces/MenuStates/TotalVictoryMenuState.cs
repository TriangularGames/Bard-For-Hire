using UnityEngine;

public class TotalVictoryMenuState : IMenuState
{
    public void EnterState(MenuManager mm)
    {
        Debug.Log("Entering Game Over State");
        EventBus.Publish(new ShowGameOverMenuEvent(false, true));
    }

    public void ExitState(MenuManager mm)
    {
        Debug.Log("Exiting Game Over State");
        EventBus.Publish(new HideGameOverMenuEvent());
    }

    public void UpdateState(MenuManager mm)
    {

    }
}
