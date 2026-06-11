using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public GameObject itemDisplay;
    [SerializeField] private TMP_Text roundText;
    public int curRound = 1;
    private int MaxRounds = 3;
    public int GameSpeed = 4;
    public DiceRoller roller;
    public List<ItemData> pendingItems;
    private string rewardDisplayText;

    public Queue<(string name, int damage, ItemData item)> BonusAttackQueue = new Queue<(string, int, ItemData)>();

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

    // Called by CalculateScoreState on first item
    public void InitializeRound(List<ItemData> items)
    {
        EventBus.Publish<ScoringStartedEvent>(new ScoringStartedEvent());
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

        itemDisplay.GetComponent<ItemController>().itemData = pendingItems[index];
        itemDisplay.GetComponent<ItemController>().Setup();
        itemDisplay.SetActive(true);
        itemDisplay.GetComponent<ItemDisplayController>().Reset();
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

    public void FinalizeScore()
    {
        int count = pendingItems.Count;
        HideItemDisplay();
        EventBus.Publish<ScoringEndedEvent>(new ScoringEndedEvent());

        if (!EnemyManager.Instance.AreEnemiesAlive() || curRound == MaxRounds)
        {
            if (!EnemyManager.Instance.AreEnemiesAlive())
            {
                int remainingRounds = MaxRounds - curRound;
                if (remainingRounds > 0)
                {
                    EventBus.Publish(new MoneyEarnedEvent(remainingRounds * 5, "Early Completion", null));
                }
                Debug.Log("Combat Completed!");
                MenuManager.Instance.SwitchState(new VictoryMenuState());
                AudioManager.Instance.PlayClip("Victory");
                EventBus.Publish(new VictoryEvent(rewardDisplayText));
            }
            else
            {
                Debug.Log("Combat Failed!");
                AudioManager.Instance.PlayClip("Lose");
                MenuManager.Instance.SwitchState(new GameOverMenuState());
            }
        }
        else
        {
            Debug.Log("Round " + curRound + " Completed!");
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

    public void AttackEnemy(ItemData item, int damage)
    {
        if (EnemyManager.Instance.isBossDay && EnemyManager.Instance.hasDisabled
            && EnemyManager.Instance.disabledItem == item.ItemType) return;

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

    private void AttackFirstEnemy(ItemData item, int damage)
    {
        foreach (Transform loc in EnemyManager.Instance.spawnPoints)
        {
            if (TryAttackAt(loc, item, damage)) break;
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
public struct ScoringStartedEvent { }

public struct ScoringCompletedEvent
{
    public int count;
    public ScoringCompletedEvent(int _count) { count = _count; }
}

public struct ScoringEndedEvent { }

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