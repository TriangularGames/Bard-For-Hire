using UnityEngine;

public class CoreHUDState : IMenuState
{
    public void EnterState(MenuManager gm)
    {
        Debug.Log("Entered Core HUD State");
    }

    public void ExitState(MenuManager gm)
    {
        Debug.Log("Exiting Core HUD State");
    }

    public void UpdateState(MenuManager gm)
    {
        
    }
}
