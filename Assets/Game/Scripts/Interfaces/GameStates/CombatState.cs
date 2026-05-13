using UnityEngine;

public class CombatState : IGameState
{
    public void EnterState(GameManager gm)
    {
        SceneLoader.Instance.LoadScene("CombatTest", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void ExitState(GameManager gm)
    {

    }

    public void UpdateState(GameManager gm)
    {
        
    }
}
