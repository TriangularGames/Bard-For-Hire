using System.Collections;
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
    public async Task CalculateScore(List<ItemData> items)
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

            int rollResult = -1;
            //yield return roller.RollDie(result => rollResult = result); // coroutine now used for rolling die

            //OnRollComplete(rollResult);

            //yield return new WaitForSeconds(waitForRoll);

            rollResult = await roller.RollDie();
            OnRollComplete(rollResult);
        }
    }

    /// <summary>
    /// Called when the roll is complete, and the score is calculated
    /// </summary>
    /// <param name="rollValue">The value of the roll</param>
    private void OnRollComplete(int rollValue)
    {
        ItemData item = pendingItems[curItem];
        // Include in here some effect thats displayed as each note is determined
        // to be scored or not
        if (item.Playable <= rollValue)
        {
            Debug.Log($"{item.name} was played!");
            itemDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(0f, 1f, 0f, 1f);

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
                                new DamageTakenEvent(enemyLocation.transform.GetChild(0).gameObject.GetEntityId(), item.Damage));
                        break;
                    }
                }
            }
        }
        else
        {
            itemDisplay.transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 0f, 0f, 1f);
        }

        if ((curItem + 1) == pendingItems.Count)
        {
            FinalizeScore();
        }
    }

    /// <summary>
    /// Finalize the Score calculation for display.
    /// </summary>
    private void FinalizeScore()
    {
        int count = pendingItems.Count;
        foreach (ItemData item in pendingItems)
        {
            EventBus.Publish<ItemScoredEvent>(new ItemScoredEvent(item));
        }

        itemDisplay.SetActive(false);

        // Check if we have hit the MaxRounds or all Enemies are dead
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
            // If we have not hit MaxRounds & Enemies are still alive, go to the next round
            Debug.Log("Round " + curRound.ToString() + " Completed!");
            curRound++;
            roundText.text = "Round " + curRound + "/3";
            EventBus.Publish(new ScoringCompletedEvent(count));
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
