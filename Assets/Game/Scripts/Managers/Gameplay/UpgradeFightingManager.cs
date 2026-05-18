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

    public int GetBonusDamage(ItemData item, int slotIndex)
    {
        int bonus = 0;

        // this is for the upgrade "Battle Tactics" (1 bonus damage for actions in middle slots)
        if(UpgradeManager.Instance.HasUpgrade("Battle Tactics"))
        {
            if (slotIndex == 3 || slotIndex == 4)
            {
                bonus += 2;
            }
        }

        // this is for the upgrade "Rhythmic Attacks" (1  bonus damage for each of the same action type in a row)
        if (UpgradeManager.Instance.HasUpgrade("Rhythmic Attacks"))
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
            bonus += combo;
        }

        // this is for the upgrade "Overwhelming Blows" (1 bonus damage for each action used)
        if (UpgradeManager.Instance.HasUpgrade("Overwhelming Blows"))
        {
            bonus += currentActions.Count;
        }

        // this is for the upgrade "Adaptive Combat" (1 bonus damage for each different action used in a row)
        if (UpgradeManager.Instance.HasUpgrade("Adaptive Combat"))
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
            bonus += combo;
        }

        // this is for the upgrade "Shining Star" (2 bonus damage for each rare weapon played)
        if (UpgradeManager.Instance.HasUpgrade("Shining Star"))
        {
            if(item.Rarity == ObjectRarity.Rare)
            {
                bonus += 2;
            }
        }
        //this is for the upgrade "Flow State" (Each action does extra damage equal to 10% of previous round's damage)
        if (UpgradeManager.Instance.HasUpgrade("Flow State"))
        {
            bonus += Mathf.RoundToInt(previousRoundDamage * 0.1f);
        }

        //this is for the upgrade "Perfect Battle" (Each consecutive action played without failing a DC gives 10% bonus damage)
        if (UpgradeManager.Instance.HasUpgrade("Perfect Battle"))
        {
            float mult = 1f + (successStreak * 0.1f);
            bonus += Mathf.RoundToInt(item.Damage * (mult - 1f));
        }
        //this is for the upgrade "Timed Swings" (Odd slots gain +1, Even slots gain +2)
        if (UpgradeManager.Instance.HasUpgrade("Timed Swings"))
        {
            if(slotIndex % 2 == 0)
            {
                bonus += 1;
            }
            else
            {
                bonus += 2;
            }
        }
        //this is for the upgrade "Consistency" (Whenever the first 3 actions played is the same as the last turn, those notes gain 50% damage)
        if (UpgradeManager.Instance.HasUpgrade("Consistency"))
        {
            if (slotIndex < 3)
            {
                if (previousActions.Count > slotIndex)
                {
                    if (previousActions[slotIndex] == item.ItemType)
                    {
                        bonus += Mathf.RoundToInt(item.Damage * 0.5f);
                    }
                }
            }
            //this is for the upgrade "Lucky Strike" (1.25x bonus score for random slot (changes every turn))
            if (UpgradeManager.Instance.HasUpgrade("Lucky Strike"))
            {
                if(slotIndex == luckySlot)
                {
                    bonus += Mathf.RoundToInt(item.Damage * 0.25f);
                }
            }
        }
        return bonus;
    }
    //this is for the upgrade "Second Chance" (Once per combat you may reroll a failed check)
    public bool CanUseSecondChance(){
        if (UpgradeManager.Instance.HasUpgrade("Second Chance"))
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
        if (UpgradeManager.Instance.HasUpgrade("Quick Save"))
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
