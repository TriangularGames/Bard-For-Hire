using UnityEngine;
using UnityEngine.EventSystems;

public class NoteSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            // Obtains the information for the object dropped into the slot
            GameObject dropped = eventData.pointerDrag;

            // Changes the parent of the object to the current slot
            DraggableItem draggable = dropped.GetComponent<DraggableItem>();
            draggable.parentAfterDrag = transform;
        }
        else
        {
            // If there is already a Note in this slot, swap them

            // Obtains the information for the object dropped into the slot and object in this slow
            GameObject dropped = eventData.pointerDrag;
            DraggableItem draggable = dropped.GetComponent<DraggableItem>();
            GameObject current = transform.GetChild(0).gameObject;
            DraggableItem currentDraggable = current.GetComponent<DraggableItem>();

            // Changes the parent of the object to the current slot
            currentDraggable.transform.SetParent(draggable.parentAfterDrag);
            draggable.parentAfterDrag = transform;
        }
    }
}
