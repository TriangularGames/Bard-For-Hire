using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotePool : MonoBehaviour
{
    [SerializeField] private int maxSlots = 6;
    [SerializeField] private GameObject noteSlotPrefab;
    [SerializeField] public Transform inventoryPanel;

    // This is just for testing
    [SerializeField] public GameObject notePrefab;

    private List<GameObject> notePool;

    public List<GameObject> GetNotePool()
    {
        return notePool;
    }

    private void Awake()
    {
        notePool = new List<GameObject>();
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
            note.name = note.name + i.ToString();

            int noteToCreate = Random.Range(0, ResourceManager.Instance.NoteData.Length);
            note.GetComponent<NoteController>().noteData = ResourceManager.Instance.NoteData[noteToCreate];

            note.GetComponent<NoteController>().SetSprite();

            AddNote(note);
        }
    }

    /// <summary>
    /// This is a temporary function for testing purposes
    /// </summary>
    public void InstantiateNote(Transform parent)
    {
        GameObject note = Instantiate(notePrefab, parent);

        int noteToCreate = Random.Range(0, ResourceManager.Instance.NoteData.Length);
        NoteData data = ResourceManager.Instance.NoteData[noteToCreate];
        note.GetComponent<NoteController>().noteData = Instantiate(data);

        note.GetComponent<NoteController>().SetSprite();

        AddNote(note);
    }

    /// <summary>
    /// Adding new notes to note pool
    /// </summary>
    /// <param name="note">Note data to add</param>
    public void AddNote(GameObject note)
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
    public void RemoveNote(GameObject note)
    {
        if (note == null) return;

        if (notePool.Contains(note))
        {
            notePool.Remove(note);
        }
    }

    /// <summary>
    /// Clear all notes from the Note Pool
    /// </summary>
    public void ClearNotes()
    {
        //TODO: redo this
    }
}