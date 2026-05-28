using UnityEngine;

public interface IMenuState
{
    public void EnterState(MenuManager gm);
    public void UpdateState(MenuManager gm);
    public void ExitState(MenuManager gm);
}
