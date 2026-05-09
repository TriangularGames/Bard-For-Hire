using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Drag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private RectTransform canvasTransform;
    [SerializeField] private RectTransform objectTransform;

    private Vector2 _offset;

    public Vector2 originalPos;

    public bool inItemPool = false;

    private void Awake()
    {
        canvasTransform = GameObject.FindWithTag("Canvas").GetComponent<RectTransform>();
        objectTransform = GetComponent<RectTransform>();
    }

    public void ResetPosition()
    {
        objectTransform.anchoredPosition = originalPos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPos = objectTransform.anchoredPosition;
        Debug.Log("Dragging");
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform, Mouse.current.position.ReadValue(), Camera.main, out var localPos);
        _offset = objectTransform.anchoredPosition - localPos;
        transform.parent.GetComponent<ItemSlot>().RemoveObject(gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasTransform, Mouse.current.position.ReadValue(), Camera.main, out var localPos);

        objectTransform.anchoredPosition = localPos + _offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Dropping");
    }
}
