using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Drag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform canvasTransform;
    private RectTransform objectTransform;

    private Vector2 _offset;

    [HideInInspector] public Vector2 originalPos;

    [HideInInspector] public bool inItemPool = false;

    private void Awake()
    {
#if UNITY_EDITOR
        Debug.Assert(canvasTransform = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>(), "Scene must contain tagged Canvas");
        Debug.Assert(objectTransform = GetComponent<RectTransform>(), "GameObject must have RectTransform");
#else
        canvasTransform = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();
        objectTransform = GetComponent<RectTransform>();
#endif
    }

    public void ResetPosition()
    {
        objectTransform.anchoredPosition = originalPos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = objectTransform.anchoredPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform, Mouse.current.position.ReadValue(), Camera.main, out var localPos);
        _offset = objectTransform.anchoredPosition - localPos;
        transform.parent.GetComponent<ItemSlot>().RemoveObject(gameObject);
        EventBus.Publish<DragEvent>(new DragEvent(gameObject));
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform, Mouse.current.position.ReadValue(), Camera.main, out var localPos);

        objectTransform.anchoredPosition = localPos + _offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ResetPosition();
    }
}

/// <summary>
/// Event for when Object is being Dragged
/// </summary>
public struct DragEvent
{
    public GameObject item;

    public DragEvent(GameObject _item)
    {
        item = _item;
    }
}
