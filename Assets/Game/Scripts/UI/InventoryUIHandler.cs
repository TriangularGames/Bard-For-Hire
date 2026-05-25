using System.Collections.Generic;
using UnityEngine;

public class InventoryUIHandler : MonoBehaviour
{
    [SerializeField] private GameObject inventorySlot;
    [SerializeField] private Transform parent;

    private void Start()
    {
        DisplayGroupedInventory();
    }

    private void DisplayGroupedInventory()
    {
        // Group items by name and count duplicates
        Dictionary<ItemData, int> groupedItems = new Dictionary<ItemData, int>();

        foreach (ItemData item in PlayerManager.Instance.itemInventory)
        {
            // Try to find an existing key with the same name
            ItemData existingKey = null;
            foreach (ItemData key in groupedItems.Keys)
            {
                if (key.name == item.name)
                {
                    existingKey = key;
                    break;
                }
            }

            if (existingKey != null)
                groupedItems[existingKey]++;
            else
                groupedItems[item] = 1;
        }

        // Spawn one slot per unique item, passing in the quantity
        foreach (KeyValuePair<ItemData, int> entry in groupedItems)
        {
            GameObject slot = Instantiate(inventorySlot, parent);
            slot.GetComponent<InventorySlot>().SetupSlotInfo(entry.Key, entry.Value);
        }
    }
}