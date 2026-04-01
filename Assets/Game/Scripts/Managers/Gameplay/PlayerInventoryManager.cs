using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : Singleton<PlayerInventoryManager>
{
    public List<NoteData> inventory;

    // For Gameplay, what Notes are still available to be grabbed from Inventory
    // and what Notes aren't
    public List<NoteData> notesUsed;
    public List<NoteData> notesNotUsed;

    public void Start()
    {
        inventory = new List<NoteData>();
    }

    /// <summary>
    /// Retrieve inventory notes
    /// </summary>
    /// <returns></returns>
    public List<NoteData> GetInventoryNotes()
    {
        return inventory;
    }

    /// <summary>
    /// Store notes from note inventory
    /// </summary>
    public void SetInventoryNotes(List<NoteData> _inventoryNotes)
    {
        foreach (NoteData item in _inventoryNotes)
        {
            inventory.Add(item);
        }
    }

    public void ResetPool()
    {
        notesUsed.Clear();
        notesNotUsed.Clear();
        foreach (NoteData item in inventory)
        {
            notesNotUsed.Add(item);
        }
    }

    public void UsedNote(NoteData item)
    {
        if (notesNotUsed.Contains(item))
        {
            notesUsed.Add(item);
            notesNotUsed.Remove(item);
        }
        else
        {
            Debug.LogWarning("Note used is not in available inventory.");
        }
        
    }

    public NoteData GetRandomNote()
    {
        return notesNotUsed[Random.Range(0, notesUsed.Count)];
    }
}
