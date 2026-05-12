using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemPool : BaseItemContainer
{
    /// <summary>
    /// Initializing ItemPool
    /// </summary>
    private void Start()
    {
        PlayerManager.Instance.ResetPool();

        for (int i = 0; i < GetMaxSlots(); i++)
        {
            // Instantiate Item
            InstantiateItem(PlayerManager.Instance.GetRandomItem(), transform);
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
        itemSpawned.name = "Item " + item.GetEntityId();

        AddItem(itemSpawned);
        itemSpawned.GetComponent<Drag>().inItemPool = true;
    }
}