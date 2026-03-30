using System.Collections.Generic;
using UnityEngine;

public class NotePool : MonoBehaviour
{
    [SerializeField] private int maxSlots = 6;
    [SerializeField] private GameObject noteSlotPrefab;
    [SerializeField] private Transform inventoryPanel;

    // This is just for testing
    [SerializeField] public GameObject notePrefab;

    private List<NoteData> notePool;

    private void Awake()
    {
        notePool = new List<NoteData>();
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

            int noteToCreate = Random.Range(0, ResourceManager.Instance.NoteData.Length);
            note.GetComponent<NoteController>().noteData = ResourceManager.Instance.NoteData[noteToCreate];

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
        if (notePool.Count >= maxSlots)
        {
            Debug.Log("Inventory is full.");
            return;
        }
        if (notePool.Contains(note))
        {
            Debug.Log("This note is already in the inventory.");
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
            Debug.Log("Removed from NotePool.");
            notePool.Remove(note);
        }
    }

    /// <summary>
    /// When discarding notes, destroy them.
    /// </summary>
    /// <param name="note">Note to be destroyed</param>
    public void DeleteNote(NoteData note)
    {
        // This func also lets the Inventory Manager know that whatever note is removed
        // is not available to be used.
        // Perhaps Inventory Manager has a list of all Notes player has, all playable notes, and all notes already used/discarded? - Nat
        GrabNewNotes();
    }

    /// <summary>
    /// When round is over, or Notes are discarded- get new notes to take their place
    /// </summary>
    public void GrabNewNotes()
    {

    }

    /// <summary>
    /// Clear all notes from note pool
    /// </summary>
    public void ClearNotes()
    {
        //TODO: redo this
    }
}