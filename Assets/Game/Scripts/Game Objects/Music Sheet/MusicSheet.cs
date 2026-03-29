using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Music sheet class to hold Note information for Scoring
/// </summary>
public class MusicSheet : MonoBehaviour
{
    /// <summary>
    /// List of all Notes within the Music sheet
    /// </summary>
    private List<BaseNote> Notes;

    [SerializeField] private GameObject Slots;

    private void Awake()
    {
        Notes = new List<BaseNote>();
    }
    private void Start()
    {
        ClearNotes();
    }

    /// <summary>
    /// Add Note into this Music Sheet's Note List
    /// </summary>
    /// <param name="note">Note to be added</param>
    public void AddNote(BaseNote note)
    {
        Notes.Add(note);
    }

    /// <summary>
    /// Remove Note from this Music Sheet's Note List
    /// </summary>
    /// <param name="note">Note to be removed</param>
    public void RemoveNote(BaseNote note)
    {
        if (Notes.Contains(note))
        {
            Notes.Remove(note);
        }
    }

    /// <summary>
    /// Clears Notes on Sheet and returns them to Note Pool
    /// </summary>
    private void ClearNotes()
    {
        Notes.Clear();
    }

    /// <summary>
    /// Clears Notes on Sheet after scoring is completed
    /// </summary>
    private void DeleteNotes()
    {
        // This should eventually be edited to allow for effects and such when they're removed
        Notes.Clear();
        foreach (Transform slot in Slots.transform)
        {
            if (slot.GetComponentInChildren<BaseNote>() != null)
            {
                RemoveNote(slot.GetComponentInChildren<BaseNote>());
                Destroy(slot.gameObject);
            }
        }
    }

    /// <summary>
    /// Goes through all Slots in order to verify Note scoring
    /// executes in the proper order
    /// </summary>
    private void VerifyOrder()
    {
        ClearNotes();
        foreach (Transform slot in Slots.transform)
        {
            if (slot.GetComponentInChildren<BaseNote>() != null)
            {
                AddNote(slot.GetComponentInChildren<BaseNote>());
                Debug.Log("Note Obtained: " + slot.GetComponentInChildren<BaseNote>().Score);
            }
        }
    }

    /// <summary>
    /// Sends Note List to ScoreManager for final score total
    /// </summary>
    public void CalculateNoteScore()
    {
        VerifyOrder();
        ScoreManager.Instance.CalculateScore(Notes);
        DeleteNotes();
    }
}
