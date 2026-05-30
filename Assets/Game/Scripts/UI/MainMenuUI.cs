using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button quitBtn;

    private void OnEnable()
    {
        startBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        optionsBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        quitBtn.onClick.AddListener(delegate { AudioManager.Instance.Back(); });
    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        optionsBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        quitBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Back(); });
    }

    public void Play()
    {
        // TODO: change this to prompt player for the tutorial, or just go to the Starter Shop screen
        GameManager.Instance.SwitchState(new CombatState());
        MenuManager.Instance.SwitchState(new DefaultMenuState());
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
