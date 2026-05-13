using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ScoreManager calculates the score based on the Items added to AttackHand
/// </summary>
public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] private TMP_Text scoreToBeat;
    private float score;

    // TODO: add CurrentRound Count to GameManager
    [SerializeField] private TMP_Text roundText;
    public int curRound = 1;
    private int MaxRounds = 3;

    public DiceRoller roller;
    private List<ItemData> pendingItems;

    private void Start()
    {
        score = 0f;
    }

    /// <summary>
    /// Calculates the final score for a round using ItemData List
    /// </summary>
    /// <param name="items">List of Items to be scored</param>
    public void CalculateScore(List<ItemData> items)
    {
        pendingItems = items;
        roller.RollDie(this, OnRollComplete);

    }

    /// <summary>
    /// Called when the roll is complete, and the score is calculated
    /// </summary>
    /// <param name="rollValue">The value of the roll</param>
    private void OnRollComplete(int rollValue)
    {
        score = 0;

        // Include in here some effect thats displayed as each note is determined
        // to be scored or not
        foreach (ItemData item in pendingItems)
        {
            if (item.Playable <= rollValue)
            {
                Debug.Log($"{item.name} was played!");
                if (item.Mult)
                {
                    score *= item.Damage;
                }
                else
                {
                    score += item.Damage;
                }

                foreach (Transform enemyLocation in EnemyManager.Instance.spawnPoints)
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
            EventBus.Publish<ItemRemovedEvent>(new ItemRemovedEvent(item));
        }
        Debug.Log("Total Score: " + score);
        FinalizeScore(score);
    }

    /// <summary>
    /// Finalize the Score calculation for display.
    /// </summary>
    /// <param name="playedScore">Final score obtained</param>
    private void FinalizeScore(float playedScore)
    {
        float sTB = float.Parse(scoreToBeat.text);
        sTB -= playedScore;
        scoreToBeat.text = sTB.ToString();

        // Check if we have hit the MaxRounds
        if (curRound == MaxRounds)
        {
            // If we have, determine if the player has won
            if (sTB <= 0)
            {
                Debug.Log("Combat Completed!");
            }
            else
            {
                Debug.Log("Combat Failed!");
            }

            // TODO: add completed screen panel of some kind
        }
        else
        {
            // If we have not hit MaxRounds, go to the next round
            // QUESTION: Should "next round" setup be handled by the GameManager?
            Debug.Log("Round " + curRound.ToString() + " Completed!");
            curRound++;
            roundText.text = "Round " + curRound;
            ItemManager.Instance.GrabNewItems();
        }

            
    }
}
