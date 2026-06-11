using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUIHandler : MonoBehaviour
{
    // Inventory Panel
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private Transform inventoryUnusedLayout;
    [SerializeField] private Transform inventoryUsedLayout;
    [SerializeField] private Button backButton;

    private void OnEnable()
    {
        EventBus.Subscribe<ShowInventoryEvent>(OnShowInventory);
        EventBus.Subscribe<HideInventoryEvent>(OnHideInventory);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ShowInventoryEvent>(OnShowInventory);
        EventBus.Unsubscribe<HideInventoryEvent>(OnHideInventory);
    }

    private void Start()
    {
        // Back Button
        backButton.onClick.AddListener(() =>
        {
            MenuManager.Instance.ResumePreviousState();
        });
    }

    private void ShowInventory()
    {
        // Add the NotUsed items, in full display first
        NotUsedDisplay();

        // Then add the Used items, at slightly lower alpha
        ItemsUsedDisplay();
    }

    private void NotUsedDisplay()
    {
        // Group items by name and count duplicates
        Dictionary<ItemData, int> groupedItems = new Dictionary<ItemData, int>();

        foreach (ItemData item in PlayerManager.Instance.itemsNotUsed)
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
            inventoryUnusedLayout.GetComponent<GridLayoutGroup>().cellSize = new Vector2(100, 100);
            inventoryUnusedLayout.GetComponent<GridLayoutGroup>().spacing = new Vector2(40, 0);
        }

        // Spawn one slot per unique item, passing in the quantity
        foreach (KeyValuePair<ItemData, int> entry in groupedItems)
        {
            GameObject slot = AssetManager.Instance.Spawn("InventorySlot", inventoryUnusedLayout);
            slot.GetComponent<InventorySlot>().SetupSlotInfo(entry.Key, entry.Value);
        }
    }

    private void ItemsUsedDisplay()
    {
        // Group items by name and count duplicates
        Dictionary<ItemData, int> groupedItems = new Dictionary<ItemData, int>();

        foreach (ItemData item in PlayerManager.Instance.itemsUsed)
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
            inventoryUsedLayout.GetComponent<GridLayoutGroup>().cellSize = new Vector2(100, 100);
            inventoryUsedLayout.GetComponent<GridLayoutGroup>().spacing = new Vector2(40, 0);
        }

        // Spawn one slot per unique item, passing in the quantity
        foreach (KeyValuePair<ItemData, int> entry in groupedItems)
        {
            GameObject slot = AssetManager.Instance.Spawn("InventorySlot", inventoryUsedLayout);
            slot.GetComponent<InventorySlot>().SetupSlotInfo(entry.Key, entry.Value);
            slot.GetComponent<Image>().color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
        }
    }

    #region Show/Hide Toggle
    /// <summary>
    /// Handles the event to show the Inventory.
    /// </summary>
    /// <param name="eventData">The event data associated with showing the Inventory</param>
    private void OnShowInventory(ShowInventoryEvent eventData)
    {
        // Code to show the Inventory
        ToggleInventoryVisibility(eventData.show);
        ShowInventory();
    }

    /// <summary>
    /// Handles the event to hide the Inventory.
    /// </summary>
    /// <param name="eventData">The event data associated with hiding the Inventory</param>
    private void OnHideInventory(HideInventoryEvent eventData)
    {
        // Code to hide the Inventory
        ToggleInventoryVisibility(eventData.show);
        
        // TODO: fix this so its not just deleting everything
        foreach (Transform child in inventoryUsedLayout.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in inventoryUnusedLayout.transform)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Toggles the visibility of the Inventory UI.
    /// </summary>
    /// <param name="isVisible">Whether the Inventory should be visible.</param>
    public void ToggleInventoryVisibility(bool isVisible)
    {
        inventoryPanel.SetActive(isVisible);
    }
    #endregion
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
