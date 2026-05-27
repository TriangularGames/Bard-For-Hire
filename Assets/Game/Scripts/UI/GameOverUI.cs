using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    public void OnClick_GoToMainMenu()
    {
        MenuManager.Instance.SwitchState(new MainMenuState());
        GameManager.Instance.SwitchState(new DefaultGameState());
    }
}
