using UnityEngine;

public class InventoryMenuState : IMenuState
{
    public void EnterState(MenuManager gm)
    {
        Debug.Log("Entering InventoryMenuState");
        EventBus.Publish(new ShowInventoryEvent(true));
    }

    public void ExitState(MenuManager gm)
    {
        Debug.Log("Exiting InventoryMenuState");
        EventBus.Publish(new HideInventoryEvent(false));
    }

    public void UpdateState(MenuManager gm)
    {
    }
    
}
