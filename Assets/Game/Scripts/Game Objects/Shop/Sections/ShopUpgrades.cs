using System.Collections.Generic;
using UnityEngine;

public class ShopUpgrades : MonoBehaviour
{
    [Tooltip("Slots where the Items will be displayed")]
    private List<GameObject> displaySlots;

    private List<UpgradeData> _data;

    private void Awake()
    {
        _data = new List<UpgradeData>();
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

    public List<UpgradeData> GetData()
    {
        return _data;
    }

    public List<GameObject> GetSlots()
    {
        return displaySlots;
    }

    public void SetupSlots(List<UpgradeData> itemList)
    {
        // Setup the slots to display data
        int i = 0;
        foreach (GameObject slot in displaySlots)
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
