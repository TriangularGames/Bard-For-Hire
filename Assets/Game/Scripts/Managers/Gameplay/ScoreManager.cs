using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// ScoreManager calculates the score based on the Items added to AttackHand
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("Inspector Reference")]
    [SerializeField] GameObject itemDisplay;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text disabledTypeText;

    [Header("Game Config")]
    public int curRound = 1;
    private int MaxRounds = 3;
    public int GameSpeed = 4;

    [Header("Dice and Items")]
    public float waitForRoll = 2f;
    public DiceRoller roller;
    private List<ItemData> pendingItems;
    private int curItem = -1;

    private string rewardDisplayText;

    // States
    private enum State
    {
        Idle,
        SetupItem,
        WaitForRoll,
        HitDelay,
        BonusPreWait,
        BonusRevealWait,
        AttackDelay,
        PostActionDelay,
        MissDelay,
        NotifWait,
        PostMissDelay,
        BonusAttackNotifWait,
        BonusAttackDelay
    }
    private State state = State.Idle; // Default at idle state
    private float timer;

    private enum NotifNext 
    { 
        SecondChance, 
        QuickSave 
    }
    private NotifNext notifNext;

    [Header("Roll Result")]
    private int finalRoll;
    private int totalDamage;

    [Header("Bonus Display")]
    private List<UpgradeFightingManager.DamageBonus> bonuses; // Bonuses for your damage
    private int bonusIndex;
    private int displayedDamage;

    [Header("Controllers")]
    private ItemController currentItemController;
    private ItemDisplayController currentDisplayController;

    [Header("Bonus Attacks")]
    private Queue<BonusAttack> bonusAttackQueue = new Queue<BonusAttack>(); // Queue up bonus attacks
    private BonusAttack currentBonusAttack;
    private struct BonusAttack // Data for the bonus attack used by some upgrades
    {
        public string upgradeName;
        public int damage;
    }


    private void OnEnable()
    {
        EventBus.Subscribe<MoneyEarnedEvent>(MakeRewardText);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<MoneyEarnedEvent>(MakeRewardText);
    }

    // Sets up reward text with justification
    private void MakeRewardText(MoneyEarnedEvent e) 
    {
        rewardDisplayText += e.reason + " : " + e.coinAmount + "\n";
    }

    private void Start()
    {
        rewardDisplayText = "";
        //combatCompleteText.text = "";
        itemDisplay.SetActive(false);
        EventBus.Publish<EnterCombatEvent>(new EnterCombatEvent());
        SetGameSpeed();
    }

    private void SetGameSpeed()
    {
        GameSpeed = PlayerPrefs.GetInt("GameSpeed");
        switch (GameSpeed)
        {
            case 0:
                GameSpeed = 4;
                break;
            case 1:
                GameSpeed = 3;
                break;
            case 2:
                GameSpeed = 2;
                break;
            case 3:
                GameSpeed = 1;
                break;
        }
    }

    private void Update()
    {
        // Idle state do nothing
        if (state == State.Idle) return;
        if (PauseManager.Instance.IsPaused) return;

        // State machine containing different states to be switched between
        switch (state)
        {
            case State.SetupItem: SetupItems(); break;
            case State.WaitForRoll: break;
            case State.HitDelay: Tick(StartBonusDisplay); break;
            case State.BonusPreWait: Tick(ShowCurrentBonus); break;
            case State.BonusRevealWait: Tick(AdvanceBonus); break;
            case State.AttackDelay: Tick(DoAttacks); break;
            case State.PostActionDelay: Tick(GoToNextItem); break;
            case State.MissDelay: Tick(DoMissProcess); break;
            case State.PostMissDelay: Tick(GoToNextItem); break;
            case State.NotifWait: Tick(DoNotif); break;
            case State.BonusAttackNotifWait: Tick(StartBonusAttack); break;
            case State.BonusAttackDelay: Tick(ProcessNextBonus); break;
        }
    }

    // this helps every timed state be one line I saw IDK
    private void Tick(System.Action onExpire)
    {
        timer -= Time.deltaTime;
        if (timer <= 0f) onExpire();
    }

    // Change between the states
    private void GoTo(State next, float delay)
    {
        state = next;
        timer = delay;
    }


    /// <summary>
    /// Calculates the final score for a round using ItemData List
    /// </summary>
    /// <param name="items">List of Items to be scored</param>
    public Task CalculateScore(List<ItemData> items)
    {
        EventBus.Publish<ScoringStartedEvent>(new ScoringStartedEvent());
        if (items == null || items.Count == 0) return Task.CompletedTask;
        pendingItems = items;
        curItem = -1;
        UpgradeFightingManager.Instance.StartRound();
        UpgradeFightingManager.Instance.GetTheHandBonuses(pendingItems);
        GoToNextItem();
        return Task.CompletedTask;

    }

    // Sets up subsequent items if there's more to be played
    private void GoToNextItem() {

        curItem++;

        if (!EnemyManager.Instance.AreEnemiesAlive() || curItem >= pendingItems.Count)
        {
            FinalizeScore();
            return;
        }

        state = State.SetupItem;
    }

    // Sets up the current item and fetches info before starting it's roll check
    private void SetupItems()
    {
        ItemData item = pendingItems[curItem];

        currentItemController = itemDisplay.GetComponent<ItemController>();
        currentDisplayController = itemDisplay.GetComponent<ItemDisplayController>();

        currentItemController.itemData = item;
        currentItemController.Setup();
        currentDisplayController.Reset();
        itemDisplay.SetActive(true);

        EventBus.Publish(new ItemUsedEvent(item));
        BeginRoll();
    }

    // Checks for advantage or modifiers (Comeback too) to decide how DiceRoller will work
    public void BeginRoll()
    {
        ItemData item = pendingItems[curItem];
        int mod = FindModifier(item);
        bool adv = ShouldRollAdvantage();

        if (adv)
        {
            if (UpgradeFightingManager.Instance.UseComeback())
            {
                roller.ShowUpgradeNotif("Comeback!");
                UpgradeFightingManager.Instance.UsingComeback();
            }
            roller.RollWithAdvantage(mod, OnRollComplete);
        }
        else
        {
            roller.RollDie(mod, OnRollComplete);
        }

        state = State.WaitForRoll;
    }


    // Checks upgrades for roll modifiers
    private int FindModifier(ItemData item)
    {
        int mod = 0;
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.SkillProficiency)) 
            mod += 2;
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.GreatWeaponMaster))
            mod += Mathf.RoundToInt(item.Playable * 0.25f);
        if (UpgradeFightingManager.Instance.tempDCReduce > 0)
            mod += UpgradeFightingManager.Instance.tempDCReduce;
        if (UpgradeFightingManager.Instance.shadowThiefActive) mod -= 2;
        return mod;
    }

    // Check to see if circumstances apply for advantage
    private bool ShouldRollAdvantage()
    {
        if ((UpgradeManager.Instance.HasUpgrade(UpgradeID.EarlyAdvantage) && curItem == 0) || UpgradeFightingManager.Instance.shadowThiefActive || UpgradeFightingManager.Instance.UseComeback())
            return true;
        return false;
    }

    /// <summary>
    /// Called when the roll is complete, and the score is calculated
    /// </summary>
    /// <param name="rollValue">The value of the roll</param>
    private void OnRollComplete(int rollValue)
{
    UpgradeFightingManager.Instance.rolledNat20 = (roller.natRoll == 20);
    ItemData item = pendingItems[curItem];
    finalRoll = UpgradeFightingManager.Instance.GetBonusRoll(rollValue);

    // Include in here some effect thats displayed as each weapon is determined
    // to be scored or not
    if (item.Playable <= finalRoll)
    {
        Debug.Log($"{item.name} was played!");
        AudioManager.Instance.PlayClip("Success");
        currentDisplayController.Success();
        totalDamage = UpgradeFightingManager.Instance.GetBonusDamage(item, curItem, out bonuses);
        totalDamage = UpgradeFightingManager.Instance.ApplyHandBonuses(totalDamage);
        GoTo(State.HitDelay, 0.3f * GameSpeed);
    }
    else
    {
        AudioManager.Instance.PlayClip("Fail");
        currentDisplayController.Fail();
        EventBus.Publish<MissEvent>(new MissEvent());
        GoTo(State.MissDelay, 0.3f * GameSpeed);

    }
}
    // Displays the bonuses applied to weapons from upgrades/consumables/crits
    private void StartBonusDisplay()
    {
        displayedDamage = pendingItems[curItem].Damage;
        bonusIndex = 0;
        currentItemController.SetDamageTxtRaw(displayedDamage);
        NoBonusesWeGot();
        if (bonusIndex >= bonuses.Count) 
        {
            GiveHit(); 
            return;  
        }
        GoTo(State.BonusPreWait, 0.4f);
    }

    // We don't get any actual bonuses out of it
    private void NoBonusesWeGot()
    {
        while (bonusIndex < bonuses.Count && bonuses[bonusIndex].amount == 0)
            bonusIndex++;
    }

    // Publishes hit action and starts to work the attack
    private void GiveHit()
    {
        UpgradeFightingManager.Instance.SuccessfulAction(pendingItems[curItem], totalDamage);
        EventBus.Publish(new HitEvent());
        GoTo(State.AttackDelay, 0.1f * GameSpeed);
    }

    // Shows player the current presented bonus and where it's from
    private void ShowCurrentBonus()
    {
        if (bonusIndex >= bonuses.Count) 
        { 
            GiveHit(); 
            return; 
        }

        var bonus = bonuses[bonusIndex];
        currentItemController.ShowBonusLabel(displayedDamage, bonus.amount, bonus.source);
        GoTo(State.BonusRevealWait, 0.7f);
    }

    // Advance to the next bonus
    private void AdvanceBonus()
    {
        displayedDamage += bonuses[bonusIndex].amount;
        bonusIndex++;
        currentItemController.SetDamageTxtRaw(displayedDamage);
        NoBonusesWeGot();

        if (bonusIndex >= bonuses.Count)
        {
            GiveHit();
            return;
        }

        GoTo(State.BonusPreWait, 0.4f);
    }

    // Call the AttackEnemy method
    private void DoAttacks()
    {
        ItemData item = pendingItems[curItem];
        AttackEnemy(item, totalDamage);
        bonusAttackQueue.Clear();

        // Additional attacks are added to queue to play after the previous attack
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.ComboChain))
            bonusAttackQueue.Enqueue(new BonusAttack
            {
                upgradeName = "Combo Chain",
                damage = Mathf.RoundToInt(totalDamage * 0.5f)
            });
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.DoubleCrit) && finalRoll == 20)
            bonusAttackQueue.Enqueue(new BonusAttack
            {
                upgradeName = "Double Crit",
                damage = totalDamage
            });
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.EchoStrike) && curItem == pendingItems.Count - 1)
            bonusAttackQueue.Enqueue(new BonusAttack
            {
                upgradeName = "Echo Strike",
                damage = totalDamage
            });
        if (bonusAttackQueue.Count > 0)
            GoTo(State.BonusAttackNotifWait, 0.6f * GameSpeed);
        else
            GoTo(State.PostActionDelay, 0.8f * GameSpeed);
    }

    // Start up the bonus attack and present it
    private void StartBonusAttack()
    {
        currentBonusAttack = bonusAttackQueue.Dequeue();
        roller.ShowUpgradeNotif(currentBonusAttack.upgradeName);
        GoTo(State.BonusAttackDelay, 0.8f);
    }

    // Processes next bonus attack in queue
    private void ProcessNextBonus()
    {
        AttackEnemy(pendingItems[curItem], currentBonusAttack.damage);

        if (bonusAttackQueue.Count > 0)
            GoTo(State.BonusAttackNotifWait, 0.6f * GameSpeed);
        else
            GoTo(State.PostActionDelay, 0.8f * GameSpeed);
    }

    // Process for missing. Checks upgrades that apply to player after failing an action
    private void DoMissProcess()
    {
        if (UpgradeFightingManager.Instance.CanUseSecondChance())
        {
            UpgradeFightingManager.Instance.ConsumeSecondChance();
            roller.ShowUpgradeNotif("Second Chance");
            notifNext = NotifNext.SecondChance;
            GoTo(State.NotifWait, 0.8f);
            return;
        }

        if (UpgradeFightingManager.Instance.CanUseQuickSave())
        {
            roller.ShowUpgradeNotif("Quick Save");
            notifNext = NotifNext.QuickSave;
            GoTo(State.NotifWait, 0.8f);
            return;
        }

        UpgradeFightingManager.Instance.FailedAction();
        GoTo(State.PostMissDelay, 0.8f * GameSpeed);
    }

    // Show the notif that presents information that upgrades activated
    private void DoNotif()
    {
        switch (notifNext)
        {
            case NotifNext.SecondChance:
                roller.RollDie(0, OnRollComplete);
                state = State.WaitForRoll;
                break;

            case NotifNext.QuickSave:
                ItemData item = pendingItems[curItem];
                int quickieDamage = UpgradeFightingManager.Instance.GetQuickSaveDamage(item, curItem);
                if (quickieDamage > 0) AttackEnemy(item, quickieDamage);
                UpgradeFightingManager.Instance.SuccessfulAction(item, quickieDamage);
                GoTo(State.PostMissDelay, 0.8f * GameSpeed);
                break;
        }
    }

    // Attack the enemy(s) with how many targets your weapon has
    private void AttackEnemy(ItemData item, int damage)
    {
        if (EnemyManager.Instance.isBossDay && EnemyManager.Instance.hasDisabled && EnemyManager.Instance.disabledItem == item.ItemType)
            return;

        switch (item.target)
        {
            case 1:
                if (UpgradeFightingManager.Instance.archmageActive)
                   AttackGuys(item, damage, 2);
                else
                 AttackFirstEnemy(item, damage);
                break;
            case 2:
                if (UpgradeFightingManager.Instance.archmageActive)
                    AttackGuys(item, damage, 3);
                else
                    AttackGuys(item, damage, 2);
                    break;
            case 3:
                foreach (Transform enemyLocation in EnemyManager.Instance.spawnPoints)
                    TryAttackAt(enemyLocation, item, damage);
                break;
        }
    }

    // Attacks first enemy, basic attack that most weapons do
    private void AttackFirstEnemy(ItemData item, int damage)
    {
        foreach (Transform enemyLocation in EnemyManager.Instance.spawnPoints)
            if (TryAttackAt(enemyLocation, item, damage)) break;
    }


    // Functionality for attacking enemies at a specific location and applies the bonus and the event for enemy damage taken
    private bool TryAttackAt(Transform enemyLocation, ItemData item, int damage)
    {
        // Check if the location has an enemy in it
        if (enemyLocation.childCount == 0) return false;

        GameObject enemy = null;
        // Get the enemy at this location
        for (int i = 0; i < enemyLocation.childCount; i++)
        {
            if (enemyLocation.transform.GetChild(i).GetComponent<EnemyController>() != null)
            {
                enemy = enemyLocation.transform.GetChild(i).gameObject;
            }
        }

        if (enemy == null) return false;

        if (enemy.GetComponent<EnemyController>().GetHealth() <= 0) return false;

        bool weakness = false;
        bool resistance = false;

        if (enemy.GetComponent<EnemyController>().enemyData.weakness == item.ItemType) // Applies additional damage for weakness
        {
            damage = Mathf.RoundToInt(damage * 1.55f);
            weakness = true;
        }

        if (EnemyManager.Instance.isBossDay && EnemyManager.Instance.bossData.ability == BossAbilities.EvenNumberReduce // Reduces damage if even number for reducer boss
            && damage % 2 == 0)
        {
            damage = Mathf.RoundToInt(damage * 0.5f);
            resistance = true;
        }

        if (item.weaponBonus == WeaponBonus.PercentHealth) // Percent damage used by Blade of the Lost
        {
            int percentDamage = Mathf.RoundToInt(enemy.GetComponent<EnemyController>().GetHealth() * 0.1f);
            damage += percentDamage;
        }

        if (item.weaponBonus == WeaponBonus.GrowingDamage) // Damage growth used by Frosted Deep
        {
            damage += item.bonusDamageStacks;
            item.bonusDamageStacks++;
        }

        EventBus.Publish(new DamageTakenEvent(enemy.GetEntityId(), damage, weakness, resistance));
        return true;

    }

    // Script for when you are attacking multiple enemies with one attack
    private void AttackGuys(ItemData item, int damage, int numberOfGuys)
    {
        int hits = 0;
        foreach (Transform enemyLocation in EnemyManager.Instance.spawnPoints)
        {
            if (hits >= numberOfGuys) break;
            if (TryAttackAt(enemyLocation, item, damage)) hits++;
        }
    }




    /// <summary>
    /// Finalize the Score calculation for display.
    /// </summary>
    /// 
    private void FinalizeScore()
    {
        int count = pendingItems.Count;
        if (itemDisplay.activeSelf)
        {
            itemDisplay.SetActive(false);
        }
        state = State.Idle;
        EventBus.Publish<ScoringEndedEvent>(new ScoringEndedEvent());
        // Check if we have hit the MaxRounds or all Enemies are dead
        if (!GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().AreEnemiesAlive() || curRound == MaxRounds)
        {
            // If we have, determine if the player has won
            // TODO: change these to actually have some kind of proper display
            if (!GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().AreEnemiesAlive())
            {
                //combatCompleteText.text = "Winner!";
                int remainingRounds = MaxRounds - curRound;
                if (remainingRounds > 0)
                {
                    int bonusCoins = remainingRounds * 5;
                    EventBus.Publish(new MoneyEarnedEvent(bonusCoins, "Early Completion"));
                }
                if (UpgradeManager.Instance.HasUpgrade(UpgradeID.StableInvestments))
                {
                    int bonus = Mathf.RoundToInt(PlayerManager.Instance.Coins * 0.08f);
                    PlayerManager.Instance.Coins += bonus;
                    PlayerManager.Instance.SetCoinText();
                    EventBus.Publish(new MoneyEarnedEvent(bonus, "Stable Investments"));
                }
                if (UpgradeManager.Instance.HasUpgrade(UpgradeID.CoinFinder))
                {
                    PlayerManager.Instance.Coins += 5;
                    PlayerManager.Instance.SetCoinText();
                    EventBus.Publish(new MoneyEarnedEvent(5, "Coin Finder"));
                }
                Debug.Log("Combat Completed!");
                MenuManager.Instance.SwitchState(new VictoryMenuState());
                EventBus.Publish(new VictoryEvent(rewardDisplayText));
            }
            else
            {
                //combatCompleteText.text = "Loser.";
                Debug.Log("Combat Failed!");
                MenuManager.Instance.SwitchState(new GameOverMenuState());
            }
        }
        else
        {
            // TODO: add some better way of indicating next round!
            // If we have not hit MaxRounds & Enemies are still alive, go to the next round
            Debug.Log("Round " + curRound.ToString() + " Completed!");
            curRound++;
            roundText.text = "Round " + curRound + "/3";
            List<ItemData> currentHand = new List<ItemData>();
            ItemManager itemManager = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();
            foreach (GameObject obj in itemManager.itemPool.GetItems())
            {
                ItemController ic = obj.GetComponent<ItemController>();
                if (ic != null) currentHand.Add(ic.itemData);
            }
            UpgradeFightingManager.Instance.EndRound(currentHand);
            if (EnemyManager.Instance.isBossDay && EnemyManager.Instance.hasDisabled)
            {
                disabledTypeText.text = EnemyManager.Instance.disabledItem.ToString();
            }
            EventBus.Publish(new ScoringCompletedEvent(count));
        }
        roller.ResetText();
    }
}

/// <summary>
/// When Scoring has started
/// </summary>
public struct ScoringStartedEvent { }

/// <summary>
/// Event for when Scoring is fully Completed, and transitioning to next Round
/// </summary>
public struct ScoringCompletedEvent
{
    public int count;

    public ScoringCompletedEvent(int _count)
    {
        count = _count;
    }
}

/// <summary>
/// When Scoring has ended, prior to the Round or Victory/GameOver transitions
/// </summary>
public struct ScoringEndedEvent { }

public struct VictoryEvent
{
    public string textContent;

    public VictoryEvent(string _textContent)
    {
        textContent = _textContent;
    }
}

/// <summary>
/// Event for when an Item is used either Successfully or not
/// </summary>
public struct ItemUsedEvent
{
    public ItemData item;

    public ItemUsedEvent(ItemData _item)
    {
        item = _item;
    }
}

/// <summary>
/// Event for when an Attack Hits
/// </summary>
public struct HitEvent { }

/// <summary>
/// Event for when an Attack Misses
/// </summary>
public struct MissEvent { }
