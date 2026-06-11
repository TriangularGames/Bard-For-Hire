using TMPro;
using UnityEngine;

public class EndScreenUI : MonoBehaviour
{
    [SerializeField] private TMP_Text infoDisplaytxt;
    [SerializeField] private Transform upgradeShowcase;

    // subscribe to event for call, when it's called. setup the information using PlayerManager?
    // PlayerManager should have: total money earned variable, highest damage dealt, tracker for most weapon used

    private void Start()
    {
        
    }

    private void SetText()
    {
        infoDisplaytxt.text = "Total Money Earned: " + PlayerManager.Instance.totalMoneyGained +
            "\nHighest Damage Dealth: " + PlayerManager.Instance.highestDamageDealt +
            "\nMost Used Weapon: " + PlayerManager.Instance.mostUsedWeapon +
            "\nUpgrades Held:";
    }

    private void SetShowcase()
    {
        for (int i = 0; i < PlayerManager.Instance.upgradeInventory.Count; i++)
        {
            GameObject obj = AssetManager.Instance.Spawn("UpgradeVictoryDisplay", upgradeShowcase);
            obj.GetComponent<UpgradeVictoryDisplay>().Setup(PlayerManager.Instance.upgradeInventory[i]);
        }
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
