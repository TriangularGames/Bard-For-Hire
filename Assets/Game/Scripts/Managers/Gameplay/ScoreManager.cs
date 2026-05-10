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
        // Include in here some effect thats displayed as each note is determined
        // to be scored or not
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
        foreach (ItemData item in pendingItems)
        {
            if (item.Playable <= rollValue)
            {
                Debug.Log($"{item.name} was played!");
                if (item.Mult)
                {
                    score *= item.Score;
                }
                else
                {
                    score += item.Score;
                }
                EventBus.Publish<ItemRemovedEvent>(new ItemRemovedEvent(item));
            }
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

        // If scoreToBeat is 0, the Player wins
        // Else move on to the next round
        // TODO: add check for if there are no rounds left
        if (sTB < 0f)
        {
            Debug.Log("Performance Completed!");
            scoreToBeat.text = "0";
        }
        else
        {
            Debug.Log("Next round!");
            scoreToBeat.text = sTB.ToString();
        }

        ItemManager.Instance.GrabNewItems();
    }
}
