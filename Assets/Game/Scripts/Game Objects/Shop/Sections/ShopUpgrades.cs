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
        // Set the data for this shop section
        _data.AddRange(itemList);

        // Setup the slots to display data
        int i = 0;
        foreach (GameObject slot in displaySlots)
        {
            slot.GetComponent<UpgradeShopSlot>().SetupSlotInfo(_data[i]);
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
