using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Music sheet class that adds notes to music sheets and calculates score
/// </summary>
public class MusicSheet : MonoBehaviour
{
    private List<BaseNote> Notes;
    private void Awake()
    {
        Notes = new List<BaseNote>();
    }
    private void Start()
    {
        ClearNotes();
    }

    public void AddNote(BaseNote note)
    {
        Notes.Add(note);
    }

    public void RemoveNote(BaseNote note)
    {
        if (Notes.Contains(note))
        {
            Notes.Remove(note);
        }
    }
    private void ClearNotes()
    {
        Notes.Clear();
    }
    public void CalculateNoteScore()
    {
        ScoreManager.Instance.CalculateScore(Notes);
    }
}
