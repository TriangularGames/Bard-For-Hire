using UnityEngine;
public interface IGameState
{
    public void EnterState(GameManager gm);
    public void UpdateState(GameManager gm);
    public void ExitState(GameManager gm);
}

