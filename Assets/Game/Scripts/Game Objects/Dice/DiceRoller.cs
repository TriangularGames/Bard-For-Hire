using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] public TMP_Text display;
    [SerializeField] public TMP_Text displayRoll;
    [SerializeField] public TMP_Text displayAdvantage;
    [SerializeField] public TMP_Text displayModifier;
    [SerializeField] public TMP_Text displayCrit;
    [SerializeField] public TMP_Text upgradeNotifText;

    [Header("Timing")]
    [SerializeField] public float numberChangeyInterval = 0.005f;
    [SerializeField] public float changeyDuration = 1.4f;
    [SerializeField] public float revealPause = 2f;
    public int RollNat()
    {
        int nat = Random.Range(1, 21);

        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.Gambler))
        {
            if (nat > 3 && nat < 18)
            {
                int second = UnityEngine.Random.Range(1, 21);
                int distA = Mathf.Min(nat, 21 - nat);
                int distB = Mathf.Min(second, 21 - second);
                if (distB < distA) nat = second;
            }
        }

        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.Ace) && nat == 1)
        {
            nat = 20;
        }

        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.WeightedDice))
        {
            if (nat <= 10 && Random.value < 0.3f)
            {
                nat = Mathf.Max(nat, Random.Range(1, 21));
            }
        }

        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.Natural20))
        {
            int second = Random.Range(1, 21);
            if (second == 20) nat = 20;
        }
    

        return nat;
    }

    public (int a, int b, int chosen) RollAdvantage()
    {
        int a = Random.Range(1, 21);
        int b = Random.Range(1, 21);
        return (a, b, Mathf.Max(a, b));
    }

    public void ResetText()
    {
        display.text = "";
        displayRoll.text = "";
        if (displayAdvantage != null) displayAdvantage.text = "";
        if (displayModifier != null) displayModifier.text = "";
        if (displayCrit != null) displayCrit.text = "";
        if (upgradeNotifText != null) upgradeNotifText.text = "";
    }

    // Determines if this is a normal roll or an early advantage roll, then starts the appropriate state
    public void StartCombatRoll(List<ItemData> items, int index)
    {
        bool withAdvantage = (index == 0 && UpgradeManager.Instance.HasUpgrade(UpgradeID.EarlyAdvantage)) || UpgradeFightingManager.Instance.shadowThiefActive || UpgradeFightingManager.Instance.UseComeback();

        CombatManager.Instance.SwitchState(new RollState(items, this, index, withAdvantage));
    }
}