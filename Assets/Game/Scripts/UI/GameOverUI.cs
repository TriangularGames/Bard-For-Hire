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
        MenuManager.Instance.SwitchState(new MainMenuState());
        GameManager.Instance.SwitchState(new DefaultGameState());
    }
}
