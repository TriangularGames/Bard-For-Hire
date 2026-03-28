using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    public Canvas canvas;
    [HideInInspector] public Transform parentAfterDrag;

    MusicSheet musicSheet;
    
    private void Awake()
    {
        musicSheet = transform.GetComponentInParent<MusicSheet>();
        Debug.Log("Music sheet found: " + musicSheet != null);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Picks up item from Sheet to drag
        parentAfterDrag = transform.parent;
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        // Removes from Music Sheet List
        musicSheet.RemoveNote(GetComponent<BaseNote>());
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Adds item to slot
        transform.SetParent(parentAfterDrag);
        image.raycastTarget = true;

        // Adds Note to Music Sheet List
        musicSheet.AddNote(GetComponent<BaseNote>());
    }
}
