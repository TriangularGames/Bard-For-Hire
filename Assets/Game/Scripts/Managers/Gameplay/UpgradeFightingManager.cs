using System.Collections.Generic;
using UnityEngine;

public class UpgradeFightingManager : MonoBehaviour
{
    public static UpgradeFightingManager Instance;

    public List<ItemType> currentActions = new List<ItemType>();
    public List<ItemType> previousActions = new List<ItemType>();
    public int roundDamage;
    public int previousRoundDamage;
    public bool secondChanceUsed;
    public bool quickSaveUsed;
    public int luckySlot;
    public int successStreak;
    public int tempDCReduce;
    public float tempDamgeIncrease = 1f;
    public bool rollAbove10;
    public bool reroll;
    public bool isFirstTurn;
    public bool rolledNat20;
    private void Awake()
    {
        Instance = this;
    }

    public struct DamageBonus
    {
        public string source;
        public int amount;
    }

    public void StartRound()
    {
        currentActions.Clear();
        quickSaveUsed = false;
        luckySlot = Random.Range(0, 3);
        isFirstTurn = true;
    }

    public void EndRound(List<ItemData> currentHand)
    {
        previousActions.Clear();

        for (int i = 0; i < currentActions.Count; i++)
        {
            previousActions.Add(currentActions[i]);
        }

        previousRoundDamage = roundDamage;
        roundDamage = 0;

        tempDCReduce = 0;

        tempDamgeIncrease = 1f;
        isFirstTurn = false;
        rollAbove10 = false;

        if (EnemyManager.Instance.isBossDay)
        {
            if(EnemyManager.Instance.bossData.ability == BossAbilities.DisableAction)
            {
                List<ItemType> items = new List<ItemType>();
                foreach (ItemData item in currentHand)
                {
                    if (!items.Contains(item.ItemType))
                    {
                        items.Add(item.ItemType);
                    }
                }
                if (items.Count == 0) {
                    return;
                }
                EnemyManager.Instance.disabledItem = items[Random.Range(0, items.Count)];
                EnemyManager.Instance.hasDisabled = true;
            }
        }
    }

    public void EndCombat()
    {
        secondChanceUsed = false;
        EnemyManager.Instance.currentDay++;
        EnemyManager.Instance.GenerateNext();
        successStreak = 0;
    }

    public void SuccessfulAction(ItemData item, int damage)
    {
        currentActions.Add(item.ItemType);
        roundDamage+=damage;
        successStreak++;
    }

    public void FailedAction()
    {
        successStreak = 0;
    }

    public int GetBonusRoll(int roll)
    {
        if(rollAbove10 == true)
        {
            if (roll < 10)
            {
                roll = 10;
            }
        }

        //if (UpgradeManager.Instance.HasUpgrade(UpgradeID.SkillProficiency)) {
        //    roll += 2;
        //}
        //    roll += tempDCReduce;

        if (roll > 20) roll = 20;
        rolledNat20 = (roll == 20);
        if (roll < 1) roll = 1;
        return roll;
    }

    public int GetBonusDamage(ItemData item, int slotIndex, out List<DamageBonus> bonuses)
    {
        int damage = item.Damage;
        bonuses = new List<DamageBonus>();

        // this is for the upgrade "Battle Tactics" (1 bonus damage for actions in middle slots)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.BattleTactics))
        {
            if (slotIndex == 1 || slotIndex == 2)
            {
                damage += 1;
                bonuses.Add(new DamageBonus { source = "Battle Tactics", amount = 1});
            }
        }

        // this is for the upgrade "Rhythmic Attacks" (1  bonus damage for each of the same action type in a row)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.RhythmicAttacks))
        {
            int combo = 0;
            for (int i = currentActions.Count - 1; i >= 0; i--) {
                if (currentActions[i] == item.ItemType)
                {
                    combo++;
                }
                else
                {
                    combo = 0;
                    break;
                }
            }
            combo = Mathf.Min(combo, 2);
            damage += combo;
            bonuses.Add(new DamageBonus { source = "Rhythmic Attacks", amount = combo });
        }

        // this is for the upgrade "Overwhelming Blows" (1 bonus damage for each action used)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.OverwhelmingBlows))
        {
            if (currentActions.Count >= 2)
                damage += currentActions.Count - 1;
                bonuses.Add(new DamageBonus { source = "Overwhelming Blows", amount = currentActions.Count - 1 });
        }


        // this is for the upgrade "Adaptive Combat" (1 bonus damage for each different action used in a row)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.AdaptiveCombat))
        {
            List<ItemType> list = new List<ItemType>();
            list.Add(item.ItemType);
            int combo = 0;

            for (int i = currentActions.Count - 1; i >= 0; i--)
            {
                if (!list.Contains(currentActions[i]))
                {
                    list.Add(currentActions[i]);
                    combo++;
                }
                else
                {
                    break;
                }
            }
            damage += combo;
            bonuses.Add(new DamageBonus { source = "Adaptive Combat", amount = combo });
        }

        // this is for the upgrade "Shining Star" (2 bonus damage for each rare weapon played)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.ShiningStar))
        {
            if(item.Rarity == ObjectRarity.Uncommon)
            {
                damage += 2;
                bonuses.Add(new DamageBonus { source = "Shining Star", amount = 2 });
            }
        }
        //this is for the upgrade "Flow State" (Each action does extra damage equal to 10% of previous round's damage)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.FlowState))
        {
            damage += Mathf.RoundToInt(previousRoundDamage * 0.15f);
            bonuses.Add(new DamageBonus { source = "Flow State", amount = Mathf.RoundToInt(previousRoundDamage * 0.15f) });
        }

        //this is for the upgrade "Perfect Battle" (Each consecutive action played without failing a DC gives 10% bonus damage)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.PerfectBattle))
        {
            float mult = 1f + (successStreak * 0.1f);
            damage += Mathf.RoundToInt(item.Damage * (mult - 1f));
            bonuses.Add(new DamageBonus { source = "Perfect Battle", amount = Mathf.RoundToInt(item.Damage * (mult - 1f)) });
        }
        //this is for the upgrade "Timed Swings" (Odd slots gain +1, Even slots gain +2)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.TimedSwings))
        {
            if(slotIndex % 2 == 0)
            {
                damage += 0;
            }
            else
            {
                damage += 2;
                bonuses.Add(new DamageBonus { source = "Timed Swings", amount = 2 });
            }
        }
        //this is for the upgrade "Consistency" (Whenever the first 3 actions played is the same as the last turn, those notes gain 50% damage)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.Consistency))
        {
            if (slotIndex < 3)
            {
                if (previousActions.Count > slotIndex)
                {
                    if (previousActions[slotIndex] == item.ItemType)
                    {
                        damage += Mathf.RoundToInt(item.Damage * 0.5f);
                        bonuses.Add(new DamageBonus { source = "Consistency", amount = Mathf.RoundToInt(item.Damage * 0.5f) });
                    }
                }
            }
        //this is for the upgrade "Lucky Strike" (1.25x bonus score for random slot (changes every turn))
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.LuckyStrike))
         {
             if(slotIndex == luckySlot)
             {
                 damage += Mathf.RoundToInt(item.Damage * 0.25f);
                 bonuses.Add(new DamageBonus { source = "Lucky Strike", amount = Mathf.RoundToInt(item.Damage * 0.25f) });
                }
         }
        }
        damage = Mathf.RoundToInt(
        damage * tempDamgeIncrease);
        if (rolledNat20)
        {
            int before = damage;
            damage *= 2;
            bonuses.Add(new DamageBonus { source = "Critical Hit", amount = damage - before });
        }
        return damage;
    }

    public int GetBonusDamage(ItemData item, int slotIndex)
    {
        return GetBonusDamage(item, slotIndex, out _);
    }

    //this is for the upgrade "Second Chance" (Once per combat you may reroll a failed check)
    public bool CanUseSecondChance(){
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.SecondChance))
        {
            if (!secondChanceUsed)
            {
                secondChanceUsed = true;

                return true;
            }
        }

        return false;
    }
    //this is for the upgrade "Quick Save" (First failed DC of each round still activates upgrade ability.)
    public bool CanUseQuickSave()
    {
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.QuickSave))
        {
            if (!quickSaveUsed)
            {
                quickSaveUsed = true;

                return true;
            }
        }

        return false;
    }
    public int GetQuickSaveDamage(ItemData item, int slotIndex)
    {
        int full = GetBonusDamage(item, slotIndex);
        return Mathf.Max(0, full - item.Damage);
    }
}
