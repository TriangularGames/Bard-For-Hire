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
        // Set the data for this shop section
        _data.AddRange(itemList);

        // Setup the slots to display data
        int i = 0;
        foreach (GameObject slot in displaySlots)
        {
            slot.GetComponent<ItemShopSlot>().SetupSlotInfo(_data[i]);
            i++;
        }
    }

    void Start()
    {
        // Get the Item display slots on this Object
        for (int i = 0; i < transform.childCount; i++)
        {
            displaySlots.Add(transform.GetChild(i).gameObject);
        }
    }
}
