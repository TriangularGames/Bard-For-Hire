using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    [HideInInspector] public Transform parentAfterDrag;
    
    private void Awake()
    {
        parentAfterDrag = gameObject.transform.parent.transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Picks up item from Sheet to drag
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        if (parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>().RemoveNote(GetComponent<NoteController>().noteData);
        }

        if (parentAfterDrag.parent.transform.parent.GetComponent<NoteInventory>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<NoteInventory>().RemoveNote(GetComponent<NoteController>().noteData);
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

        if (parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<MusicSheet>().AddNote(GetComponent<NoteController>().noteData);
        }

        if (parentAfterDrag.parent.transform.parent.GetComponent<NoteInventory>())
        {
            parentAfterDrag.parent.transform.parent.GetComponent<NoteInventory>().AddNote(GetComponent<NoteController>().noteData);
        }
    }
}
