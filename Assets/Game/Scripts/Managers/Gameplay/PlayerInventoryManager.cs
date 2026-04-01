using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : Singleton<PlayerInventoryManager>
{
    public List<NoteData> inventory;

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
}
