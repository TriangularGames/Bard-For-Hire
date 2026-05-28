using UnityEngine;

public class DefaultMenuState : IMenuState
{
    public void EnterState(MenuManager gm)
    {
        Debug.Log("Entered Default Menu State");
    }

    public void ExitState(MenuManager gm)
    {
        Debug.Log("Exiting Default Menu State");
    }

    public void UpdateState(MenuManager gm)
    {
        
    }
}
