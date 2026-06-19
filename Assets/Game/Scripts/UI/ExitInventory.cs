using UnityEngine;
using UnityEngine.EventSystems;

public class ExitInventory : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        EventBus.Publish<HideInventoryEvent>(new HideInventoryEvent());
    }
}
