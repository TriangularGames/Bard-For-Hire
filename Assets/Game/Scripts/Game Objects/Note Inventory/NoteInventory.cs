using System.Collections.Generic;
using UnityEngine;

public class NoteInventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 10;
    private List<INote> notes;

    private void Awake()
    {
        notes = new List<INote>();   
    }

    public void AddNote(INote note)
    {
        notes.Add(note);
    }

    public void RemoveNote(INote note)
    {
        notes.Remove(note);
    }
}