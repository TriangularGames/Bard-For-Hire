using System.Collections.Generic;
using UnityEngine;

public class NoteInventory : MonoBehaviour
{
    [SerializeField] private int maxSlots = 10;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform slotParent;

    private List<INote> notes;

    private void Awake()
    {
        notes = new List<INote>();   
    }

    private void Start()
    {
        for (int i = 0; i < maxSlots; i++)
        {
            Instantiate(slotPrefab, slotParent);
        }
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