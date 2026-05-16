using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IDropHandler
{
    public List<GameObject> storedObjects;

    public int limit = 0;

    [Tooltip("The distance the held object must be from an object within the slot to be Swapped")]
    public float SwapThreshold = 0.03f;

    [Tooltip("The distance the held object must be from the slot to be added to")]
    public float DropThreshold = 0.2f;

    public void RemoveObject(GameObject obj)
    {
        storedObjects.Remove(obj);
    }

    private bool isDropOverItem(GameObject _dropped)
    {
        // Check what item in list object is over
        foreach (GameObject item in storedObjects)
        {
            float dis = Vector3.Distance(item.transform.position, _dropped.transform.position);
            if (dis < SwapThreshold)
            {
                return true;
            }
        }
        return false;
    }

    private Transform OverSlot(GameObject _dropped)
    {
        float disPool = Vector3.Distance(_dropped.transform.position, GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().itemPool.transform.position);
        //float disHand = Vector3.Distance(_dropped.transform.position, GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().attackHand.transform.position);

        if (disPool <= DropThreshold)
        {
            return GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().itemPool.transform;
        }
        else
        {
            return null;
        }
    }

    private void Swap(GameObject _dropped, Transform _slot)
    {
        GameObject swap = null;

        // Check if this slot is
        if (_slot.name == "AttackHand")
        {
            // Check what item in list object is over
            foreach (GameObject item in _slot.GetComponent<ItemSlot>().storedObjects)
            {
                // TODO: SwapThreshold is at times a bit finicky, I think this'll need to be readjusted eventually
                float dis = Vector3.Distance(item.transform.position, _dropped.transform.position);
                Debug.Log("Dist between item and dropped: " + dis.ToString());
                if (dis < SwapThreshold)
                {
                    Debug.Log("Object within Swap Range!");
                    swap = item;
                    break;
                }
            }

            Debug.Log("Item to swap: " + swap.name);

            if (swap != null)
            {
                // Swap storedObject lists
                storedObjects.Remove(swap);
                storedObjects.Add(_dropped);
                swap.GetComponent<Drag>().inItemPool = true;
                _dropped.GetComponent<Drag>().inItemPool = false;

                // Swap originalPos
                Vector2 pos = swap.GetComponent<Drag>().originalPos;
                swap.GetComponent<Drag>().originalPos = _dropped.GetComponent<Drag>().originalPos;
                _dropped.GetComponent<Drag>().originalPos = pos;

                // Get the index of swap
                int swapIndex = -1;
                foreach (Transform g in transform.GetComponentInChildren<Transform>())
                {
                    swapIndex++;
                    if (g.gameObject.GetEntityId() == swap.gameObject.GetEntityId())
                    {
                        break;
                    }
                }

                // Get the index of dropped
                int droppedIndex = -1;
                foreach (Transform g in _dropped.GetComponentInParent<Transform>())
                {
                    droppedIndex++;
                    if (g.gameObject.GetEntityId() == _dropped.gameObject.GetEntityId())
                    {
                        break;
                    }
                }

                swap.transform.SetParent(_dropped.transform.parent);
                swap.transform.SetSiblingIndex(droppedIndex);

                _dropped.transform.SetParent(_slot);
                _dropped.transform.SetSiblingIndex(swapIndex);

            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Obtains the information for the object dropped into the slot
        GameObject dropped = eventData.pointerDrag;

        Transform slot = OverSlot(dropped);

        // If the ItemSlot has available space
        if (slot.GetComponent<ItemSlot>().storedObjects.Count < limit)
        {
            if (isDropOverItem(dropped)) { Swap(dropped, slot); return; }
            
            // Check what this slot is
            if (slot.name == "ItemPool")
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
            slot.GetComponent<ItemSlot>().storedObjects.Add(dropped);
            dropped.transform.SetParent(slot);
        }
        else
        {
            if (isDropOverItem(dropped))
            {
                // If the storedObjects list is full, attempt to swap
                Swap(dropped, slot);
            }
            else
            {
                // Reset Position if there is no room
                dropped.GetComponent<Drag>().ResetPosition();
            }
        }
    }
}
