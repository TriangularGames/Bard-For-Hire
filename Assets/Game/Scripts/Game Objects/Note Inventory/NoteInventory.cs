using System.Collections.Generic;
using UnityEngine;

public class NoteInventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 6;
    [SerializeField] private GameObject noteSlotPrefab;
    [SerializeField] private Transform inventoryPanel;

    // This is just for testing
    [SerializeField] public List<NoteData> notesToSpawn;
    [SerializeField] public GameObject notePrefab;

    private List<NoteData> NotePool;

    private void Awake()
    {
        NotePool = new List<NoteData>();
    }

    /// <summary>
    /// Initializing note pool
    /// </summary>
    private void Start()
    {
        if (noteSlotPrefab == null || inventoryPanel == null) return;

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject obj = Instantiate(noteSlotPrefab, inventoryPanel);

            GameObject note = Instantiate(notePrefab, obj.transform);

            int noteToCreate = Random.Range(0, notesToSpawn.Count);
            note.GetComponent<NoteController>().noteData = notesToSpawn[noteToCreate];

            note.GetComponent<NoteController>().SetSprite();

            AddNote(note.GetComponent<NoteController>().noteData);
        }
    }

    /// <summary>
    /// Adding new notes to note pool
    /// </summary>
    /// <param name="note"></param>
    public void AddNote(NoteData note)
    {
        if (noteSlotPrefab == null || inventoryPanel == null) return;

        if (note == null)
        {
            Debug.LogWarning("Tried to add a null note.");
            return;
        }
        if (NotePool.Count >= maxSlots)
        {
            Debug.Log("Inventory is full.");
            return;
        }
        if (NotePool.Contains(note))
        {
            Debug.Log("This note is already in the inventory.");
            return;
        }
        NotePool.Add(note);
    }

   /// <summary>
   /// Removing notes from note pool when player discards
   /// </summary>
   /// <param name="note"></param>
    public void RemoveNote(NoteData note)
    {
        if (note == null) return;

        if (NotePool.Contains(note))
        {
            Debug.Log("Removed from NotePool.");
            NotePool.Remove(note);
        }
    }

    /// <summary>
    /// Clear all notes from note pool
    /// </summary>
    public void ClearNotes()
    {
        //TODO: redo this
    }
}