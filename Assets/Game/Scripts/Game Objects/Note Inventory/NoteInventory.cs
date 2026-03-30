using System.Collections.Generic;
using UnityEngine;

public class NoteInventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 10;
    [SerializeField] private GameObject noteUIPrefab;
    [SerializeField] private Transform inventoryPanel;

    private List<BaseNote> Notes;

    private void Awake()
    {
        Notes = new List<BaseNote>();
    }

    /// <summary>
    /// Initializing note pool
    /// </summary>
    private void Start()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject obj = Instantiate(noteUIPrefab, inventoryPanel);
            
            BaseNote note = obj.GetComponent<BaseNote>();
            Notes.Add(note);
        }
    }

    /// <summary>
    /// Adding new notes to note pool
    /// </summary>
    /// <param name="note"></param>
    public void AddNote(BaseNote note)
    {
        GameObject obj = Instantiate(noteUIPrefab, inventoryPanel);

        if (note == null)
        {
            Debug.LogWarning("Tried to add a null note.");
            return;
        }
        if (Notes.Count >= maxSlots)
        {
            Debug.Log("Inventory is full.");
            return;
        }
        if (Notes.Contains(note))
        {
            Debug.Log("This note is already in the inventory.");
            return;
        }
        Notes.Add(note);
    }

   /// <summary>
   /// Removing notes from note pool when player discards
   /// </summary>
   /// <param name="note"></param>
    public void RemoveNote(BaseNote note)
    {
        if (note == null) return;

        if (Notes.Remove(note))
        {
            Debug.Log($"Removed {note.name} from inventory.");
        }
    }
}