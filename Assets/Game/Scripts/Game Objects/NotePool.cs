using System.Collections.Generic;
using UnityEngine;

public class NotePool : MonoBehaviour
{
    // Inventory Panel
    [HideInInspector] public Transform inventoryPanel;

    [SerializeField] private int maxSlots = 6;
    

    private List<NoteData> notePool;

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

        for (int i = 0; i < maxSlots; i++)
        {
            /// Spawn Slot using AssetManager
            GameObject obj = AssetManager.Instance.Spawn("NoteSlot", inventoryPanel);
            obj.name = "NotePoolSlot" + i;

            /// Gets a random Note from the inventory
            /// This is where we need to specifically reference a list of notes that have not been used
            /// So perhaps when we get into the performance scene, during setup the inventory creates a list of all notes useable
            /// that will then be removed as the rounds go on
            int noteToGrab = Random.Range(0, PlayerManager.Instance.GetInventoryNotes().Count);

            /// Spawn Note using AssetManager
            GameObject note = AssetManager.Instance.Spawn("Note", obj.transform);
            NoteController noteController = note.GetComponent<NoteController>();
            noteController.noteData = PlayerManager.Instance.GetInventoryNotes()[noteToGrab];

            noteController.Setup();

            AddNote(note.GetComponent<NoteController>().noteData);
        }
    }

    /// <summary>
    /// Initialize the Slots for the Note Pool
    /// </summary>
    public void SetupSlots()
    {
        Debug.Assert(inventoryPanel = transform.GetChild(0), "NotePool requires Layout for Grid");

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