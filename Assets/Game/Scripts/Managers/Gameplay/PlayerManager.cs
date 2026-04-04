using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public List<NoteData> noteInventory;

    // For Gameplay, what Notes are still available to be grabbed from Inventory
    // and what Notes aren't
    public List<NoteData> notesUsed;
    public List<NoteData> notesNotUsed;

    // Current selected Bard
    public BaseBard selectedBard;
    // List of all Bard prefabs to insantiate based on selected Bard
    public List<GameObject> bardPrefabs;

    private void OnEnable()
    {
        EventBus.Subscribe<NoteRemovedEvent>(OnNoteRemovedEvent);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<NoteRemovedEvent>(OnNoteRemovedEvent);
    }

    /// <summary>
    /// When a Note is Discarded/Scored, remove it from the NotUsed list
    /// </summary>
    /// <param name="e">Event Data with Note to remove</param>
    private void OnNoteRemovedEvent(NoteRemovedEvent e)
    {
        if (notesNotUsed.Contains(e._note))
        {
            notesUsed.Add(e._note);
            notesNotUsed.Remove(e._note);
        }
        else
        {
            Debug.LogWarning("Note used is not in available inventory.");
        }
    }

    public override void Awake()
    {
        noteInventory = new List<NoteData>();

        selectedBard = new LuteBard(); // Start with lute bard
    }

    /// <summary>
    /// Retrieve inventory notes
    /// </summary>
    /// <returns></returns>
    public List<NoteData> GetInventoryNotes()
    {
        return noteInventory;
    }

    /// <summary>
    /// Store notes from note inventory
    /// </summary>
    public void SetInventoryNotes(List<NoteData> _inventoryNotes)
    {
        foreach (NoteData item in _inventoryNotes)
        {
            noteInventory.Add(item);
        }
    }

    public void ResetPool()
    {
        notesUsed.Clear();
        notesNotUsed.Clear();
        foreach (NoteData item in noteInventory)
        {
            notesNotUsed.Add(item);
        }
    }

    public NoteData GetRandomNote()
    {
        return notesNotUsed[Random.Range(0, notesUsed.Count)];
    }
}
