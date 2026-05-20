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

    private void Awake()
    {
        Instance = this;
    }

    public void StartRound()
    {
        currentActions.Clear();
        quickSaveUsed = false;
        successStreak = 0;
        luckySlot = Random.Range(0, 6);
    }

    public void EndRound()
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

        rollAbove10 = false;
    }

    public void EndCombat()
    {
        secondChanceUsed = false;
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

        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.SkillProficiency)) {
            roll += 2;
        }
            roll += tempDCReduce;

        if (roll > 20) roll = 20;
        if (roll < 1) roll = 1;
        return roll;
    }

    public int GetBonusDamage(ItemData item, int slotIndex)
    {
        int damage = item.Damage;

        // this is for the upgrade "Battle Tactics" (1 bonus damage for actions in middle slots)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.BattleTactics))
        {
            if (slotIndex == 2 || slotIndex == 3)
            {
                damage += 2;
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
            damage += combo;
        }

        // this is for the upgrade "Overwhelming Blows" (1 bonus damage for each action used)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.OverwhelmingBlows))
        {
            damage += currentActions.Count;
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
        }

        // this is for the upgrade "Shining Star" (2 bonus damage for each rare weapon played)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.ShiningStar))
        {
            if(item.Rarity == ObjectRarity.Rare)
            {
                damage += 2;
            }
        }
        //this is for the upgrade "Flow State" (Each action does extra damage equal to 10% of previous round's damage)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.FlowState))
        {
            damage += Mathf.RoundToInt(previousRoundDamage * 0.1f);
        }

        //this is for the upgrade "Perfect Battle" (Each consecutive action played without failing a DC gives 10% bonus damage)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.PerfectBattle))
        {
            float mult = 1f + (successStreak * 0.1f);
            damage += Mathf.RoundToInt(item.Damage * (mult - 1f));
        }
        //this is for the upgrade "Timed Swings" (Odd slots gain +1, Even slots gain +2)
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.TimedSwings))
        {
            if(slotIndex % 2 == 0)
            {
                damage += 1;
            }
            else
            {
                damage += 2;
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
                    }
                }
            }
            //this is for the upgrade "Lucky Strike" (1.25x bonus score for random slot (changes every turn))
            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.LuckyStrike))
            {
                if(slotIndex == luckySlot)
                {
                    damage += Mathf.RoundToInt(item.Damage * 0.25f);
                }
            }
        }
        damage = Mathf.RoundToInt(
        damage * tempDamgeIncrease);
        return damage;
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

}
