using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public List<ItemData> itemInventory;
    public List<UpgradeData> upgradeInventory;
    public List<ConsumableData> consumableInventory;

    // For Gameplay, what Notes are still available to be grabbed from Inventory,
    // what Notes are active, and what Notes aren't
    public List<ItemData> itemsUsed;
    public List<ItemData> itemsHeld;
    public List<ItemData> itemsNotUsed;

    // Current selected Character
    public PlayerController selectedCharacter;

    [Tooltip("Amount of money the Player has")]
    public int Coins;

    [Tooltip("Max amount of Upgrades player can hold")]
    public int MAXUpgrades = 3;

    [Tooltip("Max amount of Consumables player can hold")]
    public int MAXConsumables = 2;

    private void Start()
    {
        itemsUsed = new List<ItemData>();
        itemsHeld = new List<ItemData>();
        itemsNotUsed = new List<ItemData>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ItemUsedEvent>(OnItemScored);
        EventBus.Subscribe<ItemDiscardedEvent>(OnItemDiscarded);
        EventBus.Subscribe<PurchaseEvent>(OnPurchase);
        EventBus.Subscribe<ItemBoughtEvent>(OnItemBought);
        EventBus.Subscribe<UpgradeBoughtEvent>(OnUpgradeBought);
        EventBus.Subscribe<ConsumableBoughtEvent>(OnConsumableBought);
        EventBus.Subscribe<EnterShopEvent>(EnterShop);
        EventBus.Subscribe<EnterCombatEvent>(EnterCombat);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ItemUsedEvent>(OnItemScored);
        EventBus.Unsubscribe<ItemDiscardedEvent>(OnItemDiscarded);
        EventBus.Unsubscribe<PurchaseEvent>(OnPurchase);
        EventBus.Unsubscribe<ItemBoughtEvent>(OnItemBought);
        EventBus.Unsubscribe<UpgradeBoughtEvent>(OnUpgradeBought);
        EventBus.Unsubscribe<ConsumableBoughtEvent>(OnConsumableBought);
        EventBus.Unsubscribe<EnterShopEvent>(EnterShop);
        EventBus.Unsubscribe<EnterCombatEvent>(EnterCombat);
    }

    private void EnterCombat(EnterCombatEvent e)
    {
        SetCoinText();
        SetConsumables();
    }

    private void EnterShop(EnterShopEvent e)
    {
        SetCoinText();
    }

    /// <summary>
    /// When an Item is Scored, remove it from the Held list
    /// </summary>
    /// <param name="e">Event Data with Item to remove</param>
    private void OnItemScored(ItemUsedEvent e)
    {
        RemoveItem(e.item);
    }

    /// <summary>
    /// When an Item is Discarded, remove it from the Held list
    /// </summary>
    /// <param name="e">Event Data with Item to remove</param>
    private void OnItemDiscarded(ItemDiscardedEvent e)
    {
        RemoveItem(e.item);
    }

    private void RemoveItem(ItemData item)
    {
        if (itemsHeld.Contains(item))
        {
            itemsUsed.Add(item);
            itemsHeld.Remove(item);
        }
        else
        {
            Debug.LogWarning("Item used is not in available inventory.");
        }
    }

    /// <summary>
    /// When making a purchase from the shop, subtract cost
    /// </summary>
    /// <param name="e">Data of cost to subtract</param>
    private void OnPurchase(PurchaseEvent e)
    {
        Coins -= e._amount;
        SetCoinText();
    }

    /// <summary>
    /// When an Item is purchased from the shop, add it's data to the inventory
    /// </summary>
    /// <param name="e">Data of Item to add</param>
    private void OnItemBought(ItemBoughtEvent e)
    {
        itemInventory.Add(e.data);
    }

    /// <summary>
    /// When an Upgrade is purchased from the shop, add it's data to the inventory
    /// </summary>
    /// <param name="e">Data of Upgrade to add</param>
    private void OnUpgradeBought(UpgradeBoughtEvent e)
    {
        upgradeInventory.Add(e.data);
    }

    /// <summary>
    /// When an Consumable is purchased from the shop, add it's data to the inventory
    /// </summary>
    /// <param name="e">Data of Consumable to add</param>
    private void OnConsumableBought(ConsumableBoughtEvent e)
    {
        consumableInventory.Add(e.data);
    }

    private void SetConsumables()
    {
        GameObject.FindWithTag("ItemManager").GetComponent<ConsumableManager>().consumables.AddRange(consumableInventory);
    }

    public override void Awake()
    {
        base.Awake();
        if (itemInventory == null)
        {
            itemInventory = new List<ItemData>();
        }
    }

    public void SetCoinText()
    {
        GameObject.FindWithTag("Coins").GetComponent<TMP_Text>().text = Coins.ToString();
    }

    /// <summary>
    /// Retrieve inventory
    /// </summary>
    /// <returns></returns>
    public List<ItemData> GetInventoryItems()
    {
        return itemInventory;
    }

    /// <summary>
    /// Retrieve the amount of money Player has
    /// </summary>
    /// <returns></returns>
    public int GetCoinAmount()
    {
        return Coins;
    }

    /// <summary>
    /// Store items from inventory
    /// </summary>
    public void SetInventoryNotes(List<ItemData> _inventoryNotes)
    {
        foreach (ItemData item in _inventoryNotes)
        {
            itemInventory.Add(item);
        }
    }

    public void ResetPool()
    {
        itemsUsed.Clear();
        itemsHeld.Clear();
        itemsNotUsed.Clear();
        foreach (ItemData item in itemInventory)
        {
            itemsNotUsed.Add(item);
        }
    }

    /// <summary>
    /// Get a Random Item from the Unused Items
    /// </summary>
    /// <returns>ItemData of Item from Available Inventory</returns>
    public ItemData GetRandomItem()
    {
        ItemData item = itemsNotUsed[Random.Range(0, itemsNotUsed.Count)];
        itemsNotUsed.Remove(item);
        itemsHeld.Add(item);
        return item;
    }

    public void RemoveInventoryItem(ItemData item)
    {
        if (itemInventory.Contains(item))
        {
            itemInventory.Remove(item);
        }
    }

    public void CloneInventoryItem(ItemData item)
    {
        itemInventory.Add(item);
    }

    public ItemData TransformInventoryItem(ItemData oldItem)
    {
        ItemData newItem = GetRandomInventoryItem();
        int inventoryIndex = itemInventory.IndexOf(oldItem);

        if (inventoryIndex != -1)
        {
            itemInventory[inventoryIndex] = newItem;
        }

        int heldIndex = itemsHeld.IndexOf(oldItem);

        if (heldIndex != -1)
        {
            itemsHeld[heldIndex] = newItem;
        }

        int unusedIndex = itemsNotUsed.IndexOf(oldItem);

        if (unusedIndex != -1)
        {
            itemsNotUsed[unusedIndex] = newItem;
        }

        return newItem;
    }

    public ItemData GetRandomInventoryItem()
    {
        return itemInventory[Random.Range(0, itemInventory.Count)];
    }
}
