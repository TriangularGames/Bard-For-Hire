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
    /// Prefab for creating the Slots for Notes
    /// </summary>
    private GameObject noteSlotPrefab;

    /// <summary>
    /// Maximum Slots for Notes on the MusicSheet
    /// </summary>
    [SerializeField] private int maxSlots = 4;

    /// <summary>
    /// List of all Notes within the Music sheet
    /// </summary>
    private List<GameObject> Notes;


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
        Debug.Assert(noteSlotPrefab = AssetManager.Instance.GetPrefab("NoteSlot"), "MusicSheet requires NoteSlotPrefab");
        Debug.Assert(Slots = transform.GetChild(0), "MusicSheet requires Layout for Grid");

        Notes = new List<GameObject>();

        for (int i = 0; i < maxSlots; i++)
        {
            /// Could change this to use the AssetManager Spawning
            Instantiate(noteSlotPrefab, Slots.transform);
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
