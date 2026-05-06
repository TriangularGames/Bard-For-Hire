using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public List<NoteData> noteInventory;
    public int Coins;

    // For Gameplay, what Notes are still available to be grabbed from Inventory,
    // what Notes are active, and what Notes aren't
    public List<NoteData> notesUsed;
    public List<NoteData> notesHeld;
    public List<NoteData> notesNotUsed;

    // Current selected Bard
    public BaseBard selectedBard;

    private void Start()
    {
        notesUsed = new List<NoteData>();
        notesHeld = new List<NoteData>();
        notesNotUsed = new List<NoteData>();
    }

    private void OnEnable()
    {
        EventBus.Subscribe<NoteRemovedEvent>(OnNoteRemovedEvent);
        EventBus.Subscribe<PurchaseEvent>(OnPurchaseEvent);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<NoteRemovedEvent>(OnNoteRemovedEvent);
        EventBus.Unsubscribe<PurchaseEvent>(OnPurchaseEvent);
    }

    /// <summary>
    /// When a Note is Discarded/Scored, remove it from the NotUsed list
    /// </summary>
    /// <param name="e">Event Data with Note to remove</param>
    private void OnNoteRemovedEvent(NoteRemovedEvent e)
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

    private void OnPurchaseEvent (PurchaseEvent e)
    {
        Coins -= e._amount;
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
