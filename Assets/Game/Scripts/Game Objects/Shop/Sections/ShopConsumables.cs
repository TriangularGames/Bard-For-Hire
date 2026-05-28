using System.Collections.Generic;
using UnityEngine;

public class ShopConsumables : ShopSection
{
    private List<ConsumableData> _data;

    public override void Awake()
    {
        base.Awake();
        _data = new List<ConsumableData>();
    }

    public List<ConsumableData> GetData()
    {
        return _data;
    }

    public void SetupSlots(List<ConsumableData> itemList)
    {
        // Setup the slots to display data
        int i = 0;
        foreach (GameObject slot in displaySlots)
        {
            // If slot hasn't been purchased, fill out the item in the slot, and add it's data to list of Consumables
            if (!slot.GetComponent<ConsumableShopSlot>()._Purchased)
            {
                slot.GetComponent<ConsumableShopSlot>().SetupSlotInfo(itemList[i]);
                _data.Add(itemList[i]);
            }
            i++;
        }
    }

    public void ClearSlots()
    {
        foreach (GameObject slot in displaySlots)
        {
            slot.GetComponent<ConsumableShopSlot>().ClearInfo();
        }
        _data.Clear();
    }
}
