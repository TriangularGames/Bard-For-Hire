using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Specific")]
    [SerializeField] Button rerollBtn;
    [SerializeField] Button optionsBtn;
    [SerializeField] Button nextRoundBtn;
    [SerializeField] TMP_Text dayDisplay;

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
    [SerializeField] private UpgradePool upgradeDisplayPool;
    [SerializeField] private TMP_Text upgradeLimit;

    [Header("Consumable Showcase Window")]
    [SerializeField] private Transform consumableWindow;

    [Header("Reroll Info")]
    [SerializeField] private float rerollCostChange = 1.4f;
    public float rerollCost = 5;

    private void OnEnable()
    {
        EventBus.Subscribe<UpgradeBoughtEvent>(UpdateUpgradeDisplay);
        EventBus.Subscribe<ConsumableBoughtEvent>(UpdateConsumableDisplay);

        rerollBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        optionsBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        nextRoundBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm2(); });
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<UpgradeBoughtEvent>(UpdateUpgradeDisplay);
        EventBus.Unsubscribe<ConsumableBoughtEvent>(UpdateConsumableDisplay);

        rerollBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        optionsBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        nextRoundBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm2(); });
    }

    private void Start()
    {
        EventBus.Publish<EnterShopEvent>(new EnterShopEvent());
        SetDayText();
        SetupShop();
        SetupUpgradeDisplay();
        SetupConsumableDisplay();
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
           rerollBtn.interactable = true;
        }
    }

    private bool UpgradeIsUnlocked(UpgradeData upgrade)
    {
        switch (upgrade.UpgradeID)
        {
            case UpgradeID.Archmage:
                return PlayerManager.Instance.CountItemsOfType(ItemType.Magical) >= 15;
            case UpgradeID.KnightCaptain:
                return PlayerManager.Instance.CountItemsOfType(ItemType.Slashing) >= 15;
            case UpgradeID.ShadowThief:
                return PlayerManager.Instance.CountItemsOfType(ItemType.Piercing) >= 15;
            default:
                return true;
        }
    }


    private void SetDayText()
    {
        dayDisplay.text = "Day " + GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().currentDay.ToString();
    }

    /// <summary>
    /// Generate Rarity for Item being Obtained
    /// </summary>
    /// <returns>Rarity of the Object</returns>
    private ObjectRarity RollRarity()
    {
        int total = commonWeight + uncommonWeight + rareWeight + legendaryWeight;
        int roll = Random.Range(0, total);

        if (roll < commonWeight) return ObjectRarity.Common;
        if (roll < uncommonWeight + commonWeight) return ObjectRarity.Uncommon;
        if (roll < rareWeight + commonWeight + uncommonWeight) return ObjectRarity.Rare;
        return ObjectRarity.Legendary;
    }

    /// <summary>
    /// Generate a Weapon (Item) for the shop from the total list of Weapons available
    /// given a generated ObjectRarity
    /// </summary>
    /// <returns>ItemData of generated Weapon</returns>
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

    /// <summary>
    /// Generate an Upgrade for the shop from the total list of Upgrades available
    /// given a generated ObjectRarity
    /// </summary>
    /// <returns>UpgradeData of generated Upgrade</returns>
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

    /// <summary>
    /// Generate a Consumable for the shop from the total list of Consumables available
    /// given a generated ObjectRarity
    /// </summary>
    /// <returns>ConsumableData of generated Consumable</returns>
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

    /// <summary>
    /// Initial function to setup all items available in the Shop
    /// when switching into the Shop Scene
    /// </summary>
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
        int tryThisManyTimesPleaseOkay = 1000;
        while (upgrades.Count != MAXUpgrades && tryThisManyTimesPleaseOkay-- > 0)
        {
            UpgradeData option = GetRandomUpgrade();
            bool alreadyThere = upgrades.Contains(option);
            bool alreadyGotThatOne = PlayerManager.Instance.upgradeInventory.Contains(option);
            bool unlocked = UpgradeIsUnlocked(option);
            if (!alreadyThere && !alreadyGotThatOne && unlocked)
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
            ClearUpgradeDisplay();
            foreach (UpgradeData upgrade in PlayerManager.Instance.upgradeInventory)
                upgradeDisplayPool.BringEmIn(upgrade, SetupUpgradeDisplay);
        }
        upgradeLimit.text = PlayerManager.Instance.upgradeInventory.Count + "/" + PlayerManager.Instance.MAXUpgrades;
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
                obj.GetComponent<ConsumableController>().SetTextColor(Color.white);
                Destroy(obj.GetComponent<ConsumableSelect>());
                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    if (obj.transform.GetChild(i).GetComponent<TMP_Text>())
                    {
                        Destroy(obj.transform.GetChild(i).gameObject);
                        break;
                    }
                }
            }
        }
    }

    private void ClearShop()
    {
        _items.ClearSlots();
        _upgrades.ClearSlots();
        _consumables.ClearSlots();
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
        foreach (GameObject obj in upgradeDisplayPool.storedObjects)
            Destroy(obj);
        upgradeDisplayPool.storedObjects.Clear();
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

    /// <summary>
    /// Access Options Menu from Shop
    /// </summary>
    public void Options()
    {
        MenuManager.Instance.SwitchState(new OptionsMenuState());
    }
}

/// <summary>
/// Event for when a Purchase is made
/// </summary>
public struct PurchaseEvent
{
    public int _amount;

    public PurchaseEvent(int amount)
    {
        _amount = amount;
    }
}
