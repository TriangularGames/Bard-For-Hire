using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image image;
    [HideInInspector] public Transform parentAfterDrag;

    MusicSheet musicSheet;
    
    private void Awake()
    {
        musicSheet = transform.GetComponentInParent<MusicSheet>();
        Debug.Assert(musicSheet != null, "Music Sheet was not found.");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Picks up item from Sheet to drag
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
        transform.SetAsLastSibling();
        image.raycastTarget = false;

        // Removes from Music Sheet List
        musicSheet.RemoveNote(GetComponent<BaseNote>());
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

        // Adds Note to Music Sheet List
        musicSheet.AddNote(GetComponent<BaseNote>());
    }
}
