using System.Collections.Generic;
using UnityEngine;

public class ShopUpgrades : ShopSection
{
    private List<UpgradeData> _data;

    public override void Awake()
    {
        base.Awake();
        _data = new List<UpgradeData>();
    }

    public List<UpgradeData> GetData()
    {
        return _data;
    }

    public void SetupSlots(List<UpgradeData> itemList)
    {
        List<GameObject> slots = new List<GameObject>();
        for (int j = 0; j < transform.childCount; j++)
        {
            slots.Add(transform.GetChild(j).gameObject);
        }

        // Setup the slots to display data
        int i = 0;
        foreach (GameObject slot in slots)
        {
            // If slot hasn't been purchased, fill out the item in the slot, and add it's data to list of Upgrades
            if (!slot.GetComponent<UpgradeShopSlot>()._Purchased)
            {
                slot.GetComponent<UpgradeShopSlot>().SetupSlotInfo(itemList[i]);
                _data.Add(itemList[i]);
            }
            i++;
        }
        
    }

    public void ClearSlots()
    {
        foreach (GameObject slot in displaySlots)
        {
            slot.GetComponent<UpgradeShopSlot>().ClearInfo();
        }
        _data.Clear();
    }
}
