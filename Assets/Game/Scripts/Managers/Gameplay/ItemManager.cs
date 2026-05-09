using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : Singleton<ItemManager>
{
    public List<GameObject> ItemsToDelete;

    public ItemPool itemPool;
    public AttackHand attackHand;

    private int itemsDiscarded = 0;

    private void Start()
    {
        ItemsToDelete = new List<GameObject>();
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
                if (itemPool.GetItemPool().Contains(ItemsToDelete[i].GetComponent<ItemController>().itemData) || attackHand.GetItemList().Contains(ItemsToDelete[i].GetComponent<ItemController>().itemData))
                {
                    // Deselect the Slot
                    ItemsToDelete[i].transform.parent.GetComponent<RawImage>().color = Color.white;

                    // Remove the Note from it's respective slot
                    if (ItemsToDelete[i].GetComponent<Drag>().inItemPool)
                    {
                        itemPool.RemoveItem(ItemsToDelete[i].GetComponent<ItemController>().itemData);
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
        for (int i = 0; i < itemPool.inventoryPanel.childCount; i++)
        {
            if (itemsDiscarded != 0)
            {
                itemPool.InstantiateItem(PlayerManager.Instance.GetRandomItem(), itemPool.inventoryPanel.transform);
                itemsDiscarded--;
            }
            else
            {
                return;
            }
        }
    }

    /// <summary>
    /// Remove Items from AttackHand and return them to the ItemPool
    /// </summary>
    public void ClearNotes()
    {
        foreach (GameObject item in attackHand.GetItems())
        {
            for (int i = 0; i < itemPool.inventoryPanel.childCount; i++)
            {
                if (itemPool.inventoryPanel.GetChild(i).GetComponent<ItemSlot>().storedObjects.Count == 0)
                {
                    //item.GetComponent<DraggableItem>().parentAfterDrag.GetComponent<ItemSlot>().ClearItems();
                    //item.GetComponent<DraggableItem>().parentAfterDrag = itemPool.inventoryPanel.GetChild(i).transform;
                    item.transform.SetParent(itemPool.inventoryPanel.GetChild(i));
                    
                    
                    //itemPool.inventoryPanel.GetChild(i).GetComponent<ItemSlot>().AddItem(item);
                }
            }
        }
    }
}
