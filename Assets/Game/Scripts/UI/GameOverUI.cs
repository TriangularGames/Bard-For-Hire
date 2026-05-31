using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TMP_Text dayDisplay;

    public void SetDayText()
    {
        dayDisplay.text = "Day " + GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().currentDay.ToString();
    }

    public void GoToMainMenu()
    {
        // TODO: change playermanager and enemymanager calls to an event
        PlayerManager.Instance.Reset();
        EnemyManager.Instance.Reset();

        MenuManager.Instance.SwitchState(new MainMenuState());
        GameManager.Instance.SwitchState(new DefaultGameState());
    }
}
