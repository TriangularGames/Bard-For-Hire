using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : Singleton<PlayerInventoryManager>
{
    private List<NoteData> inventoryNotes;

    public override void Awake()
    {
        inventoryNotes = new List<NoteData>();
    }

    /// <summary>
    /// Retrieve inventory notes
    /// </summary>
    /// <returns></returns>
    public List<NoteData> GetInventoryNotes()
    {
        return inventoryNotes;
    }

    /// <summary>
    /// Store notes from note inventory
    /// </summary>
    public void SetInventoryNotes(List<NoteData> _inventoryNotes)
    {
        inventoryNotes = _inventoryNotes;
    }
}
