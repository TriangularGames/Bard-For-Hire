using System.Collections.Generic;
using UnityEngine;

public class NotePool : MonoBehaviour
{
    // Inventory Panel
    [HideInInspector] public Transform inventoryPanel;

    [SerializeField] private int maxSlots = 6;

    /// <summary>
    /// Get Max Slots in NotePool
    /// </summary>
    /// <returns>Integer number of Max Slots</returns>
    public int GetMaxSlots() { return maxSlots; }
    

    private List<NoteData> notePool;

    /// <summary>
    /// Get All Notes in NotePool
    /// </summary>
    /// <returns>List of NoteData</returns>
    public List<NoteData> GetNotePool()
    {
        return notePool;
    }

    private void Awake()
    {
        notePool = new List<NoteData>();
    }

    /// <summary>
    /// Initializing note pool
    /// </summary>
    private void Start()
    {
        PlayerManager.Instance.ResetPool();

        Debug.Assert(inventoryPanel = transform.GetChild(0), "NotePool requires Layout for Grid");

        // Setup Slots
        SetupSlots();

        for (int i = 0; i < maxSlots; i++)
        {
            // Instantiate Note
            InstantiateNote(PlayerManager.Instance.GetRandomNote(), inventoryPanel.GetChild(i).transform);
        }
    }

    /// <summary>
    /// Initialize the Slots for the Note Pool
    /// </summary>
    public void SetupSlots()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            /// Spawn Slot using AssetManager
            GameObject obj = AssetManager.Instance.Spawn("NoteSlot", inventoryPanel);
            obj.name = "NotePoolSlot" + i;
        }
    }

    /// <summary>
    /// This is a temporary function for testing purposes.
    /// Unsure if we will need this permanently or not
    /// </summary>
    public void InstantiateNote(NoteData note, Transform parent)
    {
        GameObject noteSpawned = AssetManager.Instance.Spawn("Note", parent);
        noteSpawned.GetComponent<NoteController>().noteData = note;

        noteSpawned.GetComponent<NoteController>().Setup();

        AddNote(noteSpawned.GetComponent<NoteController>().noteData);
    }

    /// <summary>
    /// Adding new notes to note pool
    /// </summary>
    /// <param name="note">Note data to add</param>
    public void AddNote(NoteData note)
    {
        if (inventoryPanel == null) return;

        if (note == null)
        {
            Debug.LogWarning("Tried to add a null note.");
            return;
        }
        if (notePool.Count >= maxSlots)
        {
            Debug.Log("Inventory is full.");
            return;
        }
        notePool.Add(note);
    }

   /// <summary>
   /// Removing notes from NotePool
   /// </summary>
   /// <param name="note"></param>
    public void RemoveNote(NoteData note)
    {
        if (note == null) return;

        if (notePool.Contains(note))
        {
            notePool.Remove(note);
        }
    }
}