using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ScoreManager calculates the score based on the Items added to AttackHand
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [SerializeField] TMP_Text combatCompleteText;
    public float score;

    [SerializeField] GameObject itemDisplay;

    // TODO: add CurrentRound Count to GameManager
    [SerializeField] private TMP_Text roundText;
    public int curRound = 1;
    private int MaxRounds = 3;

    public float waitForRoll = 7f;

    public DiceRoller roller;
    private List<ItemData> pendingItems;
    private int curItem = -1;

    private void Start()
    {
        score = 0f;
        combatCompleteText.text = "";
        itemDisplay.SetActive(false);
    }

    /// <summary>
    /// Calculates the final score for a round using ItemData List
    /// </summary>
    /// <param name="items">List of Items to be scored</param>
    public IEnumerator CalculateScore(List<ItemData> items)
    {
        pendingItems = items;
        curItem = -1;
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

            roller.RollDie(this, OnRollComplete);
            yield return new WaitForSeconds(waitForRoll);
        }
        yield return null;
    }

    /// <summary>
    /// Called when the roll is complete, and the score is calculated
    /// </summary>
    /// <param name="rollValue">The value of the roll</param>
    private void OnRollComplete(int rollValue)
    {
        ItemData item = pendingItems[curItem];
        int slotIndex = curItem;

        int finalroll = UpgradeFightingManager.Instance.GetBonusRoll(rollValue);
        // Include in here some effect thats displayed as each note is determined
        // to be scored or not
        if (item.Playable <= finalroll)
        {
            Debug.Log($"{item.name} was played!");
            itemDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);
            int totalDamage = UpgradeFightingManager.Instance.GetBonusDamage(item, slotIndex);
            UpgradeFightingManager.Instance.SuccessfulAction(item, totalDamage);

            AttackEnemy(totalDamage);

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.ComboChain))
            {
                int comboDMG = Mathf.RoundToInt(totalDamage * 0.5f);
                AttackEnemy(comboDMG);
            }

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.DoubleCrit))
            {
                if(finalroll == 20)
                AttackEnemy(totalDamage);
            }

            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.EchoStrike))
            {
                if(slotIndex == pendingItems.Count - 1)
                AttackEnemy(totalDamage);
            }
        }
        else
        {
            itemDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 0f, 0f, 1f);
            UpgradeFightingManager.Instance.FailedAction();

            if (UpgradeFightingManager.Instance.CanUseSecondChance())
            {
                roller.RollDie(this, OnRollComplete);
                return;
            }
            if (UpgradeFightingManager.Instance.CanUseQuickSave())
            { int totalDamage = UpgradeFightingManager.Instance.GetBonusDamage(item, slotIndex);
              AttackEnemy(totalDamage);

            }
        }



        if ((curItem + 1) == pendingItems.Count)
        {
            FinalizeScore();
        }
    }

    private void AttackEnemy(int damage)
    {
        foreach (Transform enemyLocation in GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().spawnPoints)
        {
            // Check if the location has an enemy in it
            if (enemyLocation.transform.childCount > 0)
            {
                // Get the enemy at this location
                GameObject enemy = enemyLocation.transform.GetChild(0).gameObject;

                if (enemy.GetComponent<EnemyController>().GetHealth() > 0)
                {
                    EventBus.Publish<DamageTakenEvent>(
                            new DamageTakenEvent(enemyLocation.transform.GetChild(0).gameObject.GetEntityId(), damage));
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
        foreach (ItemData item in pendingItems)
        {
            EventBus.Publish<ItemScoredEvent>(new ItemScoredEvent(item));
        }

        itemDisplay.SetActive(false);

        // Check if we have hit the MaxRounds or all Enemies are dead
        // TODO: fix this
        if (!GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().AreEnemiesAlive() || curRound == MaxRounds)
        {
            // If we have, determine if the player has won
            if (!GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().AreEnemiesAlive())
            {
                combatCompleteText.text = "Winner!";
                Debug.Log("Combat Completed!");
                StartCoroutine(SwitchToShop());
            }
            else
            {
                combatCompleteText.text = "Loser.";
                Debug.Log("Combat Failed!");
                StartCoroutine(SwitchToMainMenu());
            }

            // TODO: add completed screen panel of some kind
            // TODO: include some coin bonus if player finishes early!

        }
        else
        {
            // If we have not hit MaxRounds, go to the next round
            // QUESTION: Should "next round" setup be handled by the GameManager?
            Debug.Log("Round " + curRound.ToString() + " Completed!");
            curRound++;
            roundText.text = "Round " + curRound + "/3";
            GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().GrabNewItems(count);
        }


    }

    /// <summary>
    /// Switch To Shop After Combat is Completed, with a delay to show the final score and round completion message
    /// </summary>
    /// <returns></returns>
    private IEnumerator SwitchToShop()
    {
        yield return new WaitForSeconds(3f);
        GameManager.Instance.SwitchState(new ShopState());
    }

    /// <summary>
    /// Switch To Main Menu after Combat finshes and you did not win, with a delay
    /// </summary>
    /// <returns></returns>
    private IEnumerator SwitchToMainMenu()
    {
        yield return new WaitForSeconds(3f);
        MenuManager.Instance.SwitchState(new MainMenuState());
    }
}
