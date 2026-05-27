using UnityEngine;

public class VictoryState : IGameState
{
    public void EnterState(GameManager gm)
    {
        SceneLoader.Instance.LoadScene("Victory", UnityEngine.SceneManagement.LoadSceneMode.Additive);
    }

    public void ExitState(GameManager gm)
    {
        
    }

    public void UpdateState(GameManager gm)
    {
        
    }
}
