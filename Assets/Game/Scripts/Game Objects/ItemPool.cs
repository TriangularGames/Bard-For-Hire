using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPool : MonoBehaviour
{
    // Inventory Panel
    [HideInInspector] public Transform inventoryPanel;

    [SerializeField] private int maxSlots = 6;

    /// <summary>
    /// Get Max Slots in ItemPool
    /// </summary>
    /// <returns>Integer number of Max Slots</returns>
    public int GetMaxSlots() { return maxSlots; }

    /// <summary>
    /// Get All Notes in ItemPool
    /// </summary>
    /// <returns>List of ItemData</returns>
    public List<GameObject> GetItemPool()
    {
        return inventoryPanel.GetComponent<ItemSlot>().storedObjects;
    }

    /// <summary>
    /// Initializing ItemPool
    /// </summary>
    private void Start()
    {
        PlayerManager.Instance.ResetPool();

        Debug.Assert(inventoryPanel = transform.GetChild(0), "ItemPool requires Layout for Grid");

        // Setup Slots
        SetupSlots();

        for (int i = 0; i < maxSlots; i++)
        {
            // Instantiate Item
            Debug.Log("Added item " + i);
            InstantiateItem(PlayerManager.Instance.GetRandomItem(), inventoryPanel.transform);
        }
    }

    /// <summary>
    /// Initialize the Slots for the ItemPool
    /// </summary>
    public void SetupSlots()
    {
        inventoryPanel.GetComponent<ItemSlot>().limit = maxSlots;
    }

    /// <summary>
    /// This is a temporary function for testing purposes.
    /// Unsure if we will need this permanently or not
    /// </summary>
    public void InstantiateItem(ItemData item, Transform parent)
    {
        GameObject itemSpawned = AssetManager.Instance.Spawn("Item", parent);
        itemSpawned.GetComponent<ItemController>().itemData = item;

        itemSpawned.GetComponent<ItemController>().Setup();

        AddItem(itemSpawned);
        itemSpawned.GetComponent<Drag>().inItemPool = true;
    }

    /// <summary>
    /// Adding new items to ItemPool
    /// </summary>
    /// <param name="item">Item GameObject to add</param>
    public void AddItem(GameObject item)
    {
        if (inventoryPanel == null) return;

        if (item == null)
        {
            Debug.LogWarning("Tried to add a null item.");
            return;
        }
        if (inventoryPanel.GetComponent<ItemSlot>().storedObjects.Count >= maxSlots)
        {
            Debug.Log("Inventory is full.");
            return;
        }
        inventoryPanel.GetComponent<ItemSlot>().storedObjects.Add(item);
    }

    public void AddAll(List<GameObject> list)
    {
        inventoryPanel.GetComponent<ItemSlot>().storedObjects.AddRange(list);
    }

   /// <summary>
   /// Removing Items from ItemPool
   /// </summary>
   /// <param name="item">Item GameObject to remove</param>
    public void RemoveItem(GameObject item)
    {
        if (item == null) return;

        if (inventoryPanel.GetComponent<ItemSlot>().storedObjects.Contains(item))
        {
            inventoryPanel.GetComponent<ItemSlot>().storedObjects.Remove(item);
        }
    }
}