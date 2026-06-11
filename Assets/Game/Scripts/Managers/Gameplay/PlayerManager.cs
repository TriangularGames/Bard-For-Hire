using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [Header("Starting Inventory Loadout")]
    [SerializeField] private List<ItemData> _defaultInventory;

    [Header("Active Player Inventory")]
    public List<ItemData> itemInventory;
    public List<UpgradeData> upgradeInventory;
    public List<ConsumableData> consumableInventory;

    // For Gameplay, what Weapons are still available to be grabbed from Inventory,
    // what Weapons are active, and what Weapons aren't
    [Header("Weapons Available For Combat")]
    [Tooltip("Weapons played")]
    public List<ItemData> itemsUsed;
    [Tooltip("Weapons in Hand")]
    public List<ItemData> itemsHeld;
    [Tooltip("Weapons not drawn")]
    public List<ItemData> itemsNotUsed;

    [Header("Other Info")]
    [Tooltip("Amount of money the Player has")]
    public int Coins;

    [Tooltip("Max amount of Upgrades player can hold")]
    public int MAXUpgrades = 4;
    private GameObject UpgradeLimitTxt;

    [Tooltip("Max amount of Consumables player can hold")]
    public int MAXConsumables = 2;
    private GameObject ConsumableLimitTxt;

    [Header("Overall Stats (For the EndRun Screen)")]
    private int TotalMoneyGained;
    public int totalMoneyGained { get { return TotalMoneyGained; } set { TotalMoneyGained = value; } }
    
    // TODO: set this up
    private int HighestDamageDealt;
    public int highestDamageDealt { get { return HighestDamageDealt; } set { HighestDamageDealt = value; } }
    
    // TODO: set this up
    private string MostUsedWeapon;
    public string mostUsedWeapon { get { return MostUsedWeapon; } set { MostUsedWeapon = value; } }

    private void Start()
    {
        itemsUsed = new List<ItemData>();
        itemsHeld = new List<ItemData>();
        itemsNotUsed = new List<ItemData>();
        totalMoneyGained = 0;
        highestDamageDealt = 0;
        mostUsedWeapon = "";
    }

    #region EventBus Event Subscriptions
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
        EventBus.Subscribe<MoneyEarnedEvent>(AddMoney);
        EventBus.Subscribe<ResetGameEvent>(ResetGame);
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
        EventBus.Unsubscribe<MoneyEarnedEvent>(AddMoney);
        EventBus.Unsubscribe<ResetGameEvent>(ResetGame);
    }
    #endregion

    // Reset to defaults
    private void ResetGame(ResetGameEvent e)
    {
        Coins = 0;
        upgradeInventory.Clear();
        consumableInventory.Clear();
        itemsUsed.Clear();
        itemsHeld.Clear();
        itemsNotUsed.Clear();
        itemInventory.Clear();
        itemInventory.AddRange(_defaultInventory);
        foreach (ItemData item in itemInventory)
            item.bonusDamageStacks = 0;
    }

    private void AddMoney(MoneyEarnedEvent e)
    {
        totalMoneyGained += e.coinAmount;
        SetCoinText();
    }

    private void Update()
    {
        
    }

    private void EnterCombat(EnterCombatEvent e)
    {
        SetCoinText();
        SetLimitText();
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
        EventBus.Publish(new RefreshInventoryDisplayEvent());
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
        EventBus.Publish(new RefreshInventoryDisplayEvent());
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

    public int CountItemsOfType(ItemType type)
    {
        int count = 0;
        foreach (ItemData item in itemInventory)
            if (item.ItemType == type) count++;
        return count;
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

    public void SetLimitText()
    {
        if (UpgradeLimitTxt == null)
        {
            UpgradeLimitTxt = GameObject.FindWithTag("UpgradeLimit");
        }
        if (ConsumableLimitTxt == null)
        {
            ConsumableLimitTxt = GameObject.FindWithTag("ConsumableLimit");
        }

        UpgradeLimitTxt.GetComponent<TMP_Text>().text = upgradeInventory.Count + "/" + MAXUpgrades;
        ConsumableLimitTxt.GetComponent<TMP_Text>().text = consumableInventory.Count + "/" + MAXConsumables;
    }

    public void RefreshItems()
    {
        itemsNotUsed.AddRange(itemsUsed);
        itemsUsed.Clear();
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

/// <summary>
/// Event for when Item is added to Inventory
/// </summary>
public struct RefreshInventoryDisplayEvent { }
