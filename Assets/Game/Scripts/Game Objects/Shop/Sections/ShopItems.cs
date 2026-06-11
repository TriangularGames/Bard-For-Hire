using System.Collections.Generic;
using UnityEngine;

public class ShopItems : ShopSection
{
    private List<ItemData> _data;

    public override void Awake()
    {
        base.Awake();
        _data = new List<ItemData>();
    }

    public List<ItemData> GetData()
    {
        return _data;
    }

    public void SetupSlots(List<ItemData> itemList)
    {
        List<GameObject> slots = new List<GameObject>();
        // Get the Item display slots on this Object
        for (int j = 0; j < transform.childCount; j++)
        {
            slots.Add(transform.GetChild(j).gameObject);
        }

        int i = 0;
        foreach (GameObject slot in slots)
        {
            ItemShopSlot itemSlot = slot.GetComponent<ItemShopSlot>();
            if (itemSlot != null)
            {
                itemSlot.SetupSlotInfo(itemList[i]);
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
