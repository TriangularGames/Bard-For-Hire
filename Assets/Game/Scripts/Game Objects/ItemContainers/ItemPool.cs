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
            InstantiateItem(PlayerManager.Instance.GetRandomItem());
        }
    }

    /// <summary>
    /// This is a temporary function for testing purposes.
    /// Unsure if we will need this permanently or not
    /// </summary>
    public void InstantiateItem(ItemData item)
    {
        GameObject itemSpawned = AssetManager.Instance.Spawn("Item", transform);
        itemSpawned.GetComponent<ItemController>().itemData = item;

        itemSpawned.GetComponent<ItemController>().Setup();
        itemSpawned.name = "Item " + item.GetEntityId();

        AddItem(itemSpawned);
    }
}