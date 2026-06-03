using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

/// <summary>
/// ScoreManager calculates the score based on the Items added to AttackHand
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [SerializeField] GameObject itemDisplay;

    [SerializeField] private TMP_Text roundText;
    public int curRound = 1;
    private int MaxRounds = 3;

    public float waitForRoll = 2f;
    private List<GameObject> lineupObjects;
    public int GameSpeed = 4;

    public DiceRoller roller;
    private List<ItemData> pendingItems;
    private int curItem = -1;

    private string rewardDisplayText;

    private void OnEnable()
    {
        EventBus.Subscribe<MoneyEarnedEvent>(MakeRewardText);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<MoneyEarnedEvent>(MakeRewardText);
    }

    private void MakeRewardText(MoneyEarnedEvent e)
    {
        rewardDisplayText += e.reason + " : " + e.coinAmount + "\n";
    }

    private void Start()
    {
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

    /// <summary>
    /// Calculates the final score for a round using ItemData List
    /// </summary>
    /// <param name="items">List of Items to be scored</param>
    public async Task CalculateScore(List<ItemData> items)
    {
        EventBus.Publish<ScoringStartedEvent>(new ScoringStartedEvent());

        pendingItems = items;
        curItem = -1;
        UpgradeFightingManager.Instance.StartRound();

        if (pendingItems == null || pendingItems.Count <= 0)
        {
            return;
        }

        UpgradeFightingManager.Instance.GetTheHandBonuses(pendingItems);

        foreach (ItemData item in pendingItems)
        {
            if (!GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().AreEnemiesAlive()) { FinalizeScore(); break; }

            curItem += 1;
            Debug.Log("Item " + curItem + " being rolled for.");
            // Display item being rolled for
            itemDisplay.GetComponent<ItemController>().itemData = item;
            itemDisplay.GetComponent<ItemController>().Setup();
            itemDisplay.SetActive(true);
            itemDisplay.GetComponent<ItemDisplayController>().Reset();
            // Remove item being checked from Hotbar
            EventBus.Publish<ItemUsedEvent>(new ItemUsedEvent(item));

            int rollResult = -1;
            int modifier = 0;

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.SkillProficiency))
            {
                modifier += 2;
            }

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.GreatWeaponMaster))
            {
                int gwmBonus = Mathf.RoundToInt(item.Playable * 0.25f);
                modifier += gwmBonus;
            }

            if (UpgradeFightingManager.Instance.tempDCReduce > 0)
            {
                modifier += UpgradeFightingManager.Instance.tempDCReduce;
            }

            if (UpgradeFightingManager.Instance.shadowThiefActive)
                modifier -= 2;

            bool useAdvantage = (UpgradeManager.Instance.HasUpgrade(UpgradeID.EarlyAdvantage) && curItem == 0) || UpgradeFightingManager.Instance.shadowThiefActive || UpgradeFightingManager.Instance.UseComeback();
            if (useAdvantage)
            {
                if (UpgradeFightingManager.Instance.UseComeback())
                {
                    await roller.ShowUpgradeNotif("Comeback!");
                    UpgradeFightingManager.Instance.UseComeback();
                }
                rollResult = await roller.RollWithAdvantage(modifier);
            }
            else
            {
                rollResult = await roller.RollDie(modifier);
            }
            UpgradeFightingManager.Instance.rolledNat20 = (roller.natRoll == 20);
            await OnRollComplete(rollResult);
        }
    }

    /// <summary>
    /// Called when the roll is complete, and the score is calculated
    /// </summary>
    /// <param name="rollValue">The value of the roll</param>
    private async Task OnRollComplete(int rollValue)
    {
        ItemData item = pendingItems[curItem];
        int slotIndex = curItem;

        int finalroll = UpgradeFightingManager.Instance.GetBonusRoll(rollValue);
        // Include in here some effect thats displayed as each weapon is determined
        // to be scored or not
        if (item.Playable <= finalroll)
        {
            Debug.Log($"{item.name} was played!");
            AudioManager.Instance.PlayClip("Success");
            itemDisplay.GetComponent<ItemDisplayController>().Success();


            await PauseExtensions.DelayRespectingPause(300 * GameSpeed);
            int totalDamage = UpgradeFightingManager.Instance.GetBonusDamage(item, slotIndex, out var bonuses);
            await itemDisplay.GetComponent<ItemController>().ShowDamageBonuses(bonuses, item.Damage);
            UpgradeFightingManager.Instance.SuccessfulAction(item, totalDamage);

            // Tell the Player Controller to play the Attack Animation
            EventBus.Publish<HitEvent>(new HitEvent());
            await PauseExtensions.DelayRespectingPause(100 * GameSpeed);

            AttackEnemy(item, totalDamage, UpgradeFightingManager.Instance.archmageActive);

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.ComboChain))
            {
                int comboDMG = Mathf.RoundToInt(totalDamage * 0.5f);
                AttackEnemy(item, comboDMG);
            }

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.DoubleCrit))
            {
                if (finalroll == 20)
                    AttackEnemy(item, totalDamage);
            }

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.EchoStrike))
            {
                if (slotIndex == pendingItems.Count - 1)
                    AttackEnemy(item, totalDamage);
            }
        }
        else
        {
            // Attack Missed
            AudioManager.Instance.PlayClip("Fail");
            itemDisplay.GetComponent<ItemDisplayController>().Fail();
            EventBus.Publish<MissEvent>(new MissEvent());
            await PauseExtensions.DelayRespectingPause(100 * GameSpeed);

            bool savedBySecondChance = false;
            bool savedByQuickSave = false;

            if (UpgradeFightingManager.Instance.CanUseSecondChance())
            {
                await roller.ShowUpgradeNotif("Second Chance");
                rollValue = await roller.RollDie(0);
                UpgradeFightingManager.Instance.rolledNat20 = (roller.natRoll == 20);
                await OnRollComplete(rollValue);
                savedBySecondChance = true;
                return;
            }
            if (!savedBySecondChance && UpgradeFightingManager.Instance.CanUseQuickSave())
            {
                await roller.ShowUpgradeNotif("Quick Save");
                int quickSaveDamage = UpgradeFightingManager.Instance.GetQuickSaveDamage(item, slotIndex);
                if (quickSaveDamage > 0) AttackEnemy(item, quickSaveDamage);
                UpgradeFightingManager.Instance.SuccessfulAction(item, quickSaveDamage);
                savedByQuickSave = true;
            }
            if (!savedByQuickSave)
                UpgradeFightingManager.Instance.FailedAction();
        }

        // Wait for possible animations
        await PauseExtensions.DelayRespectingPause(800 * GameSpeed);
        if ((curItem + 1) == pendingItems.Count)
        {
            FinalizeScore();
        }
    }

    private void AttackEnemy(ItemData item, int damage, bool archmageHitExtra = false)
    {
        if (EnemyManager.Instance.isBossDay && EnemyManager.Instance.hasDisabled && EnemyManager.Instance.disabledItem == item.ItemType)
            return;

        switch (item.target)
        {
            case 1:
                if (archmageHitExtra)
                {
                    int hits = 0;
                    foreach (Transform enemyLocation in EnemyManager.Instance.spawnPoints)
                    {
                        if (hits >= 2) break;
                        if (TryAttackAt(enemyLocation, item, damage)) hits++;
                    }
                }
                else AttackFirstEnemy(item, damage);
                break;
            case 2:
                if (archmageHitExtra)
                {
                    int hits = 0;
                    foreach (Transform enemyLocation in EnemyManager.Instance.spawnPoints)
                    {
                        if (hits >= 3) break;
                        if (TryAttackAt(enemyLocation, item, damage)) hits++;
                    }
                }
                else
                {
                    int h = 0;
                    foreach (Transform enemyLocation in EnemyManager.Instance.spawnPoints)
                    {
                        if (h >= 2) break;
                        if (TryAttackAt(enemyLocation, item, damage)) h++;
                    }
                }
                break;
            case 3:
                foreach (Transform enemyLocation in EnemyManager.Instance.spawnPoints)
                    TryAttackAt(enemyLocation, item, damage);
                break;
        }
    }

    private void AttackFirstEnemy(ItemData item, int damage)
    {
        foreach (Transform enemyLocation in EnemyManager.Instance.spawnPoints)
            if (TryAttackAt(enemyLocation, item, damage)) break;
    }



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

        if (enemy.GetComponent<EnemyController>().enemyData.weakness == item.ItemType)
        {
            damage = Mathf.RoundToInt(damage * 1.55f);
            weakness = true;
        }

        if (EnemyManager.Instance.isBossDay && EnemyManager.Instance.bossData.ability == BossAbilities.EvenNumberReduce
            && damage % 2 == 0)
        {
            damage = Mathf.RoundToInt(damage * 0.5f);
            resistance = true;
        }

        if (item.weaponBonus == WeaponBonus.PercentHealth)
        {
            int percentDamage = Mathf.RoundToInt(enemy.GetComponent<EnemyController>().GetHealth() * 0.1f);
            damage += percentDamage;
        }

        if (item.weaponBonus == WeaponBonus.GrowingDamage)
        {
            damage += item.bonusDamageStacks;
            item.bonusDamageStacks++;
        }

        EventBus.Publish(new DamageTakenEvent(enemy.GetEntityId(), damage, weakness, resistance));
        return true;

    }




    /// <summary>
    /// Finalize the Score calculation for display.
    /// </summary>
    /// 
    private void FinalizeScore()
    {
        int count = pendingItems.Count;
        //foreach (ItemData item in pendingItems)
        //{
        //    EventBus.Publish<ItemScoredEvent>(new ItemScoredEvent(item));
        //}
        if (itemDisplay.activeSelf)
        {
            itemDisplay.SetActive(false);
        }

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
