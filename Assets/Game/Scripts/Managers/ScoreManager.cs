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
    public void CalculateScore(List<INote> notes)
    {
        foreach (INote note in notes)
        {
            if (note.NoteType == NoteType.Quarter)
            {
                // Quarter note scoring logic
            }
            else if (note.NoteType == NoteType.Half)
            {
                // Half note scoring logic
            }
            else if (note.NoteType == NoteType.Whole)
            {
                // Whole note scoring logic
            }
        }
    }
}
