using System.Collections.Generic;
using UnityEngine;

public class ShopItems : MonoBehaviour
{
    [Tooltip("Slots where the Items will be displayed")]
    private List<GameObject> displaySlots;

    private List<ItemData> _data;

    private void Awake()
    {
        _data = new List<ItemData>();
        displaySlots = new List<GameObject>();
    }

    void Start()
    {
        // Get the Item display slots on this Object
        for (int i = 0; i < transform.childCount; i++)
        {
            displaySlots.Add(transform.GetChild(i).gameObject);
        }
    }

    public List<ItemData> GetData()
    {
        return _data;
    }

    public List<GameObject> GetSlots()
    {
        return displaySlots;
    }

    public void SetupSlots(List<ItemData> itemList)
    {
        // Setup the slots to display data
        int i = 0;
        foreach (GameObject slot in displaySlots)
        {
            if (!slot.GetComponent<ItemShopSlot>()._Purchased)
            {
                // If slot hasn't been purchased, fill out the item in the slot, and add it's data to list of Items
                slot.GetComponent<ItemShopSlot>().SetupSlotInfo(itemList[i]);
                _data.Add(itemList[i]);
            }
            i++;
        }
    }
    
    public void ClearSlots()
    {
        foreach (GameObject slot in displaySlots)
        {
            slot.GetComponent<ItemShopSlot>().ClearInfo();
        }
        _data.Clear();
    }
}
