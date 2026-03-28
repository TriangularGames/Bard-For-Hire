using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A score manager that calculates the score based on the notes added to the music sheet
/// </summary>
public class ScoreManager : Singleton<ScoreManager>
{
    private float score;
    private void Start()
    {
        score = 0f;
    }
    public void CalculateScore(List<BaseNote> notes)
    {
        score = 0;
        foreach (BaseNote note in notes)
        {
            if (note.Mult)
            {
                score *= note.Score;
            }
            else
            {
                score += note.Score;
            }
            Debug.Log("Total Score: " + score);
        }
    }
}
