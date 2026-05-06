using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    public Image image;
    [SerializeField] public Transform parentAfterDrag;
    public bool isSelected = false;
    public bool inNotePool = false;
    
    private void Awake()
    {
        // Check if item is in a slot
        if (gameObject.transform.parent.gameObject.GetComponent<NoteSlot>() != null)
        {
            parentAfterDrag = gameObject.transform.parent.transform;
            parentAfterDrag.gameObject.GetComponent<NoteSlot>().SetNote(gameObject);
        }
    }

    #region Dragging Item
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Deselect Note
        DeselectNote();
        
        // Picks up item from Sheet to drag
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;
        transform.GetChild(0).GetComponent<TMP_Text>().raycastTarget = false;
        transform.GetChild(1).GetComponent<TMP_Text>().raycastTarget = false;

        parentAfterDrag.GetComponent<NoteSlot>().ClearNote();

        // If Note is in MusicSheet, Remove the Note from it and Clear the Slot
        if (parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>().RemoveNote(GetComponent<NoteController>().noteData);
            parentAfterDrag.GetComponent<NoteSlot>().ClearNote();
        }

        // If Note is in NotePool, Remove the Note from it and Clear the Slot
        if (parentAfterDrag.parent.transform.parent.GetComponent<NotePool>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<NotePool>().RemoveNote(GetComponent<NoteController>().noteData);
            parentAfterDrag.GetComponent<NoteSlot>().ClearNote();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Adds item to slot
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;

        parentAfterDrag.GetComponent<NoteSlot>().SetNote(gameObject);

        // If NoteSlot is now in the MusicSheet, Add it to the NotePool and Parent it to the Slot
        if (parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>().AddNote(GetComponent<NoteController>().noteData);
            parentAfterDrag.GetComponent<NoteSlot>().SetNote(gameObject);
        }

        // If NoteSlot is now in the NotePool, Add it to the NotePool and Parent it to the Slot
        if (parentAfterDrag.parent.transform.parent.GetComponent<NotePool>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<NotePool>().AddNote(GetComponent<NoteController>().noteData);
            parentAfterDrag.GetComponent<NoteSlot>().SetNote(gameObject);
        }
    }
    #endregion

    #region Highlight and Select Item
    // Highlight the item when mouse is hovered
    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = Color.lightPink;
    }

    // Change Slot colour to showcase item is selected/unselected
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isSelected)
        {
            parentAfterDrag.GetComponent<RawImage>().color = Color.blue;
            isSelected = true;
            NoteManager.Instance.notesToDelete.Add(gameObject);
            if (parentAfterDrag.parent.transform.parent.GetComponent<NotePool>())
            {
                inNotePool = true;
            }
            else
            {
                inNotePool = false;
            }
        }
        else
        {
            DeselectNote();
        }
    }

    /// <summary>
    /// Deselect current Note
    /// </summary>
    private void DeselectNote()
    {
        parentAfterDrag.GetComponent<RawImage>().color = Color.white;
        isSelected = false;
        if (NoteManager.Instance.notesToDelete.Contains(gameObject))
        {
            NoteManager.Instance.notesToDelete.Remove(gameObject);
        }
    }

    // Unhighlights the item when mouse is moved off
    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
    }
    #endregion
}
