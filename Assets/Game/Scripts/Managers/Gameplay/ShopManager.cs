using NUnit.Framework;
using System.Collections.Generic;
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
        SetupShop();
    }

    private void Update()
    {
        if (PlayerManager.Instance.GetCoinAmount() < rerollCost)
        {
            rerollBtn.interactable = false;
        }
    }

    public void SetupShop()
    {
        // This will generate the 5 notes and 3 upgrades available for purchase

        // TODO: figure out how it gets the data to input. Presumably it'll use the resource manager for that?

        // Setup the Weapons
        List<ItemData> items = new List<ItemData>();
        for (int i = 0; i < MAXItems; i++)
        {
            ItemData randomData = ResourceManager.Instance.ItemData[Random.Range(0, ResourceManager.Instance.ItemData.Length - 1)];
            items.Add(randomData);
        }
        _items.SetupSlots(items);
    }

    private void CheckItemSelection(ItemSelectedEvent e)
    {
        // Check all Upgrade slots, if one is selected- disable and return

        // Check all Consumable slots, if one is selected- disable and return

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

    public void ReRoll()
    {
        // Subtract coins from player
        EventBus.Publish(new PurchaseEvent(rerollCost));

        //Generate different notes and upgrades
    }

    public void NextRound()
    {
        // Switch to Performance scene
    }

    public void ViewInventory()
    {
        // Show a pop-up view of the inventory
    }
}
