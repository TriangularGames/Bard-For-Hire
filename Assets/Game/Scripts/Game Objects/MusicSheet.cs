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
    private List<GameObject> Notes;

    /// <summary>
    /// Maximum Slots for Notes on the MusicSheet
    /// </summary>
    [SerializeField] private int maxSlots = 4;

    [SerializeField] private GameObject noteSlotPrefab;

    /// <summary>
    /// Layout group for the Slots
    /// </summary>
    [SerializeField] private GameObject Slots;

    public List<GameObject> GetNotes()
    {
        return Notes;
    }

    public void EmptySlots()
    {
        foreach (ItemSlot slot in Slots.transform.GetComponentsInChildren<ItemSlot>())
        {
            slot.ClearNote();
        }
    }

    private void Awake()
    {
        Notes = new List<GameObject>();

        if (noteSlotPrefab == null) return;

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject obj = Instantiate(noteSlotPrefab, Slots.transform);
        }
    }

    private void Start()
    {
        ClearNotes();
    }

    /// <summary>
    /// Add Note into this Music Sheet's Note List
    /// </summary>
    /// <param name="note">Note to be added</param>
    public void AddNote(GameObject note)
    {
        Notes.Add(note);
    }

    /// <summary>
    /// Remove Note from this Music Sheet's Note List
    /// </summary>
    /// <param name="note">Note to be removed</param>
    public void RemoveNote(GameObject note)
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
            if (slot.GetComponentInChildren<NoteController>() != null)
            {
                RemoveNote(slot.GetChild(0).gameObject);
                Destroy(slot.GetChild(0).gameObject);
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
            if (slot.GetComponentInChildren<NoteController>() != null)
            {
                AddNote(slot.GetChild(0).gameObject);
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
