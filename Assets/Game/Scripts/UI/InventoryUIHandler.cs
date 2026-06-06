using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIHandler : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus.Subscribe<RefreshInventoryDisplayEvent>(RefreshDisplay);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<RefreshInventoryDisplayEvent>(RefreshDisplay);
    }

    private void Start()
    {
        DisplayGroupedInventory();
    }

    private void RefreshDisplay(RefreshInventoryDisplayEvent e)
    {
        // TODO: figure out better way of doing this without just deleting and respawning objects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
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

        if (groupedItems.Count >= 5)
        {
            GetComponent<GridLayoutGroup>().cellSize = new Vector2(100, 100);
            GetComponent<GridLayoutGroup>().spacing = new Vector2(40, 0);
        }

        // Spawn one slot per unique item, passing in the quantity
        foreach (KeyValuePair<ItemData, int> entry in groupedItems)
        {
            GameObject slot = AssetManager.Instance.Spawn("InventorySlot", transform);
            slot.GetComponent<InventorySlot>().SetupSlotInfo(entry.Key, entry.Value);
        }
    }
}