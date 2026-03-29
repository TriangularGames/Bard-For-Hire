using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A score manager that calculates the score based on the notes added to the music sheet
/// </summary>
public class ScoreManager : Singleton<ScoreManager>
{
    private float score;

    public DiceRoller roller;

    private void Start()
    {
        score = 0f;
    }

    /// <summary>
    /// Calculates the final score for a round using Note List
    /// </summary>
    /// <param name="notes">List of Notes to be scored</param>
    public async void CalculateScore(List<BaseNote> notes)
    {
        // Include in here some effect thats displayed as each note is determined
        // to be scored or not
        int rollValue = await roller.RollDie();
        score = 0;
        foreach (BaseNote note in notes)
        {
            if (note.Playable <= rollValue)
            {
                Debug.Log("Note was played!");
                if (note.Mult)
                {
                    score *= note.Score;
                }
                else
                {
                    score += note.Score;
                }
            }  
        }
        Debug.Log("Total Score: " + score);
    }
}
