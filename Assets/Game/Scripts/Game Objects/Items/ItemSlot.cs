using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public List<GameObject> storedObjects;

    public int limit = 0;

    public void RemoveObject(GameObject obj)
    {
        storedObjects.Remove(obj);
    }

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped on " + transform.parent.name);

        // Obtains the information for the object dropped into the slot
        GameObject dropped = eventData.pointerDrag;

        // If the ItemSlot has available space
        if (storedObjects.Count < limit)
        {
            // Sets the Parent to the ItemSlot it's dropped on
            dropped.transform.SetParent(transform);

            // Check if the item is in the ItemPool or AttackHand
            if (transform.parent.name == "ItemPool")
            {
                // If already in the ItemPool, reset its position
                if (dropped.GetComponent<Drag>().inItemPool)
                {
                    dropped.GetComponent<Drag>().ResetPosition();
                }
                else
                {
                    dropped.GetComponent<Drag>().inItemPool = true;
                }
            }
            else
            {
                // If already in the AttackHand, reset its position
                if (!dropped.GetComponent<Drag>().inItemPool)
                {
                    dropped.GetComponent<Drag>().ResetPosition();
                }
                else
                {
                    dropped.GetComponent<Drag>().inItemPool = false;
                }
            }

            // Add dropped object to this ItemSlot
            storedObjects.Add(dropped);
            dropped.transform.SetParent(transform);
        }
        else
        {
            // Reset Position if there is no room
            dropped.GetComponent<Drag>().ResetPosition();
        }
    }
}
