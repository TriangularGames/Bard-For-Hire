using System.Collections.Generic;
using UnityEngine;

public class NoteInventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 10;
    [SerializeField] private GameObject noteUIPrefab;
    [SerializeField] private Transform inventoryPanel;

    private List<INote> notes;

    private void Awake()
    {
        notes = new List<INote>();   
    }

    public void AddNote(INote note)
    {
        notes.Add(note);
        Instantiate(noteUIPrefab, inventoryPanel);
    }

    public void RemoveNote(INote note)
    {
        notes.Remove(note);
    }
}