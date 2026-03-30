using System.Collections.Generic;
using UnityEngine;

public class NoteInventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 10;
    [SerializeField] private GameObject noteUIPrefab;
    [SerializeField] private Transform inventoryPanel;

    private List<BaseNote> notes;

    private void Awake()
    {
        notes = new List<BaseNote>();
    }

    private void Start()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            GameObject obj = Instantiate(noteUIPrefab, inventoryPanel);
            Debug.Log("Instantiated: " + obj.name);
        }
    }

    // Adding notes to the note pool
    public void AddNote(BaseNote note)
    {
        if (note == null)
        {
            Debug.LogWarning("Tried to add a null note.");
            return;
        }

        if (notes.Count >= maxSlots)
        {
            Debug.Log("Inventory is full.");
            return;
        }

        if (notes.Contains(note))
        {
            Debug.Log("This note is already in the inventory.");
            return;
        }

        notes.Add(note);

        GameObject newUI = Instantiate(noteUIPrefab, inventoryPanel);
    }

    // Used when discarding notes in the note pool
    public void RemoveNote(BaseNote note)
    {
        if (note == null) return;

        if (notes.Remove(note))
        {
            Debug.Log($"Removed {note.name} from inventory.");
        }
    }
}