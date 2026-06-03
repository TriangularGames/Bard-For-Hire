using UnityEngine;

public class CombatState : IGameState
{
    
    public void EnterState(GameManager gm)
    {
        SceneLoader.Instance.LoadScene("Combat", UnityEngine.SceneManagement.LoadSceneMode.Single);
        CombatManager.Instance.SwitchState(new DefaultCombatState());
    }

    public void ExitState(GameManager gm)
    {

    }

    public void UpdateState(GameManager gm)
    {
        
    }


}

public struct EnterCombatEvent { }