using UnityEngine;

public class GameOverMenuState : IMenuState
{
    public void EnterState(MenuManager mm)
    {
        Debug.Log("Entering Game Over State");
        EventBus.Publish(new ShowGameOverMenuEvent(true, false));
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
