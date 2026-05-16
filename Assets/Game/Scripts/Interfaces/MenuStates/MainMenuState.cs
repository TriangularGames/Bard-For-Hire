using UnityEngine;

public class MainMenuState : IMenuState
{
    public void EnterState(MenuManager gm)
    {
        Debug.Log("Entering MainMenuState");
        SceneLoader.Instance.LoadScene("MainMenu", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void ExitState(MenuManager gm)
    {
        Debug.Log("Exiting Main Menu State");
    }

    public void UpdateState(MenuManager gm)
    {
    }
}
