using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private GameObject tutorialPrompt;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    private void OnEnable()
    {
        startBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        optionsBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        quitBtn.onClick.AddListener(delegate { AudioManager.Instance.Back(); });
        yesBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        noBtn.onClick.AddListener(delegate { AudioManager.Instance.Error(); });
    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        optionsBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        quitBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Back(); });
        yesBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        noBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Error(); });
    }

    private void Awake()
    {
        tutorialPrompt.SetActive(false);
    }

    public void Play()
    {
        tutorialPrompt.SetActive(true);
    }

    public void Options()
    {
        MenuManager.Instance.SwitchState(new OptionsMenuState());
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Yes()
    {
        PlayerPrefs.SetInt("TutorialComplete", 0);
        PlayerPrefs.Save();
        StartGame();
    }

    public void No()
    {
        PlayerPrefs.SetInt("TutorialComplete", 1);
        PlayerPrefs.Save();
        StartGame();
    }

    public void StartGame()
    {
        GameManager.Instance.SwitchState(new CombatState());
        MenuManager.Instance.SwitchState(new DefaultMenuState());
    }
}
