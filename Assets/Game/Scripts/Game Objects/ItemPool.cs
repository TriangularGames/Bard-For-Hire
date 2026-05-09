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
    

    private List<ItemData> itemPool;

    /// <summary>
    /// Get All Notes in ItemPool
    /// </summary>
    /// <returns>List of ItemData</returns>
    public List<ItemData> GetItemPool()
    {
        return itemPool;
    }

    private void Awake()
    {
        itemPool = new List<ItemData>();
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
            InstantiateItem(PlayerManager.Instance.GetRandomItem(), inventoryPanel.transform);
        }
    }

    /// <summary>
    /// Initialize the Slots for the ItemPool
    /// </summary>
    public void SetupSlots()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            /// Spawn Slot using AssetManager
            //GameObject obj = AssetManager.Instance.Spawn("ItemSlot", inventoryPanel);
            //obj.name = "ItemPoolSlot" + i;
        }
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

        inventoryPanel.GetComponent<ItemSlot>().storedObjects.Add(itemSpawned);
        AddItem(itemSpawned.GetComponent<ItemController>().itemData);
    }

    /// <summary>
    /// Adding new items to ItemPool
    /// </summary>
    /// <param name="item">ItemData to add</param>
    public void AddItem(ItemData item)
    {
        if (inventoryPanel == null) return;

        if (item == null)
        {
            Debug.LogWarning("Tried to add a null item.");
            return;
        }
        if (itemPool.Count >= maxSlots)
        {
            Debug.Log("Inventory is full.");
            return;
        }
        itemPool.Add(item);
    }

   /// <summary>
   /// Removing Items from ItemPool
   /// </summary>
   /// <param name="item">ItemData to remove</param>
    public void RemoveItem(ItemData item)
    {
        if (item == null) return;

        if (itemPool.Contains(item))
        {
            itemPool.Remove(item);
        }
    }
}