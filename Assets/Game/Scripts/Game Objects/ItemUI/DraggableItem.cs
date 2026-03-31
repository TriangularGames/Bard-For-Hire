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
        parentAfterDrag = gameObject.transform.parent.transform;
        parentAfterDrag.gameObject.GetComponent<ItemSlot>().SetNote(gameObject);
    }

    #region Dragging Item
    public void OnBeginDrag(PointerEventData eventData)
    {
        isSelected = false;
        parentAfterDrag.GetComponent<RawImage>().color = Color.white;
        // Picks up item from Sheet to drag
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        parentAfterDrag.GetComponent<ItemSlot>().ClearNote();

        if (parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>().RemoveNote(gameObject);
        }

        if (parentAfterDrag.parent.transform.parent.GetComponent<NotePool>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<NotePool>().RemoveNote(gameObject);
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

        parentAfterDrag.GetComponent<ItemSlot>().SetNote(gameObject);

        if (parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>().AddNote(gameObject);
        }

        if (parentAfterDrag.parent.transform.parent.GetComponent<NotePool>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<NotePool>().AddNote(gameObject);
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
            parentAfterDrag.GetComponent<RawImage>().color = Color.white;
            isSelected = false;
            if (NoteManager.Instance.notesToDelete.Contains(gameObject))
            {
                NoteManager.Instance.notesToDelete.Remove(gameObject);
            }
        }
    }

    // Unhighliights the item when mouse is moved off
    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = Color.white;
    }
    #endregion
}
