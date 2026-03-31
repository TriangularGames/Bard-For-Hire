using System.Collections.Generic;
using UnityEngine;

public class NoteInventory : MonoBehaviour
{
    // Inventory Panel
    private Transform inventoryPanel;

    // Prefabs for Spawning
    private GameObject noteSlotPrefab;
    private GameObject notePrefab;


    [SerializeField] private int inventorySpace = 10;
    

    private List<GameObject> noteInventory;

    public List<GameObject> GetNoteInventory()
    {
        return noteInventory;
    }

    private void Awake()
    {
        noteInventory = new List<GameObject>();
    }

    /// <summary>
    /// Initializing note inventory
    /// </summary>
    private void Start()
    {
        Debug.Assert(noteSlotPrefab = AssetManager.Instance.GetPrefab("NoteSlot"), "NoteInventory requires NoteSlotPrefab");
        Debug.Assert(notePrefab = AssetManager.Instance.GetPrefab("Note"), "NoteInventory requires NotePrefab");
        Debug.Assert(inventoryPanel = transform.GetChild(0), "NoteInventory requires Layout for Grid");

        for (int i = 0; i < inventorySpace; i++)
        {
            /// Could change these entirely to use the spawn functions from AssetManager
            GameObject noteSlot = Instantiate(noteSlotPrefab, inventoryPanel);
            GameObject note = Instantiate(notePrefab, noteSlot.transform);
            note.name = note.name + i.ToString();

            int noteToCreate = Random.Range(0, ResourceManager.Instance.NoteData.Length);
            NoteData data = Instantiate(ResourceManager.Instance.NoteData[noteToCreate]);

            note.GetComponent<NoteController>().noteData = data;
            note.GetComponent<NoteController>().SetSprite();

            AddNote(note);
        }

        PlayerInventoryManager.Instance.SetInventoryNotes(noteInventory);
    }

    /// <summary>
    /// Adding new notes to note inventory
    /// </summary>
    /// <param name="note">Note data to add</param>
    public void AddNote(GameObject note)
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
        if (noteInventory.Contains(note))
        {
            Debug.Log("This note is already in the inventory.");
            return;
        }
        noteInventory.Add(note);
    }
}
