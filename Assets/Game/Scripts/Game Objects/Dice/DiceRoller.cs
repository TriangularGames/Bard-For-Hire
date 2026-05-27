using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class DiceRoller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text display;
    [SerializeField] private TMP_Text display2;
    [SerializeField] private TMP_Text modifierDisplay;
    [SerializeField] private Dice diePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform displayPoint;
    [SerializeField] private TMP_Text critText;


    [Header("Timing")]
    [SerializeField] private float maxRollDuration = 5f;
    [SerializeField] private float postSettleDelay = 0.15f;
    [SerializeField] private float diceLerp = 1f;
    [SerializeField] private float diceShowTimer = 3f;
    [SerializeField] private float numberChangeyInterval = 0.005f;
    [SerializeField] private float changeyDuration = 1.4f;
    [SerializeField] private float revealPause = 2f;


    private bool isRolling;
    private float elapsedTime;
    private bool hasRolled;
    private bool shouldRemoveDice = false;
    private Dice diceRef;

    private void Update()
    {
        if (hasRolled && diceRef)
        {
            elapsedTime += Time.deltaTime;
            diceRef.transform.position = Vector3.Lerp(diceRef.transform.position, displayPoint.position, elapsedTime / diceLerp); // reset position for next roll
        }
    }

    public async Task<int> RollDie(int modifier = 0)
    {
        int nat = UnityEngine.Random.Range(1, 21);
        display.gameObject.SetActive(true);
        display2.gameObject.SetActive(false);
        // Natural20: doubles the chance of rolling a 20
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.Natural20))
        {
            int second = UnityEngine.Random.Range(1, 21);
            if (second == 20) nat = 20;
        }

        // WeightedDice: rolls above 10 become 50% more likely
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.WeightedDice))
        {
            int second = UnityEngine.Random.Range(1, 21);
            nat = Mathf.Max(nat, second);
        }

        await ShuffleDice(display, nat);

        if (modifier != 0 || nat == 20 || nat == 1)
        {
            int final = Mathf.Clamp(nat + modifier, 1, 20);
            await ShowModifier(display, modifierDisplay, nat, modifier, final);
            return final;
        }
        return nat;

    }

    private async Task ShuffleDice(TMP_Text target, int landOn)
    {
        float timed = 0f;
        float interval = numberChangeyInterval;

        while (timed < changeyDuration)
        {
            target.text = UnityEngine.Random.Range(1, 21).ToString();
            await Task.Delay(Mathf.RoundToInt(interval * 1000));
            timed += interval;

            if (timed > changeyDuration * 0.5f) {

                interval = Mathf.Lerp(numberChangeyInterval, numberChangeyInterval * 16f , (timed-changeyDuration * 0.6f) / (changeyDuration * 0.4f));
            }
        }
        target.text = landOn.ToString();
    }

    // EarlyAdvantage: rolls 2 dice and takes the higher (advantage)
    public async Task<int> RollWithAdvantage(int modifier = 0)
    {
        display.gameObject.SetActive(true);
        display2.gameObject.SetActive(true);

        int a = UnityEngine.Random.Range(1, 21);
        int b = UnityEngine.Random.Range(1, 21);
        if (display2 != null) display2.gameObject.SetActive(true);

        Task aShuffle = ShuffleDice(display, a);
        Task bShuffle = ShuffleDice(display2, b);
        await Task.WhenAll(aShuffle, bShuffle);

        await Task.Delay(Mathf.RoundToInt(revealPause * 1000));

        int lower = a <= b ? a : b;
        int higher = a >= b ? a : b;
        TMP_Text loserDisplay = a <= b ? display : display2;
        TMP_Text winnerDisplay = a >= b ? display : display2;

        loserDisplay.text = $"<color=red>{lower}";

        await Task.Delay(600);
        loserDisplay.gameObject.SetActive(false);

        if (modifier != 0 && modifierDisplay != null)
        {
            int final = Mathf.Clamp(higher + modifier, 1, 20);
            await ShowModifier(winnerDisplay, modifierDisplay, higher, modifier, final);
            return final;
        }

        return higher;
    }
    private async Task ShowModifier(TMP_Text main, TMP_Text modDisplay, int nat, int modifier, int final)
    {
        if (modifier != 0)
        {
            modDisplay.text = $"+ {modifier}";
            await Task.Delay(Mathf.RoundToInt(revealPause * 400));
        }

        if (nat == 20)
        {
            main.color = Color.yellow;
            critText.text = "CRITICAL HIT!";
            critText.color = Color.yellow;
        }
        else if (nat == 1)
        {
            main.color = Color.red;
            critText.text = "CRITICAL MISS!";
            critText.color = Color.red;
        }
        else
        {
            main.color = Color.black;
            critText.text = "";
        }
        await Task.Delay(Mathf.RoundToInt(revealPause * 800));
        main.text = final.ToString();
        main.color = Color.black;
        critText.text = "";
        modDisplay.text = "";
    }

    public void ResetText()
    {
        display.text = "";
        if (display2 != null) display2.text = "";
        if (modifierDisplay != null) modifierDisplay.text = "";
        if (critText != null) critText.text = "";
    }
}