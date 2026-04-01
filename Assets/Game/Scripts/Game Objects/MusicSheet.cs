using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Music sheet class to hold Note information for Scoring
/// </summary>
public class MusicSheet : MonoBehaviour
{
    /// <summary>
    /// Layout group for the Slots
    /// </summary>
    private Transform Slots;

    /// <summary>
    /// Maximum Slots for Notes on the MusicSheet
    /// </summary>
    [SerializeField] private int maxSlots = 4;

    /// <summary>
    /// List of all Notes within the Music sheet
    /// </summary>
    private List<NoteData> Notes;

    public List<GameObject> GetNotes()
    {
        List<GameObject> list = new List<GameObject>();
        for (int i = 0; i < Slots.transform.childCount; i++)
        {
            if (Slots.transform.GetChild(i).transform.childCount != 0)
            {
                list.Add(Slots.transform.GetChild(i).transform.GetChild(0).gameObject);
            }
        }
        return list;
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
        Debug.Assert(Slots = transform.GetChild(0), "MusicSheet requires Layout for Grid");

        Notes = new List<NoteData>();

        for (int i = 0; i < maxSlots; i++)
        {
            /// Spawn Slots using AssetManager
            GameObject slot = AssetManager.Instance.Spawn("NoteSlot", Slots.transform);
            slot.name = "MusicSheetSlot" + i;
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
    public void AddNote(NoteData note)
    {
        Notes.Add(note);
    }

    /// <summary>
    /// Remove Note from this Music Sheet's Note List
    /// </summary>
    /// <param name="note">Note to be removed</param>
    public void RemoveNote(NoteData note)
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
                RemoveNote(slot.GetChild(0).GetComponent<NoteController>().noteData);
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
                AddNote(slot.GetChild(0).GetComponent<NoteController>().noteData);
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
