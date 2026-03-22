using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Music sheet class that adds notes to music sheets and calculates score
/// </summary>
public class MusicSheet : MonoBehaviour
{
    private List<INote> Notes;
    private void Awake()
    {
        Notes = new List<INote>();
    }
    private void Start()
    {
        ClearNotes();
    }
    private void AddNote(INote note)
    {
        Notes.Add(note);
    }
    private void RemoveNote(INote note)
    {
        Notes.Remove(note);
    }
    private void ClearNotes()
    {
        Notes.Clear();
    }
    private void CalculateNoteScore()
    {
        ScoreManager.Instance.CalculateScore(Notes);
    }
}
