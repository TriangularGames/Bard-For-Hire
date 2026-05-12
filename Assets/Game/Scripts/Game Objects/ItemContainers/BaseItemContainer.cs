using System.Collections.Generic;
using UnityEngine;

public class BaseItemContainer : MonoBehaviour
{
    /// <summary>
    /// Script for Handling the Slot
    /// </summary>
    private ItemSlot items;

    /// <summary>
    /// Maximum Slots for Container
    /// </summary>
    [SerializeField] private int maxSlots = 4;

    /// <summary>
    /// Get Max Slots of Container
    /// </summary>
    /// <returns>Integer number of Max Slots</returns>
    public int GetMaxSlots() { return maxSlots; }

    /// <summary>
    /// Get Items as List of GameObjects
    /// </summary>
    /// <returns>List of GameObjects</returns>
    public List<GameObject> GetItems()
    {
        return items.storedObjects;
    }

    /// <summary>
    /// Get Items as List of ItemData
    /// </summary>
    /// <returns>List of ItemData</returns>
    public List<ItemData> GetItemList()
    {
        List<ItemData> data = new List<ItemData>();
        foreach (GameObject item in items.storedObjects)
        {
            data.Add(item.GetComponent<ItemData>());
        }
        return data;
    }

    private void Awake()
    {
        Debug.Assert(items = GetComponent<ItemSlot>(), "ItemContainer requires ItemSlot");

        items.limit = maxSlots;
    }

    /// <summary>
    /// Add Item into the Container's Item List
    /// </summary>
    /// <param name="item">Item to be added</param>
    public void AddItem(GameObject item)
    {
        items.storedObjects.Add(item);
    }

    /// <summary>
    /// Add List of Items into the Container's Item List
    /// </summary>
    /// <param name="list"></param>
    public void AddAll(List<GameObject> list)
    {
        items.storedObjects.AddRange(list);
    }

    /// <summary>
    /// Remove Item from the Container's Item List
    /// </summary>
    /// <param name="item">Item to be removed</param>
    public void RemoveItem(GameObject item)
    {
        if (items.storedObjects.Contains(item))
        {
            items.storedObjects.Remove(item);
        }
    }

    /// <summary>
    /// Removes List of Items from the Container's Item List
    /// </summary>
    /// <param name="list">List of Items to remove</param>
    public void RemoveAll(List<GameObject> list)
    {
        items.storedObjects.RemoveAll(item => list.Contains(item));
    }
}
