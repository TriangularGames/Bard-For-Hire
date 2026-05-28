using System.Collections.Generic;
using UnityEngine;

public class ShopSection : MonoBehaviour
{
    [Tooltip("Slots where the Items will be displayed")]
    protected List<GameObject> displaySlots;

    public virtual void Awake()
    {
        displaySlots = new List<GameObject>();
    }

    public virtual void Start()
    {
        // Get the Item display slots on this Object
        for (int i = 0; i < transform.childCount; i++)
        {
            displaySlots.Add(transform.GetChild(i).gameObject);
        }
    }

    public List<GameObject> GetSlots()
    {
        return displaySlots;
    }
}
