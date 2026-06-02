using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text display;
    [SerializeField] private TMP_Text displayRoll;
    [SerializeField] private TMP_Text displayAdvantage;
    [SerializeField] private TMP_Text displayModifer;
    [SerializeField] private TMP_Text displayCrit;
    [SerializeField] private TMP_Text upgradeNotifText;

    [Header("Timing")]
    [SerializeField] private float numberChangeyInterval = 0.005f;
    [SerializeField] private float changeyDuration = 1.4f;
    [SerializeField] private float revealPause = 2f;

    public async Task<int> RollDie(int modifier = 0)
    {
        CombatManager.Instance.SwitchState(new DiceRollState());
        int nat = UnityEngine.Random.Range(1, 21);
        display.gameObject.SetActive(true);
        displayRoll.gameObject.SetActive(true);
        displayAdvantage.gameObject.SetActive(false);

        // WeightedDice: rolls above 10 become 50% more likely
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.WeightedDice))
        {
            if (nat <= 10 && Random.value < 0.3f)
            {
                int second = Random.Range(1, 21);
                nat = Mathf.Max(nat, second);
            }
        }

        // Natural20: doubles the chance of rolling a 20
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.Natural20))
        {
            int second = UnityEngine.Random.Range(1, 21);
            if (second == 20) nat = 20;
        }

        display.text = "Rolling die...";
        AudioManager.Instance.PlayClip("DieRoll");
        await ShuffleDice(displayRoll, nat);

        if (modifier != 0 || nat == 20 || nat == 1)
        {
            int final = Mathf.Clamp(nat + modifier, 1, 20);
            await ShowModifier(displayRoll, displayModifer, nat, modifier, final);
            return final;
        }
        return nat;

    }

    public async Task ShowUpgradeNotif(string message)
    {
        if (upgradeNotifText == null) return;
        upgradeNotifText.text = message;
        await Task.Delay(800);
        upgradeNotifText.text = "";
    }

    private async Task ShuffleDice(TMP_Text target, int landOn)
    {
        float timed = 0f;
        float interval = numberChangeyInterval;

        while (timed < changeyDuration)
        {
            // Suspend the shuffle animation loop while paused
            await PauseManager.Instance.WaitWhilePausedAsync();

            target.text = UnityEngine.Random.Range(1, 21).ToString();
            await PauseExtensions.DelayRespectingPause(Mathf.RoundToInt(interval * 1000));
            timed += interval;

            if (timed > changeyDuration * 0.5f) {

                interval = Mathf.Lerp(numberChangeyInterval, numberChangeyInterval * 16f , (timed-changeyDuration * 0.6f) / (changeyDuration * 0.4f));
            }
        }
        display.text = "You rolled:";
        target.text = landOn.ToString();
    }

    // EarlyAdvantage: rolls 2 dice and takes the higher (advantage)
    public async Task<int> RollWithAdvantage(int modifier = 0)
    {
        displayRoll.gameObject.SetActive(true);
        displayAdvantage.gameObject.SetActive(true);

        int a = UnityEngine.Random.Range(1, 21);
        int b = UnityEngine.Random.Range(1, 21);
        if (displayAdvantage != null) displayAdvantage.gameObject.SetActive(true);

        Task aShuffle = ShuffleDice(displayRoll, a);
        Task bShuffle = ShuffleDice(displayAdvantage, b);
        await Task.WhenAll(aShuffle, bShuffle);

        // Pause-aware delay after both dice land
        await PauseExtensions.DelayRespectingPause(Mathf.RoundToInt(revealPause * 1000));

        int lower = a <= b ? a : b;
        int higher = a >= b ? a : b;
        TMP_Text loserDisplay = a <= b ? displayRoll : displayAdvantage;
        TMP_Text winnerDisplay = a >= b ? displayRoll : displayAdvantage;

        loserDisplay.text = $"<color=red>{lower}";

        await PauseExtensions.DelayRespectingPause(600);
        loserDisplay.gameObject.SetActive(false);

        if (modifier != 0 && displayModifer != null)
        {
            int final = Mathf.Clamp(higher + modifier, 1, 20);
            await ShowModifier(winnerDisplay, displayModifer, higher, modifier, final);
            return final;
        }

        return higher;
    }
    private async Task ShowModifier(TMP_Text main, TMP_Text modDisplay, int nat, int modifier, int final)
    {
        if (modifier != 0)
        {
            modDisplay.text = $"+ {modifier}";
            await PauseExtensions.DelayRespectingPause(Mathf.RoundToInt(revealPause * 400));
        }

        if (nat == 20)
        {
            main.color = Color.yellow;
            displayCrit.text = "CRITICAL HIT!";
            displayCrit.color = Color.yellow;
        }
        else if (nat == 1)
        {
            main.color = Color.red;
            displayCrit.text = "CRITICAL MISS!";
            displayCrit.color = Color.red;
        }
        else
        {
            main.color = Color.white;
            displayCrit.text = "";
        }
        await PauseExtensions.DelayRespectingPause(Mathf.RoundToInt(revealPause * 800));
        main.text = final.ToString();
        main.color = Color.black;
        displayCrit.text = "";
        modDisplay.text = "";
    }

    public void ResetText()
    {
        display.text = "";
        displayRoll.text = "";
        if (displayAdvantage != null) displayAdvantage.text = "";
        if (displayModifer != null) displayModifer.text = "";
        if (displayCrit != null) displayCrit.text = "";
        if (upgradeNotifText != null) upgradeNotifText.text = "";
    }
}