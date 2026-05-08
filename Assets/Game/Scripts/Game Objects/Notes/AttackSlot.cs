using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AttackSlot : MonoBehaviour, IDropHandler
{
    public List<GameObject> storedObjects;

    public void OnDrop(PointerEventData eventData)
    {
        // Obtains the information for the object dropped into the slot
        GameObject dropped = eventData.pointerDrag;

        dropped.transform.SetParent(transform);
        storedObjects.Add(dropped);
    }
}
