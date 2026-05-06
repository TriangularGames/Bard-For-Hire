using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// A score manager that calculates the score based on the notes added to the music sheet
/// </summary>
public class ScoreManager : Singleton<ScoreManager>
{
    [SerializeField] TMP_Text scoreToBeat;
    private float score;

    public DiceRoller roller;
    private List<NoteData> pendingNotes;

    private void Start()
    {
        score = 0f;
    }

    /// <summary>
    /// Calculates the final score for a round using Note List
    /// </summary>
    /// <param name="notes">List of Notes to be scored</param>
    public void CalculateScore(List<NoteData> notes)
    {
        // Include in here some effect thats displayed as each note is determined
        // to be scored or not
        pendingNotes = notes;
        roller.RollDie(this, OnRollComplete);

    }

    /// <summary>
    /// Called when the roll is complete, and the score is calculated
    /// </summary>
    /// <param name="rollValue">The value of the roll</param>
    private void OnRollComplete(int rollValue)
    {
        score = 0;
        foreach (NoteData note in pendingNotes)
        {
            if (note.Playable <= rollValue)
            {
                Debug.Log($"{note.name} was played!");
                if (note.Mult)
                {
                    score *= note.Score;
                }
                else
                {
                    score += note.Score;
                }
                EventBus.Publish<NoteRemovedEvent>(new NoteRemovedEvent(note));
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

        NoteManager.Instance.GrabNewNotes();
    }
}
