using System.Collections.Generic;
using UnityEngine;

public class NoteInventory : MonoBehaviour
{
    // Inventory Panel
    private Transform inventoryPanel;


    [SerializeField] private int inventorySpace = 10;
    

    private List<NoteData> noteInventory;

    public List<NoteData> GetNoteInventory()
    {
        return noteInventory;
    }

    private void Awake()
    {
        noteInventory = new List<NoteData>();
    }

    /// <summary>
    /// Initializing note inventory
    /// </summary>
    private void Start()
    {
        Debug.Assert(inventoryPanel = transform.GetChild(0), "NoteInventory requires Layout for Grid");

        for (int i = 0; i < inventorySpace; i++)
        {
            /// Spawn NoteSlot and Note from AssetManager
            GameObject noteSlot = AssetManager.Instance.Spawn("NoteSlot", inventoryPanel);
            GameObject note = AssetManager.Instance.Spawn("Note", noteSlot.transform);
            note.name = note.name + i.ToString();

            int noteToCreate = Random.Range(0, ResourceManager.Instance.NoteData.Length);
            NoteData data = Instantiate(ResourceManager.Instance.NoteData[noteToCreate]);

            note.GetComponent<NoteController>().noteData = data;
            note.GetComponent<NoteController>().SetSprite();

            AddNote(data);
        }

        PlayerInventoryManager.Instance.SetInventoryNotes(noteInventory);
    }

    /// <summary>
    /// Adding new notes to note inventory
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
        if (noteInventory.Count >= inventorySpace)
        {
            Debug.Log("Inventory is full.");
            return;
        }
        noteInventory.Add(note);
    }
}
