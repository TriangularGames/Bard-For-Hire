using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class ItemManager : Singleton<ItemManager>
{
    public List<GameObject> ItemsToDelete;

    public ItemPool itemPool;
    public AttackHand attackHand;

    private int itemsDiscarded = 0;

    private void Start()
    {
        ItemsToDelete = new List<GameObject>();
        Debug.Assert(itemPool = GameObject.FindWithTag("ItemPool").GetComponent<ItemPool>(), "ItemManager requires ItemPool");
        Debug.Assert(attackHand = GameObject.FindWithTag("AttackHand").GetComponent<AttackHand>(), "ItemManager requires AttackHand");
    }

    /// <summary>
    /// When discarding Items, destroy them.
    /// </summary>
    public void DiscardNote()
    {
        itemsDiscarded = 0;
        if (PlayerManager.Instance.itemsNotUsed.Count != 0)
        {
            for (int i = 0; i < ItemsToDelete.Count; i++)
            {
                // Checks if the Notes to Delete is in the NotePool or the MusicSheet
                if (itemPool.GetItemPool().Contains(ItemsToDelete[i]) || attackHand.GetItemList().Contains(ItemsToDelete[i].GetComponent<ItemController>().itemData))
                {
                    // Deselect the Slot
                    ItemsToDelete[i].transform.parent.GetComponent<RawImage>().color = Color.white;

                    // Remove the Note from it's respective slot
                    if (ItemsToDelete[i].GetComponent<Drag>().inItemPool)
                    {
                        itemPool.RemoveItem(ItemsToDelete[i]);
                    }
                    else
                    {
                        attackHand.RemoveItem(ItemsToDelete[i]);
                    }

                    // Clear the Note from the slot
                    //notesToDelete[i].transform.parent.GetComponent<NoteSlot>().ClearNote();
                    // Notify that an Item has been removed
                    EventBus.Publish<ItemRemovedEvent>(new ItemRemovedEvent(ItemsToDelete[i].GetComponent<ItemController>().itemData));

                    GameObject remove = ItemsToDelete[i];
                    ItemsToDelete.Remove(remove);
                    Destroy(remove);
                    itemsDiscarded++;
                }
            }
            GrabNewItems();
        }
        else
        {
            Debug.Log("Item Inventory is Empty! Cannot Discard.");
        }
    }

    /// <summary>
    /// When round is over, or Items are discarded- get new Items from the inventory to take their place
    /// </summary>
    public void GrabNewItems()
    {
        Debug.Log("Getting new items!");
        if (itemPool.GetItemPool().Count != itemPool.GetMaxSlots())
        {
            itemPool.InstantiateItem(PlayerManager.Instance.GetRandomItem(), itemPool.inventoryPanel.transform);
        }
    }

    /// <summary>
    /// Remove Items from AttackHand and return them to the ItemPool
    /// </summary>
    public void ClearNotes()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject item in attackHand.GetItems())
        {
            item.GetComponent<Drag>().inItemPool = true;
            item.transform.SetParent(itemPool.transform.GetChild(0));
            toRemove.Add(item);
        }
        attackHand.RemoveAll(toRemove);
        itemPool.AddAll(toRemove);
    }
}
