using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Specific")]
    [SerializeField] Button rerollBtn;

    [Header("Shop Loadout Limits")]
    private static int MAXItems = 4;
    private static int MAXUpgrades = 2;
    private static int MAXConsumables = 2;

    [Header("Weight of Each Rarity")]
    [SerializeField] private int commonWeight = 60;
    [SerializeField] private int uncommonWeight = 25;
    [SerializeField] private int rareWeight = 12;
    [SerializeField] private int legendaryWeight = 3;

    [Header("Loadouts")]
    [SerializeField] private ShopItems _items;
    [SerializeField] private ShopUpgrades _upgrades;
    [SerializeField] private ShopConsumables _consumables;

    [Header("Upgrade Showcase Window")]
    [SerializeField] private Transform upgradeWindow;

    [Header("Consumable Showcase Window")]
    [SerializeField] private Transform consumableWindow;

    [Header("Reroll Info")]
    [SerializeField] private float rerollCostChange = 1.4f;
    public float rerollCost = 5;

    private void OnEnable()
    {
        EventBus.Subscribe<ItemSelectedEvent>(CheckItemSelection);
        EventBus.Subscribe<UpgradeBoughtEvent>(UpdateUpgradeDisplay);
        EventBus.Subscribe<ConsumableBoughtEvent>(UpdateConsumableDisplay);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ItemSelectedEvent>(CheckItemSelection);
        EventBus.Unsubscribe<UpgradeBoughtEvent>(UpdateUpgradeDisplay);
        EventBus.Unsubscribe<ConsumableBoughtEvent>(UpdateConsumableDisplay);
    }

    private void Start()
    {
        EventBus.Publish<EnterShopEvent>(new EnterShopEvent());
        SetupShop();
        SetupUpgradeDisplay();
        SetRerollText();
    }

    private void Update()
    {
        if (PlayerManager.Instance.GetCoinAmount() < Mathf.RoundToInt(rerollCost))
        {
            rerollBtn.interactable = false;
        }
        else
        {
           rerollBtn.interactable= true;
        }
    }

    private ObjectRarity RollRarity()
    {
        int total = commonWeight + uncommonWeight + rareWeight + legendaryWeight;
        int roll = Random.Range(0, total);

        if (roll < commonWeight) return ObjectRarity.Common;
        if (roll < uncommonWeight + commonWeight) return ObjectRarity.Uncommon;
        if (roll < rareWeight + commonWeight + uncommonWeight) return ObjectRarity.Rare;
        return ObjectRarity.Legendary;
    }

    private ItemData GetRandomItem()
    {
        ObjectRarity targetRarity = RollRarity();

        List<ItemData> pool = new List<ItemData>();
        foreach (ItemData item in ResourceManager.Instance.ItemData)
        {
            if (item.Rarity == targetRarity) pool.Add(item);
        }

        if (pool.Count == 0)
        {
            pool = new List<ItemData>(ResourceManager.Instance.ItemData);
        }
        return pool[Random.Range(0, pool.Count)];
    }

    private UpgradeData GetRandomUpgrade()
    {
        ObjectRarity targetRarity = RollRarity();

        List<UpgradeData> pool = new List<UpgradeData>();
        foreach (UpgradeData upgrade in ResourceManager.Instance.UpgradeData)
        {
            if (upgrade.Rarity == targetRarity) pool.Add(upgrade);
        }

        if (pool.Count == 0)
        {
            pool = new List<UpgradeData>(ResourceManager.Instance.UpgradeData);
        }
        return pool[Random.Range(0, pool.Count)];
    }

    private ConsumableData GetRandomConsumable()
    {
        ObjectRarity targetRarity = RollRarity();

        List<ConsumableData> pool = new List<ConsumableData>();
        foreach (ConsumableData cons in ResourceManager.Instance.ConsumableData)
        {
            if (cons.Rarity == targetRarity) pool.Add(cons);
        }

        if (pool.Count == 0)
        {
            pool = new List<ConsumableData>(ResourceManager.Instance.ConsumableData);
        }
        return pool[Random.Range(0, pool.Count)];
    }

    private void SetRerollText()
    {
        rerollBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Reroll\n$" + rerollCost;
    }

    public void SetupShop()
    {
        // Setup the Weapons
        List<ItemData> items = new List<ItemData>();
        for (int i = 0; i < MAXItems; i++)
        {
            items.Add(GetRandomItem());
        }
        _items.SetupSlots(items);

        // Setup the Upgrades
        GenerateUpgrades();

        // Setup the Consumables
        List<ConsumableData> consumables = new List<ConsumableData>();
        for (int i = 0; i < MAXConsumables; i++)
        {
            consumables.Add(GetRandomConsumable());
        }
        _consumables.SetupSlots(consumables);
    }

    private void GenerateUpgrades()
    {
        List<UpgradeData> upgrades = new List<UpgradeData>();
        while (upgrades.Count != MAXUpgrades)
        {
            UpgradeData option = GetRandomUpgrade();
            bool alreadyThere = upgrades.Contains(option);
            bool alreadyGotThatOne = PlayerManager.Instance.upgradeInventory.Contains(option);
           if (!alreadyThere && !alreadyGotThatOne)
                upgrades.Add(option);
        }
        _upgrades.SetupSlots(upgrades);
    }

    private void SetupUpgradeDisplay()
    {
        // TODO: fix this to not just delete and respawn the objects
        ClearUpgradeDisplay();
        if (PlayerManager.Instance.upgradeInventory.Count > 0)
        {
            foreach (UpgradeData upgrade in PlayerManager.Instance.upgradeInventory)
            {
                GameObject obj = AssetManager.Instance.Spawn("Upgrade", upgradeWindow);
                obj.GetComponent<UpgradeController>().upgradeData = upgrade;
                obj.GetComponent<UpgradeController>().Setup();
            }
        }
    }

    private void SetupConsumableDisplay()
    {
        // TODO: fix this to not just delete and respawn the objects
        ClearConsumableDisplay();
        if (PlayerManager.Instance.consumableInventory.Count > 0)
        {
            foreach (ConsumableData consumable in PlayerManager.Instance.consumableInventory)
            {
                GameObject obj = AssetManager.Instance.Spawn("Consumable", consumableWindow);
                obj.GetComponent<ConsumableController>().consumableData = consumable;
                obj.GetComponent<ConsumableController>().Setup();
                Destroy(obj.GetComponent<ConsumableSelect>());
            }
        }
    }

    private void ClearShop()
    {
        _items.ClearSlots();
        _upgrades.ClearSlots();
        _consumables.ClearSlots();
    }

    private void CheckItemSelection(ItemSelectedEvent e)
    {
        // Check all Upgrade slots, if one is selected- disable and return
        foreach (GameObject slot in _upgrades.GetSlots())
        {
            UpgradeShopSlot upgradeSlot = slot.GetComponent<UpgradeShopSlot>();
            if (slot.GetEntityId() != e.id && upgradeSlot._isSelected)
            {
                upgradeSlot.Deselect();
                return;
            }
        }

        // Check all Consumable slots, if one is selected- disable and return
        foreach (GameObject slot in _consumables.GetSlots())
        {
            ConsumableShopSlot consumableSlot = slot.GetComponent<ConsumableShopSlot>();
            if (slot.GetEntityId() != e.id && consumableSlot._isSelected)
            {
                consumableSlot.Deselect();
                return;
            }
        }

        // Check all Item slots, if one is selected- disable and return
        foreach (GameObject slot in _items.GetSlots())
        {
            ItemShopSlot itemSlot = slot.GetComponent<ItemShopSlot>();
            if (slot.GetEntityId() != e.id && itemSlot._isSelected)
            {
                itemSlot.Deselect();
                return;
            }
        }
        return;
    }

    private void UpdateUpgradeDisplay(UpgradeBoughtEvent e)
    {
        SetupUpgradeDisplay();
    }
    private void UpdateConsumableDisplay(ConsumableBoughtEvent e)
    {
        SetupConsumableDisplay();
    }

    private void ClearUpgradeDisplay()
    {
        foreach (Transform child in upgradeWindow)
        {
            Destroy(child.gameObject);
        }
    }

    private void ClearConsumableDisplay()
    {
        foreach (Transform child in consumableWindow)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Reroll the Shop Items for a Cost
    /// </summary>
    public void ReRoll()
    {
        // Subtract coins from player
        EventBus.Publish(new PurchaseEvent(Mathf.RoundToInt(rerollCost)));
        IncreaseCost();

        //Generate different items
        ClearShop();
        SetupShop();
    }

    private void IncreaseCost()
    {
        rerollCost *= rerollCostChange;
        rerollBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Reroll\n$" + Mathf.RoundToInt(rerollCost);
    }

    public void NextRound()
    {
        // Switch to Performance scene
        GameManager.Instance.SwitchState(new CombatState());
    }

    public void ViewInventory()
    {
        // Show a pop-up view of the inventory
        MenuManager.Instance.SwitchState(new InventoryMenuState());
    }

    /// <summary>
    /// Access Options Menu from Shop
    /// </summary>
    public void Options()
    {
        MenuManager.Instance.SwitchState(new OptionsMenuState());
    }
}
