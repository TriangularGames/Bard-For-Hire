using System.Collections.Generic;
using UnityEngine;

public class NoteInventory : MonoBehaviour
{
    [SerializeField] private int inventorySpace = 10;
    [SerializeField] public Transform inventoryPanel;

    // This is just for testing
    [SerializeField] public GameObject notePrefab;

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
        if (inventoryPanel == null) return;

        for (int i = 0; i < inventorySpace; i++)
        {
            GameObject note = Instantiate(notePrefab, inventoryPanel);
            note.name = note.name + i.ToString();

            int noteToCreate = Random.Range(0, ResourceManager.Instance.NoteData.Length);
            note.GetComponent<NoteController>().noteData = ResourceManager.Instance.NoteData[noteToCreate];

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
