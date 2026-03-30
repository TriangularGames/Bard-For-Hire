using System.Collections.Generic;
using UnityEngine;

public class NoteInventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 10;
    [SerializeField] private GameObject notePrefab;
    [SerializeField] private Transform inventoryPanel;

    private List<BaseNote> NotePool;

    private void Awake()
    {
        NotePool = new List<BaseNote>();
    }

    /// <summary>
    /// Initializing note pool
    /// </summary>
    private void Start()
    {
        if (notePrefab == null || inventoryPanel == null) return;

        for (int i = 0; i < maxSlots; i++)
        {
            GameObject obj = Instantiate(notePrefab, inventoryPanel);
            
            BaseNote note = obj.GetComponent<BaseNote>();
            NotePool.Add(note);
        }
    }

    /// <summary>
    /// Adding new notes to note pool
    /// </summary>
    /// <param name="note"></param>
    public void AddNote(BaseNote note)
    {
        if (notePrefab == null || inventoryPanel == null) return;

        GameObject obj = Instantiate(notePrefab, inventoryPanel);

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
    public void RemoveNote(BaseNote note)
    {
        if (note == null) return;

        if (NotePool.Remove(note))
        {
            Debug.Log($"Removed {note.name} from inventory.");
        }
    }

    /// <summary>
    /// Clear all notes from note pool
    /// </summary>
    public void ClearNotes()
    {
        foreach (var note in NotePool)
        {
            Destroy(note.gameObject);
        }

        NotePool.Clear();
    }
}