using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    // Inventory Panel
    [SerializeField] private Transform inventoryLayout;
    [SerializeField] private Button backButton;

    //[SerializeField] private int inventorySpace = 10;

    /// <summary>
    /// Subscribes to necessary events on the event bus.
    /// </summary>
    private void SubscribeToEvents()
    {
        EventBus.Subscribe<ShowInventoryEvent>(OnShowInventory);
        EventBus.Subscribe<HideInventoryEvent>(OnHideInventory);
    }

    /// <summary>
    /// Unsubscribes from events on the event bus.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        EventBus.Unsubscribe<ShowInventoryEvent>(OnShowInventory);
        EventBus.Unsubscribe<HideInventoryEvent>(OnHideInventory);
    }

    private void Awake()
    {
        SubscribeToEvents();
        ToggleInventoryVisibility(false);
    }

    private void Start()
    {
        SetupInventory();

        // Back Button
        backButton.onClick.AddListener(() =>
        {
            MenuManager.Instance.ResumePreviousState();
        });
    }

    /// <summary>
    /// Initializing note inventory
    /// </summary>
    public void SetupInventory()
    {
        for (int i = 0; i < PlayerManager.Instance.noteInventory.Count; i++)
        {
            /// Spawn NoteSlot and Note from AssetManager
            GameObject note = AssetManager.Instance.Spawn("Note", inventoryLayout);
            note.name = note.name + i.ToString();

            note.GetComponent<DraggableItem>().enabled = false;
            NoteData data = Instantiate(PlayerManager.Instance.noteInventory[i]);

            note.GetComponent<NoteController>().noteData = data;
            note.GetComponent<NoteController>().Setup();
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
        this?.gameObject.SetActive(isVisible);
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
