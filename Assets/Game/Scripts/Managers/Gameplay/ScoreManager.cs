using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public GameObject itemDisplay;
    [SerializeField] private TMP_Text roundText;
    public int curRound = 1;
    private int MaxRounds = 3;
    public DiceRoller roller;
    public List<ItemData> pendingItems;
    private string rewardDisplayText;

    private Animator _banner;

    private List<ItemData> _pendingLineupItems;
    private int _pendingLineupIndex;
    private bool _hasPendingLineup;

    public Queue<(string name, int damage, ItemData item)> BonusAttackQueue = new Queue<(string, int, ItemData)>();

    private void OnEnable()
    {
        EventBus.Subscribe<MoneyEarnedEvent>(MakeRewardText);
        EventBus.Subscribe<LastCoinCollected>(OnLastCoinCollected);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<MoneyEarnedEvent>(MakeRewardText);
        EventBus.Unsubscribe<LastCoinCollected>(OnLastCoinCollected);
    }

    private void MakeRewardText(MoneyEarnedEvent e)
    {
        rewardDisplayText += e.reason + " : " + e.coinAmount + "\n";
    }

    private void OnLastCoinCollected(LastCoinCollected e)
    {
        if (EnemyManager.Instance.AreEnemiesAlive())
        {
            if (_hasPendingLineup)
            {
                _hasPendingLineup = false;
                CombatManager.Instance.SwitchState(new MoveLineupState(_pendingLineupItems, _pendingLineupIndex, this, skipWait: true));
            }
            else
            {
                return;
            }
        }
        else
        {
            // Victory / game over � list should be empty (or run from end of EnemyManager.RemoveEnemy)
            if (CheckCombatEnd())
            {
                _hasPendingLineup = false; // don't lineup after victory
                return;
            }
        }
    }

    private void Start()
    {
        itemDisplay.SetActive(false);
        _banner = GameObject.FindWithTag("RollBanner").GetComponent<Animator>();
        EventBus.Publish<EnterCombatEvent>(new EnterCombatEvent());
    }

    public void SetPendingLineup(List<ItemData> items, int index)
    {
        _pendingLineupItems = items;
        _pendingLineupIndex = index;
        _hasPendingLineup = true;
    }

    // Called by CalculateScoreState on first item
    public void InitializeRound(List<ItemData> items)
    {
        EventBus.Publish<RoundStartedEvent>(new RoundStartedEvent());
        pendingItems = items;
        UpgradeFightingManager.Instance.StartRound();
        UpgradeFightingManager.Instance.GetTheHandBonuses(items);
    }

    // Called by CalculateScoreState to set up item display
    public void SetupItemDisplay(int index)
    {
        ItemManager itemManager = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();
        GameObject stackedItem = itemManager.GetAttackItem(index);

        if (stackedItem != null)
        {
            itemDisplay.transform.position = stackedItem.transform.position;
            stackedItem.SetActive(false);
        }

        if (itemDisplay.transform.parent.childCount - 1 != itemDisplay.transform.GetSiblingIndex())
        {
            itemDisplay.transform.SetAsLastSibling();
        }

        itemDisplay.GetComponent<ItemController>().itemData = pendingItems[index];
        itemDisplay.GetComponent<ItemController>().Setup();
        itemDisplay.SetActive(true);
        itemDisplay.GetComponent<ItemDisplayController>().ResetDisplay();
        
    }

    public void ShowHit(int index)
    {
        AudioManager.Instance.PlayClip("Success");
        itemDisplay.GetComponent<ItemDisplayController>().Success();
    }

    public void ShowMiss()
    {
        AudioManager.Instance.PlayClip("Fail");
        itemDisplay.GetComponent<ItemDisplayController>().Fail();
        EventBus.Publish<MissEvent>(new MissEvent());
    }

    public void ApplyAttack(int index, int totalDamage, ItemData item, int finalRoll)
    {
        UpgradeFightingManager.Instance.SuccessfulAction(item, totalDamage);
        EventBus.Publish<HitEvent>(new HitEvent());
        AttackEnemy(item, totalDamage);

        BonusAttackQueue.Clear();

        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.ComboChain))
            BonusAttackQueue.Enqueue(("Combo Chain", Mathf.RoundToInt(totalDamage * 0.5f), item));

        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.DoubleCrit) && finalRoll == 20)
            BonusAttackQueue.Enqueue(("Double Crit", totalDamage, item));
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.EchoStrike) && index == pendingItems.Count - 1)
            BonusAttackQueue.Enqueue(("Echo Strike", totalDamage, item));

    }

    public void ApplyQuickSave(int index, ItemData item)
    {
        int quickSaveDamage = UpgradeFightingManager.Instance.GetQuickSaveDamage(item, index);
        if (quickSaveDamage > 0) AttackEnemy(item, quickSaveDamage);
        UpgradeFightingManager.Instance.SuccessfulAction(item, quickSaveDamage);
    }

    public void HideItemDisplay()
    {
        if (itemDisplay.activeSelf)
        {
            itemDisplay.SetActive(false);
        }
    }

    public void FinalizeRound()
    {
        _banner.ResetTrigger("Lower");
        _banner.SetTrigger("Raise");

        if (CheckCombatEnd()) return;

        int count = pendingItems.Count;
        HideItemDisplay();
        EventBus.Publish<RoundEndedEvent>(new RoundEndedEvent());
        EventBus.Publish(new ScoringCompletedEvent(count));
        Debug.Log("Round " + curRound + " Completed!");
        curRound++;
        roundText.text = "Round " + curRound + "/3";

        if (curRound == 3)
        {
            roundText.GetComponent<Animator>().SetTrigger("Flash");
        }
        List<ItemData> currentHand = new List<ItemData>();
        ItemManager itemManager = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();
        foreach (GameObject obj in itemManager.itemPool.GetItems())
        {
            ItemController icont = obj?.GetComponent<ItemController>();
            if (icont != null) currentHand.Add(icont.itemData);
        }
        UpgradeFightingManager.Instance.EndRound(currentHand);
        roller.ResetText();
    }

    public bool CheckCombatEnd()
    {
        if (!EnemyManager.Instance.AreEnemiesAlive())
        {
            int remainingRounds = MaxRounds - curRound;
            if (remainingRounds > 0)
                EventBus.Publish(new MoneyEarnedEvent(remainingRounds * 5, "Early Completion", null));

            if (EnemyManager.Instance.currentDay == EnemyManager.Instance.finalDay)
            {
                Debug.Log("Victory Achieved");
                MenuManager.Instance.SwitchState(new TotalVictoryMenuState());
                return true;
            }
            else
            {
                Debug.Log("Combat Completed!");
                MenuManager.Instance.SwitchState(new VictoryMenuState());
                EventBus.Publish(new VictoryEvent(rewardDisplayText));
                return true;
            }
        }

        if (curRound >= MaxRounds)
        {
            Debug.Log("Combat Failed!");
            EventBus.Publish<MissEvent>(new MissEvent());
            CombatManager.Instance.SwitchState(new DefaultCombatState());
            MenuManager.Instance.SwitchState(new GameOverMenuState());
            return true;
        }

        return false;
    }

    public void AttackEnemy(ItemData item, int damage)
    {
        if (EnemyManager.Instance.isBossDay && EnemyManager.Instance.hasDisabled
            && EnemyManager.Instance.disabledItem == item.ItemType)
        {
            AudioManager.Instance.PlayClip("Resist");
            return;
        }

        switch (item.target)
        {
            case 1:
                AttackGuys(item, damage, UpgradeFightingManager.Instance.archmageActive ? 2 : 1);
                break;
            case 2:
                AttackGuys(item, damage, UpgradeFightingManager.Instance.archmageActive ? 3 : 2);
                break;
            case 3:
                foreach (Transform loc in EnemyManager.Instance.spawnPoints)
                    TryAttackAt(loc, item, damage);
                break;
        }
    }

    private void AttackGuys(ItemData item, int damage, int numberOfGuys)
    {
        int hits = 0;
        foreach (Transform enemyLocation in EnemyManager.Instance.spawnPoints)
        {
            if (hits >= numberOfGuys) break;
            if (TryAttackAt(enemyLocation, item, damage)) hits++;
        }
    }

    private bool TryAttackAt(Transform enemyLocation, ItemData item, int damage)
    {
        if (enemyLocation.childCount == 0) return false;

        GameObject enemy = null;
        for (int i = 0; i < enemyLocation.transform.childCount; i++)
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

        if (EnemyManager.Instance.isBossDay
            && EnemyManager.Instance.bossData.ability == BossAbilities.EvenNumberReduce
            && damage % 2 == 0)
        {
            damage = Mathf.RoundToInt(damage * 0.5f);
            resistance = true;
        }

        if (item.weaponBonus == WeaponBonus.PercentHealth)
            damage += Mathf.RoundToInt(enemy.GetComponent<EnemyController>().GetHealth() * 0.1f);

        if (item.weaponBonus == WeaponBonus.GrowingDamage)
        {
            damage += item.bonusDamageStacks;
            item.bonusDamageStacks++;
        }

        EventBus.Publish(new DamageTakenEvent(enemy.GetEntityId(), damage, item.ItemType.ToString(), weakness, resistance));
        return true;
    }
}

// Event structs for ScoreManager to publish
public struct RoundStartedEvent { }

public struct ScoringCompletedEvent
{
    public int count;
    public ScoringCompletedEvent(int _count) { count = _count; }
}

public struct RoundEndedEvent { }

public struct VictoryEvent
{
    public string textContent;
    public VictoryEvent(string _textContent) { textContent = _textContent; }
}

public struct ItemUsedEvent
{
    public ItemData item;
    public int attackIndex;
    public ItemUsedEvent(ItemData _item, int _attackIndex)
    {
        item = _item;
        attackIndex = _attackIndex;
    }
}

public struct HitEvent { }

public struct MissEvent { }