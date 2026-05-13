using UnityEngine;

/// <summary>
/// State for when Game is in the Shop Scene
/// </summary>
public class ShopState : IGameState
{
    public void EnterState(GameManager gm)
    {
        SceneLoader.Instance.LoadScene("ShopTest", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void ExitState(GameManager gm)
    {

    }

    public void UpdateState(GameManager gm)
    {

    }
}
