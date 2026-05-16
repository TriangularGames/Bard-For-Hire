using NUnit.Framework;
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

    [Header("Loadouts")]
    [SerializeField] private ShopItems _items;
    [SerializeField] private ShopUpgrades _upgrades;
    [SerializeField] private ShopConsumables _consumables;

    // TODO: setup rerollCost
    public int rerollCost = 5;

    private void OnEnable()
    {
        EventBus.Subscribe<ItemSelectedEvent>(CheckItemSelection);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ItemSelectedEvent>(CheckItemSelection);
    }

    private void Start()
    {
        // TODO: add this as an event publish when Scene Switching
        PlayerManager.Instance.SetCoinText();
        
        SetupShop();
        SetRerollText();
    }

    private void Update()
    {
        if (PlayerManager.Instance.GetCoinAmount() < rerollCost)
        {
            rerollBtn.interactable = false;
        }
    }

    private void SetRerollText()
    {
        rerollBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Reroll\n$" + rerollCost;
    }

    public void SetupShop()
    {
        // TODO: Edit so the items are grabbed based on rarity weight

        // Setup the Weapons
        List<ItemData> items = new List<ItemData>();
        for (int i = 0; i < MAXItems; i++)
        {
            ItemData randomData = ResourceManager.Instance.ItemData[Random.Range(0, ResourceManager.Instance.ItemData.Length - 1)];
            items.Add(randomData);
        }
        _items.SetupSlots(items);

        // Setup the Upgrades
        List<UpgradeData> upgrades = new List<UpgradeData>();
        for (int i = 0; i < MAXUpgrades; i++)
        {
            UpgradeData randomData = ResourceManager.Instance.UpgradeData[Random.Range(0, ResourceManager.Instance.UpgradeData.Length - 1)];
            upgrades.Add(randomData);
        }
        _upgrades.SetupSlots(upgrades);

        // Setup the Consumables
        List<ConsumableData> consumables = new List<ConsumableData>();
        for (int i = 0; i < MAXConsumables; i++)
        {
            ConsumableData randomData = ResourceManager.Instance.ConsumableData[Random.Range(0, ResourceManager.Instance.ConsumableData.Length - 1)];
            consumables.Add(randomData);
        }
        _consumables.SetupSlots(consumables);
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

    /// <summary>
    /// Reroll the Shop Items for a Cost
    /// </summary>
    public void ReRoll()
    {
        // Subtract coins from player
        EventBus.Publish(new PurchaseEvent(rerollCost));
        IncreaseCost();

        //Generate different items
        ClearShop();
        SetupShop();
    }

    private void IncreaseCost()
    {
        rerollCost += 5;
        rerollBtn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Reroll\n$" + rerollCost;
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
}
