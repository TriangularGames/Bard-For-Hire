using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public List<NoteData> noteInventory;
    public List<UpgradeData> upgradeInventory;

    // For Gameplay, what Notes are still available to be grabbed from Inventory,
    // what Notes are active, and what Notes aren't
    public List<NoteData> notesUsed;
    public List<NoteData> notesHeld;
    public List<NoteData> notesNotUsed;

    // Current selected Bard
    public BaseBard selectedBard;

    public int Coins;

    private void Start()
    {
        notesUsed = new List<NoteData>();
        notesHeld = new List<NoteData>();
        notesNotUsed = new List<NoteData>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<NoteRemovedEvent>(OnNoteRemoved);
        EventBus.Subscribe<PurchaseEvent>(OnPurchase);
        EventBus.Subscribe<NoteBoughtEvent>(OnNoteBought);
        EventBus.Subscribe<UpgradeBoughtEvent>(OnUpgradeBought);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<NoteRemovedEvent>(OnNoteRemoved);
        EventBus.Unsubscribe<PurchaseEvent>(OnPurchase);
        EventBus.Unsubscribe<NoteBoughtEvent>(OnNoteBought);
        EventBus.Unsubscribe<UpgradeBoughtEvent>(OnUpgradeBought);
    }

    /// <summary>
    /// When a Note is Discarded/Scored, remove it from the NotUsed list
    /// </summary>
    /// <param name="e">Event Data with Note to remove</param>
    private void OnNoteRemoved(NoteRemovedEvent e)
    {
        if (notesHeld.Contains(e._note))
        {
            notesUsed.Add(e._note);
            notesHeld.Remove(e._note);
        }
        else
        {
            Debug.LogWarning("Note used is not in available inventory.");
        }
    }

    /// <summary>
    /// When making a purchase from the shop, subtract cost
    /// </summary>
    /// <param name="e">Data of cost to subtract</param>
    private void OnPurchase(PurchaseEvent e)
    {
        Coins -= e._amount;
    }

    /// <summary>
    /// When a Note is purchased from the shop, add it's data to the inventory
    /// </summary>
    /// <param name="e">Data of Note to add</param>
    private void OnNoteBought(NoteBoughtEvent e)
    {
        noteInventory.Add(e.data);
    }

    /// <summary>
    /// When an Upgrade is purchased from the shop, add it's data to the inventory
    /// </summary>
    /// <param name="e">Data of Upgrade to add</param>
    private void OnUpgradeBought(UpgradeBoughtEvent e)
    {
        upgradeInventory.Add(e.data);
    }

    public override void Awake()
    {
        if (noteInventory == null)
        {
            noteInventory = new List<NoteData>();
        }

        //selectedBard = new LuteBard(); // Start with lute bard
    }

    /// <summary>
    /// Retrieve inventory notes
    /// </summary>
    /// <returns></returns>
    public List<NoteData> GetInventoryNotes()
    {
        return noteInventory;
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
    /// Store notes from note inventory
    /// </summary>
    public void SetInventoryNotes(List<NoteData> _inventoryNotes)
    {
        foreach (NoteData item in _inventoryNotes)
        {
            noteInventory.Add(item);
        }
    }

    public void ResetPool()
    {
        notesUsed.Clear();
        notesHeld.Clear();
        notesNotUsed.Clear();
        foreach (NoteData item in noteInventory)
        {
            notesNotUsed.Add(item);
        }
    }

    /// <summary>
    /// Get a Random Note from the Unused Notes
    /// </summary>
    /// <returns>NoteData of Note from Available Inventory</returns>
    public NoteData GetRandomNote()
    {
        NoteData note = notesNotUsed[Random.Range(0, notesNotUsed.Count)];
        notesNotUsed.Remove(note);
        notesHeld.Add(note);
        return note;
    }
}
