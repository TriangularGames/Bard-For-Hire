using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIHandler : MonoBehaviour
{
    // Inventory Panel
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventoryLayout;
    [SerializeField] private Button backButton;

    private void Start()
    {
        DisplayGroupedInventory();

        // Back Button
        backButton.onClick.AddListener(() =>
        {
            MenuManager.Instance.ResumePreviousState();
        });
    }

    private void OnEnable()
    {
        SubscribeToEvents();
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /// <summary>
    /// Subscribes to necessary events on the event bus.
    /// </summary>
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ShowInventoryEvent>(OnShowInventory);
        EventBus.Subscribe<HideInventoryEvent>(OnHideInventory);
        EventBus.Subscribe<RefreshInventoryDisplayEvent>(RefreshDisplay);
    }

    /// <summary>
    /// Unsubscribes from events on the event bus.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<ShowInventoryEvent>(OnShowInventory);
        EventBus.Unsubscribe<HideInventoryEvent>(OnHideInventory);
        EventBus.Unsubscribe<RefreshInventoryDisplayEvent>(RefreshDisplay);
    }

    private void RefreshDisplay(RefreshInventoryDisplayEvent e)
    {
        // TODO: figure out better way of doing this without just deleting and respawning objects
        foreach (Transform child in inventoryLayout)
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
            inventoryLayout.GetComponent<GridLayoutGroup>().cellSize = new Vector2(100, 100);
            inventoryLayout.GetComponent<GridLayoutGroup>().spacing = new Vector2(40, 0);
        }

        // Spawn one slot per unique item, passing in the quantity
        foreach (KeyValuePair<ItemData, int> entry in groupedItems)
        {
            GameObject slot = AssetManager.Instance.Spawn("InventorySlot", inventoryLayout);
            slot.GetComponent<InventorySlot>().SetupSlotInfo(entry.Key, entry.Value);
        }
    }

    /// <summary>
    /// Handles the event to show the Inventory.
    /// </summary>
    /// <param name="eventData">The event data associated with showing the Inventory</param>
    private void OnShowInventory(ShowInventoryEvent eventData)
    {
        // Code to show the Inventory
        ToggleInventoryVisibility(eventData.show);
    }

    /// <summary>
    /// Handles the event to hide the Inventory.
    /// </summary>
    /// <param name="eventData">The event data associated with hiding the Inventory</param>
    private void OnHideInventory(HideInventoryEvent eventData)
    {
        // Code to hide the Inventory
        ToggleInventoryVisibility(eventData.show);
    }

    /// <summary>
    /// Toggles the visibility of the Inventory UI.
    /// </summary>
    /// <param name="isVisible">Whether the Inventory should be visible.</param>
    public void ToggleInventoryVisibility(bool isVisible)
    {
        inventoryPanel.SetActive(isVisible);
    }
}

/// <summary>
/// Event struct for showing the inventory screen.
/// </summary>
public struct ShowInventoryEvent
{
    public bool show;

    public ShowInventoryEvent(bool _show)
    { show = _show; }
}

/// <summary>
/// Event struct for hiding the inventory screen.
/// </summary>
public struct HideInventoryEvent
{
    public bool show;

    public HideInventoryEvent(bool _show)
    { show = _show; }
}
