using UnityEngine;

public class DefaultGameState : IGameState
{
    public void EnterState(GameManager gm)
    {
        Debug.Log("Entering default game state");
    }

    public void ExitState(GameManager gm)
    {
        Debug.Log("Exiting default game state");
    }

    public void UpdateState(GameManager gm)
    {
        
    }
}
