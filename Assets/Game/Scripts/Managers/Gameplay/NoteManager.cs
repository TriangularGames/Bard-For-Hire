using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class NoteManager : Singleton<NoteManager>
{
    public List<GameObject> notesToDelete;

    public NotePool notePool;
    public MusicSheet musicSheet;

    private int notesDiscarded = 0;

    private void Start()
    {
        notesToDelete = new List<GameObject>();
    }

    /// <summary>
    /// When discarding notes, destroy them.
    /// </summary>
    public void DiscardNote()
    {
        notesDiscarded = 0;
        if (PlayerManager.Instance.notesNotUsed.Count != 0)
        {
            for (int i = 0; i < notesToDelete.Count; i++)
            {
                // Checks if the Notes to Delete is in the NotePool or the MusicSheet
                if (notePool.GetNotePool().Contains(notesToDelete[i].GetComponent<NoteController>().noteData) || musicSheet.GetNoteList().Contains(notesToDelete[i].GetComponent<NoteController>().noteData))
                {
                    // Deselect the Slot
                    notesToDelete[i].transform.parent.GetComponent<RawImage>().color = Color.white;

                    // Remove the Note from it's respective slot
                    if (notesToDelete[i].GetComponent<DraggableItem>().inNotePool)
                    {
                        notePool.RemoveNote(notesToDelete[i].GetComponent<NoteController>().noteData);
                    }
                    else
                    {
                        musicSheet.RemoveNote(notesToDelete[i].GetComponent<NoteController>().noteData);
                    }

                    // Clear the Note from the slot
                    notesToDelete[i].transform.parent.GetComponent<ItemSlot>().ClearNote();
                    // Notify that a Note has been removed
                    EventBus.Publish<NoteRemovedEvent>(new NoteRemovedEvent(notesToDelete[i].GetComponent<NoteController>().noteData));

                    GameObject remove = notesToDelete[i];
                    notesToDelete.Remove(remove);
                    Destroy(remove);
                    notesDiscarded++;
                }
            }
            GrabNewNotes();
        }
        else
        {
            Debug.Log("Note Inventory is Empty! Cannot Discard.");
        }
    }

    /// <summary>
    /// When round is over, or Notes are discarded- get new notes from the inventory to take their place
    /// </summary>
    public void GrabNewNotes()
    {
        for (int i = 0; i < notePool.inventoryPanel.childCount; i++)
        {
            if (notesDiscarded != 0)
            {
                if (notePool.inventoryPanel.GetChild(i).GetComponent<ItemSlot>().StoredNote == null)
                {
                    notePool.InstantiateNote(PlayerManager.Instance.GetRandomNote(), notePool.inventoryPanel.GetChild(i).transform);
                    notesDiscarded--;
                }
            }
        }
    }

    /// <summary>
    /// Remove Notes from Music Sheet and return them to the Note Pool
    /// </summary>
    public void ClearNotes()
    {
        foreach (GameObject note in musicSheet.GetNotes())
        {
            for (int i = 0; i < notePool.inventoryPanel.childCount; i++)
            {
                if (notePool.inventoryPanel.GetChild(i).GetComponent<ItemSlot>().StoredNote == null)
                {
                    note.GetComponent<DraggableItem>().parentAfterDrag.GetComponent<ItemSlot>().ClearNote();
                    note.GetComponent<DraggableItem>().parentAfterDrag = notePool.inventoryPanel.GetChild(i).transform;
                    note.transform.SetParent(notePool.inventoryPanel.GetChild(i));
                    notePool.inventoryPanel.GetChild(i).GetComponent<ItemSlot>().SetNote(note);
                }
            }
        }
    }
}
