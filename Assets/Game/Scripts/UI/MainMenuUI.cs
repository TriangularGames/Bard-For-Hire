using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button optionsBtn;
    [SerializeField] private Button quitBtn;
    [SerializeField] private Button creditBtn;
    [SerializeField] private GameObject creditWindow;
    [SerializeField] private Button closeBtn;
    [SerializeField] private GameObject tutorialPrompt;
    [SerializeField] private Button yesBtn;
    [SerializeField] private Button noBtn;

    private void OnEnable()
    {
        startBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        optionsBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        quitBtn.onClick.AddListener(delegate { AudioManager.Instance.Back(); });
        creditBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        closeBtn.onClick.AddListener(delegate { AudioManager.Instance.Error(); });
        yesBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        noBtn.onClick.AddListener(delegate { AudioManager.Instance.Error(); });
    }

    private void OnDisable()
    {
        startBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        optionsBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        quitBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Back(); });
        creditBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        closeBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Error(); });
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

    public void OpenCredits()
    {
        creditWindow.SetActive(true);
    }

    public void CloseCredits()
    {
        creditWindow.SetActive(false);
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
