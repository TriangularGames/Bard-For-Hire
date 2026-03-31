using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoteManager : Singleton<NoteManager>
{
    public List<GameObject> notesToDelete;

    public NotePool notePool;
    public MusicSheet musicSheet;

    private void Start()
    {
        notesToDelete = new List<GameObject>();
    }
    /// <summary>
    /// When discarding notes, destroy them.
    /// </summary>
    public void DiscardNote()
    {
        for (int i = 0; i < notesToDelete.Count; i++)
        {
            if (notePool.GetNotePool().Contains(notesToDelete[i]))
            {
                notesToDelete[i].transform.parent.GetComponent<RawImage>().color = Color.white;
                if (notesToDelete[i].GetComponent<DraggableItem>().inNotePool)
                {
                    notePool.RemoveNote(notesToDelete[i]);
                }
                else
                {
                    musicSheet.RemoveNote(notesToDelete[i]);
                }
                notesToDelete[i].transform.parent.GetComponent<ItemSlot>().ClearNote();

                GameObject remove = notesToDelete[i];
                notesToDelete.Remove(remove);
                Destroy(remove);
            } 
        }
        GrabNewNotes();
    }

    /// <summary>
    /// When round is over, or Notes are discarded- get new notes from the inventory to take their place
    /// </summary>
    public void GrabNewNotes()
    {
        // THIS IS TEMPORARY
        for (int i = 0; i < notePool.inventoryPanel.childCount; i++)
        {
            if (notePool.inventoryPanel.GetChild(i).GetComponent<ItemSlot>().StoredNote == null)
            {
                notePool.InstantiateNote(notePool.inventoryPanel.GetChild(i).transform);
            }
        }
    }
}
