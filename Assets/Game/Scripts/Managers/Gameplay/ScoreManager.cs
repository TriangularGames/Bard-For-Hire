using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        pendingItems = items;
        curItem = -1;
        UpgradeFightingManager.Instance.StartRound();

        foreach (ItemData item in pendingItems)
        {
            if(!GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().AreEnemiesAlive()) { FinalizeScore(); break; }
            
            curItem += 1;
            Debug.Log("Item " + curItem + " being rolled for.");
            // Display item being rolled for
            itemDisplay.GetComponent<ItemController>().itemData = item;
            itemDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
            itemDisplay.GetComponent<ItemController>().Setup();
            itemDisplay.SetActive(true);
            // Remove item being checked from Hotbar
            EventBus.Publish<ItemUsedEvent>(new ItemUsedEvent(item));

            int rollResult = -1;
            int modifier = 0;

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.SkillProficiency))
            {
                modifier += 2;
            }

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.EarlyAdvantage)&& UpgradeFightingManager.Instance.isFirstTurn && curItem == 0){
                rollResult = await roller.RollWithAdvantage(modifier);
            }
            else
            {
                rollResult = await roller.RollDie(modifier);
            }
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
            itemDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);
            
            await Task.Delay(300 * GameSpeed);
            EventBus.Publish<HitEvent>(new HitEvent());
            await Task.Delay(100 * GameSpeed);
            int totalDamage = UpgradeFightingManager.Instance.GetBonusDamage(item, slotIndex, out var bonuses);
            await itemDisplay.GetComponent<ItemController>().ShowDamageBonuses(bonuses, item.Damage);
            UpgradeFightingManager.Instance.SuccessfulAction(item, totalDamage);

            AttackEnemy(item, totalDamage);

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.ComboChain))
            {
                int comboDMG = Mathf.RoundToInt(totalDamage * 0.5f);
                AttackEnemy(item, comboDMG);
            }

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.DoubleCrit))
            {
                if(finalroll == 20)
                AttackEnemy(item, totalDamage);
            }

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.EchoStrike))
            {
                if(slotIndex == pendingItems.Count - 1)
                AttackEnemy(item, totalDamage);
            }
        }
        else
        {
            itemDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 0f, 0f, 1f);
            UpgradeFightingManager.Instance.FailedAction();
            EventBus.Publish<MissEvent>(new MissEvent());
            await Task.Delay(100 * GameSpeed);

            if (UpgradeFightingManager.Instance.CanUseSecondChance())
            {
                rollValue = await roller.RollDie(GameSpeed);
                await OnRollComplete(rollValue);
                return;
            }
            if (UpgradeFightingManager.Instance.CanUseQuickSave())
            {
                int totalDamage = UpgradeFightingManager.Instance.GetBonusDamage(item, slotIndex);
                AttackEnemy(item, totalDamage);
            }
        }

        // Wait for possible animations
        await Task.Delay(800 * GameSpeed);
        if ((curItem + 1) == pendingItems.Count)
        {
            FinalizeScore();
        }
    }

    private void AttackEnemy(ItemData item, int damage)
    {
        if(EnemyManager.Instance.isBossRound && EnemyManager.Instance.hasDisabled && EnemyManager.Instance.disabledItem == item.ItemType) {
            return;
        }

        foreach (Transform enemyLocation in GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().spawnPoints)
        {
            // Check if the location has an enemy in it
            if (enemyLocation.transform.childCount > 0)
            {
                // Get the enemy at this location
                GameObject enemy = enemyLocation.transform.GetChild(0).gameObject;
                bool weakness = false;
                bool resistance = false;

                if (enemy.GetComponent<EnemyController>().enemyData.weakness == item.ItemType)
                {
                    damage = Mathf.RoundToInt(damage * 1.55f);
                    weakness = true;
                }

                if(EnemyManager.Instance.isBossRound && EnemyManager.Instance.bossData.ability == BossAbilities.EvenNumberReduce && damage % 2 == 0)
                {
                    damage = Mathf.RoundToInt(damage * 0.5f);
                    resistance = true;
                }

                if (enemy.GetComponent<EnemyController>().GetHealth() > 0)
                {
                    EventBus.Publish<DamageTakenEvent>(
                            new DamageTakenEvent(enemyLocation.transform.GetChild(0).gameObject.GetEntityId(), damage, weakness, resistance));
                    break;
                }
            }
        }
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
                    PlayerManager.Instance.Coins += bonusCoins;
                    PlayerManager.Instance.SetCoinText();
                }
                Debug.Log("Combat Completed!");
                MenuManager.Instance.SwitchState(new VictoryMenuState());
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
            EventBus.Publish(new ScoringCompletedEvent(count));
        }
        roller.ResetText();
    }
}
