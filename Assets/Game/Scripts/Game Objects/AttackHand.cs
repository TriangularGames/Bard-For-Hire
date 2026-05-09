using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AttackHand class to hold Item information for Scoring
/// </summary>
public class AttackHand : MonoBehaviour
{
    /// <summary>
    /// Slot for the Items
    /// </summary>
    private ItemSlot items;

    /// <summary>
    /// Maximum Slots for Items on the AttackHand
    /// </summary>
    [SerializeField] private int maxSlots = 4;

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
        Debug.Assert(items = GetComponent<ItemSlot>(), "AttackHand requires ItemSlot");
    }

    private void Start()
    {
        ClearItems();
    }

    /// <summary>
    /// Add Item into the AttackHand's Item List
    /// </summary>
    /// <param name="item">Item to be added</param>
    public void AddItem(GameObject item)
    {
        items.storedObjects.Add(item);
    }

    /// <summary>
    /// Remove Item from the AttackHand's Item List
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
    /// Clears Items in AttackHand returns them to ItemPool
    /// </summary>
    private void ClearItems()
    {
        items.storedObjects.Clear();
    }

    /// <summary>
    /// Clears Items in AttackHand after scoring is completed
    /// </summary>
    private void DeleteItems()
    {
        // This should eventually be edited to allow for effects and such when they're removed
        items.storedObjects.Clear();
    }

    /// <summary>
    /// Goes through all Items to ensure proper order for scoring
    /// </summary>
    private void VerifyOrder()
    {
        List<GameObject> orderedItems = new List<GameObject>();
        foreach (GameObject item in items.storedObjects)
        {
            orderedItems.Add(item);
        }
        ClearItems();
        foreach (GameObject orderedItem in orderedItems)
        {
            AddItem(orderedItem);
        }
    }

    /// <summary>
    /// Sends Item List to ScoreManager for final score total
    /// </summary>
    public void CalculateScore()
    {
        VerifyOrder();
        List<ItemData> itemData = new List<ItemData>();
        foreach (GameObject itemObj in items.storedObjects)
        {
            itemData.Add(itemObj.GetComponent<ItemController>().itemData);
        }
        ScoreManager.Instance.CalculateScore(itemData);
        DeleteItems();
    }
}
