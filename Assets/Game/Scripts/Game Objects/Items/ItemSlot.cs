using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public List<GameObject> storedObjects;

    public void RemoveObject(GameObject obj)
    {
        storedObjects.Remove(obj);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped on " + transform.parent.name);
        // Obtains the information for the object dropped into the slot
        GameObject dropped = eventData.pointerDrag;

        dropped.transform.SetParent(transform);
        if (transform.parent.name == "ItemPool")
        {
            dropped.GetComponent<Drag>().inItemPool = true;
        }
        else
        {
            dropped.GetComponent<Drag>().inItemPool = false;
        }
        storedObjects.Add(dropped);
        dropped.transform.SetParent(transform);
    }
}
