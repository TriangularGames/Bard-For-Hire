using TMPro;
using UnityEngine;

public class EndScreenUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleTxt;
    [SerializeField] private TMP_Text infoDisplayTxt;
    [SerializeField] private Transform upgradeShowcase;

    [SerializeField] private GameObject mainMenuBtn;
    [SerializeField] private GameObject continueBtn;

    // If Game is Over (A Loss)
    private bool GameOver = false;

    // If Game is Won (3rd Boss is defeated)
    private bool GameWon = false;

    public void SetGameOver()
    {
        GameOver = true;
    }

    public void SetGameWon()
    {
        GameWon = true;
    }

    public void SetText()
    {
        if (GameOver)
        {
            titleTxt.text = "Battle Lost";
        }
        if (GameWon)
        {
            titleTxt.text = "Total Victory!";
        }

        infoDisplayTxt.text = "Day " + GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().currentDay.ToString() +
            "\nTotal Money Earned: " + PlayerManager.Instance.totalMoneyGained +
            "\nHighest Damage Dealt: " + PlayerManager.Instance.highestDamageDealt +
            "\nMost Used Weapon: " + PlayerManager.Instance.GetMostUsedWeapon() +
            "\nUpgrades Held:";
    }

    public void SetShowcase()
    {
        for (int i = 0; i < PlayerManager.Instance.upgradeInventory.Count; i++)
        {
            GameObject obj = AssetManager.Instance.Spawn("UpgradeVictoryDisplay", upgradeShowcase);
            obj.GetComponent<UpgradeVictoryDisplay>().Setup(PlayerManager.Instance.upgradeInventory[i]);
        }
    }

    public void ButtonDisplay()
    {
        if (GameOver)
        {
            mainMenuBtn.SetActive(true);
            continueBtn.SetActive(false);
        }

        if (GameWon)
        {
            mainMenuBtn.SetActive(true);
            continueBtn.SetActive(true);
        }
    }

    public void Clear()
    {
        infoDisplayTxt.text = string.Empty;
        foreach (Transform child in upgradeShowcase.transform)
        {
            Destroy(child.gameObject);
        }
        GameOver = false;
        GameWon = false;
    }

    public void GoToShop()
    {
        GameManager.Instance.SwitchState(new ShopState());
        MenuManager.Instance.SwitchState(new DefaultMenuState());
    }

    public void GoToMainMenu()
    {
        // Reset game to default values on quit
        EventBus.Publish(new ResetGameEvent());

        MenuManager.Instance.SwitchState(new MainMenuState());
        GameManager.Instance.SwitchState(new DefaultGameState());
    }
}
