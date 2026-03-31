using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : Singleton<PlayerInventoryManager>
{
    public List<GameObject> inventory;

    public void Start()
    {
        inventory = new List<GameObject>();
    }

    /// <summary>
    /// Retrieve inventory notes
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetInventoryNotes()
    {
        return inventory;
    }

    /// <summary>
    /// Store notes from note inventory
    /// </summary>
    public void SetInventoryNotes(List<GameObject> _inventoryNotes)
    {
        foreach (GameObject item in _inventoryNotes)
        {
            GameObject obj = Instantiate(item, transform);
            inventory.Add(obj);
        }
    }
}
