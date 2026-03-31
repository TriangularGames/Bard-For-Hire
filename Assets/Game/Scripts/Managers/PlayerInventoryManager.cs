using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : Singleton<PlayerInventoryManager>
{
    private List<GameObject> inventoryNotes;

    public override void Awake()
    {
        inventoryNotes = new List<GameObject>();
    }

    /// <summary>
    /// Retrieve inventory notes
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetInventoryNotes()
    {
        return inventoryNotes;
    }

    /// <summary>
    /// Store notes from note inventory
    /// </summary>
    public void SetInventoryNotes(List<GameObject> _inventoryNotes)
    {
        inventoryNotes = _inventoryNotes;
    }
}
