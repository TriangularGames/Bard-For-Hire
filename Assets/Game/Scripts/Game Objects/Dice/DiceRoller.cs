using System;
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
    [SerializeField] private float revealPause = 3f;

    private enum State
    {
        Idle,
        Shuffling,
        AdvantageRevealPause,
        AdvantageStrikeWait,
        CritPause,
        ModifierPause,
        FinalPause,
        Done
    }
    private State state = State.Idle;
    private float timer;

    [Header("Roll Data")]
    private int nat;
    private int natB;
    private int mod;
    private bool isAdvantage;
    private int higherRoll;
    private int lowerRoll;
    public int natRoll;

    [Header("Roll Data")]
    private TMP_Text winnerDisplay;
    private TMP_Text loserDisplay;
    private TMP_Text GeneralDisplay() => isAdvantage ? winnerDisplay : displayRoll;

    [Header("Timers")]
    private float shuffleTimeElapsed;
    private float intervalTimer;
    private float currentInterval;
    private float notifTimer;

    private Action<int> onDone;

    public void Update()
    {
        if (PauseManager.Instance.IsPaused) return;

        ProgressNotif();

        switch (state)
        {
            case State.Shuffling: ProgressShuffle(); break;
            case State.AdvantageRevealPause: ProgressAdvantageReveal(); break;
            case State.FinalPause: ProgressFinalPause(); break;
            case State.AdvantageStrikeWait: ProgressAdvantageStrike(); break;
            case State.CritPause: ProgressCritPause(); break;
            case State.ModifierPause: ProgressModifierPause(); break;


        }
    }
    // Function for rolling the d20 then applying upgrades, leading to the dice anim
    public void RollDie(int modifier, Action<int> call)
    {
        mod = modifier;
        onDone = call;
        isAdvantage = false;
        nat = UnityEngine.Random.Range(1, 21);
        ApplyUpgradeEffect();
        natRoll = nat;
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.Ace) && nat == 1)
        {
            nat = 20;
            natRoll = 20;
        }
        display.gameObject.SetActive(true);
        displayRoll.gameObject.SetActive(true);
        displayAdvantage.gameObject.SetActive(false);

        display.text = "Rolling die...";
        AudioManager.Instance.PlayClip("DieRoll");
        ShuffleDice();
    }

    // Shows the notification for the upgrades upon activation
    public void ShowUpgradeNotif(string message)
    {
        if (upgradeNotifText == null) return;
        upgradeNotifText.text = message;
        notifTimer = 0.8f;
    }

    // Sets up and starts the dice animation
    private void ShuffleDice()
    {
        shuffleTimeElapsed = 0f;
        currentInterval = numberChangeyInterval;
        intervalTimer = 0f;
        state = State.Shuffling;
    }

    // Animation to make the dice switch numbers before eventually slwoing down and landing on a number
    private void ProgressShuffle()
    {
        shuffleTimeElapsed += Time.deltaTime;
        intervalTimer -= Time.deltaTime;

        if (intervalTimer <= 0f)
        {
            displayRoll.text = UnityEngine.Random.Range(1, 21).ToString();
            if (isAdvantage)
                displayAdvantage.text = UnityEngine.Random.Range(1, 21).ToString();

            if (shuffleTimeElapsed > changeyDuration * 0.5f)
            {
                float timeToSlow = Mathf.Clamp01((shuffleTimeElapsed - changeyDuration * 0.6f) / (changeyDuration * 0.4f));
                currentInterval = Mathf.Lerp(numberChangeyInterval, numberChangeyInterval * 40f, timeToSlow);
            }

            intervalTimer = currentInterval;
        }

        if (shuffleTimeElapsed >= changeyDuration)
            StopShuffle();
    }

    // Ends the shuffle state and presents the roll results
    private void StopShuffle()
    {
        display.text = "You rolled:";
        displayRoll.text = nat.ToString();

        if(isAdvantage)
        {
            displayAdvantage.text = natB.ToString();
            state = State.AdvantageRevealPause;
            timer = revealPause;
        }
        else
        {
            RevealDice(nat);
        }

    }

    // EarlyAdvantage: rolls 2 dice and takes the higher (advantage)
    public void RollWithAdvantage(int modifier, Action<int> call)
    {
        mod = modifier;
        onDone = call;
        isAdvantage = true;

        nat = UnityEngine.Random.Range(1, 21);
        natB = UnityEngine.Random.Range(1, 21);

        higherRoll = Mathf.Max(nat, natB);
        lowerRoll = Mathf.Min(nat, natB);

        winnerDisplay = nat >= natB ? displayRoll : displayAdvantage;
        loserDisplay = nat < natB ? displayRoll : displayAdvantage;

        natRoll = higherRoll;

        displayRoll.gameObject.SetActive(true);
        displayAdvantage.gameObject.SetActive(true);

        display.text = "Rolling die...";
        AudioManager.Instance.PlayClip("DieRoll");
        ShuffleDice();
    }
    // Reds out the lesser roll to present it as the non-successful one
    private void ProgressAdvantageReveal()
    {
        timer-= Time.deltaTime;
        if (timer > 0) return;
        loserDisplay.text = $"<color=red>{lowerRoll}</color>";
        state = State.AdvantageStrikeWait;
        timer = 0.6f;
    }
    // Removes the lower number and prepares to present the higher one
    private void ProgressAdvantageStrike()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        loserDisplay.gameObject.SetActive(false);
        RevealDice(higherRoll);
    }

    // Reveals the dice result to the player fully
    private void RevealDice(int roll)
    {
        // If roll is naturally a 20 you get a critical hit which does double damage
        if (roll == 20)
        {
            GeneralDisplay().color = Color.yellow;
            displayCrit.text = "CRITICAL HIT!";
            displayCrit.color = Color.yellow;
            state = State.CritPause;
            timer = revealPause * 0.8f;
            return;
        }

        // If roll is naturally a 1 you get a critical miss which ignores all modifiers
        if (roll == 1)
        {
            GeneralDisplay().color = Color.red;
            displayCrit.text = "CRITICAL Fail!";
            displayCrit.color = Color.red;
            state = State.CritPause;
            timer = revealPause * 0.8f;
            return;
        }

        // Starts displaying the modifiers if there are any
        if (mod != 0)
        {
            if (mod > 0)
                displayModifer.text = $"+ {mod}";
            if (mod < 0)
                displayModifer.text = $"{mod}";
            state = State.ModifierPause;
            timer = revealPause * 0.6f;
            return;
        }

        SendBack();
    }

    // Progresses after the crit result
    private void ProgressCritPause()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        GeneralDisplay().color = Color.white;
        displayCrit.text = "";
        SendBack();
    }

    // Progresses after the modifiers
    private void ProgressModifierPause()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        state = State.FinalPause;
        timer = revealPause * 0.6f;
    }
    // Final pause for the dice section
    private void ProgressFinalPause()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        GeneralDisplay().text = FinalResult().ToString();
        GeneralDisplay().color = Color.white;
        displayCrit.text = "";
        if (displayModifer != null) displayModifer.text = "";
        SendBack();
    }

    // Progresses the upgrade notification text with the timer
    private void ProgressNotif()
    {
        if (notifTimer <= 0f) return;
        notifTimer -= Time.deltaTime;
        if (notifTimer <= 0f && upgradeNotifText != null)
            upgradeNotifText.text = "";
    }

    // The final result used for presentation
    private int FinalResult()
    {
        int roll = isAdvantage ? higherRoll : nat;
        if (roll == 20 || roll == 1) return roll;
        return mod != 0 ? roll + mod : roll;
    }

    // Resets all the text used for the details of this roll
    public void ResetText()
    {
        state = State.Idle;
        display.text = "";
        displayRoll.text = "";
        if (displayAdvantage != null) displayAdvantage.text = "";
        if (displayModifer != null) displayModifer.text = "";
        if (displayCrit != null) displayCrit.text = "";
        if (upgradeNotifText != null) upgradeNotifText.text = "";
    }

    // Progresses after the crit result
    private void SendBack()
    {
        // Mark the roller as finished so update stops processing
        state = State.Done;

        // Calculate the final roll value
        int result = FinalResult();

        // Cache the call before it goes through
        var send = onDone;

        // Clear the reference so the script is ready to go!
        onDone = null;

        // Invoke the call that was cached
        send?.Invoke(result);
    }

    // Applies the effects of upgrades that impact dice probability
    public void ApplyUpgradeEffect()
    {
        // WeightedDice: rolls above 10 become 50% more likely
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.WeightedDice))
        {
            if (nat <= 10 && UnityEngine.Random.value < 0.3f)
            {
                int second = UnityEngine.Random.Range(1, 21);
                nat = Mathf.Max(nat, second);
            }
        }

        // Natural20: doubles the chance of rolling a 20
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.Natural20))
        {
            int second = UnityEngine.Random.Range(1, 21);
            if (second == 20) nat = 20;
        }

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
    }
}