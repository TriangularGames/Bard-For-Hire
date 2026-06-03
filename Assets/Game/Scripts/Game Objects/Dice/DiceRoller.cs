using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static UnityEngine.ProBuilder.AutoUnwrapSettings;

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

    public Task<int> RollDie(int modifier = 0)
    {
        var state = new DiceRollState(this, modifier, false);
        CombatManager.Instance.SwitchState(state);
        return state.RollTask;
    }

    // EarlyAdvantage: rolls 2 dice and takes the higher (advantage)
    public Task<int> RollWithAdvantage(int modifier = 0)
    {
        var state = new DiceRollState(this, modifier, true);
        CombatManager.Instance.SwitchState(state);
        return state.RollTask;
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

    #region DiceRollStates
    // All the actual roll methods move to internal — only DiceRollState calls them
    internal async Task<int> ExecuteRollDie(int modifier, CancellationToken ct = default)
    {
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
        await ExecuteShuffleDice(displayRoll, nat, ct);

        if (modifier != 0 || nat == 20 || nat == 1)
        {
            int final = Mathf.Clamp(nat + modifier, 1, 20);
            await ExecuteShowModifier(displayRoll, displayModifer, nat, modifier, final, ct);
            return final;
        }
        return nat;
    }
    internal async Task<int> ExecuteRollWithAdvantage(int modifier, CancellationToken ct = default)
    {
        displayRoll.gameObject.SetActive(true);
        displayAdvantage.gameObject.SetActive(true);

        int a = UnityEngine.Random.Range(1, 21);
        int b = UnityEngine.Random.Range(1, 21);

        Task aShuffle = ExecuteShuffleDice(displayRoll, a, ct);
        Task bShuffle = ExecuteShuffleDice(displayAdvantage, b, ct);
        await Task.WhenAll(aShuffle, bShuffle);

        await PauseExtensions.DelayRespectingPause(Mathf.RoundToInt(revealPause * 1000), ct);

        int lower = a <= b ? a : b;
        int higher = a >= b ? a : b;
        TMP_Text loserDisplay = a <= b ? displayRoll : displayAdvantage;
        TMP_Text winnerDisplay = a >= b ? displayRoll : displayAdvantage;

        loserDisplay.text = $"<color=red>{lower}";

        await PauseExtensions.DelayRespectingPause(600, ct);
        loserDisplay.gameObject.SetActive(false);

        if (modifier != 0 && displayModifer != null)
        {
            int final = Mathf.Clamp(higher + modifier, 1, 20);
            await ExecuteShowModifier(winnerDisplay, displayModifer, higher, modifier, final, ct);
            return final;
        }
        return higher;
    }
    internal async Task ExecuteShuffleDice(TMP_Text target, int landOn, CancellationToken ct = default)
    {
        float timed = 0f;
        float interval = numberChangeyInterval;

        while (timed < changeyDuration)
        {
            // Suspend the shuffle animation loop while paused
            await PauseManager.Instance.WaitWhilePausedAsync();

            target.text = UnityEngine.Random.Range(1, 21).ToString();
            await PauseExtensions.DelayRespectingPause(Mathf.RoundToInt(interval * 1000), ct);
            timed += interval;

            if (timed > changeyDuration * 0.5f)
            {

                interval = Mathf.Lerp(numberChangeyInterval, numberChangeyInterval * 16f, (timed - changeyDuration * 0.6f) / (changeyDuration * 0.4f));
            }
        }
        display.text = "You rolled:";
        target.text = landOn.ToString();
    }
    internal async Task ExecuteShowModifier(TMP_Text main, TMP_Text modDisplay, int nat, int modifier, int final, CancellationToken ct = default)
    {
        if (modifier != 0)
        {
            modDisplay.text = $"+ {modifier}";
            await PauseExtensions.DelayRespectingPause(Mathf.RoundToInt(revealPause * 400), ct);
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
        await PauseExtensions.DelayRespectingPause(Mathf.RoundToInt(revealPause * 800), ct);
        main.text = final.ToString();
        main.color = Color.black;
        displayCrit.text = "";
        modDisplay.text = "";
    }
    internal async Task ShowUpgradeNotif(string message, CancellationToken ct = default)
    {
        if (upgradeNotifText == null) return;
        upgradeNotifText.text = message;
        await PauseExtensions.DelayRespectingPause(Mathf.RoundToInt(800), ct);
        upgradeNotifText.text = "";
    }
    #endregion
}