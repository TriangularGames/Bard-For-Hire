using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void Play()
    {
        // TODO: change this to prompt player for the tutorial, or just go to the Starter Shop screen
        SceneLoader.Instance.LoadScene("Performance", LoadSceneMode.Single);
        //GameManager.Instance.SwitchState(new NewGameState());
    }

    public void Options()
    {
        MenuManager.Instance.SwitchState(new OptionsMenuState());
    }

    public void Quit()
    {
        Application.Quit();
    }
}
