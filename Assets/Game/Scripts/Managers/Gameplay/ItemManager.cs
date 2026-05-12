using System.Collections.Generic;
using UnityEngine;

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
    public void DiscardItem()
    {
        itemsDiscarded = 0;
        if (PlayerManager.Instance.itemsNotUsed.Count != 0)
        {
            for (int i = 0; i < ItemsToDelete.Count; i++)
            {
                // Checks if the Items to Delete is in the ItemPool or the AttackHand
                if (itemPool.GetItems().Contains(ItemsToDelete[i]) || attackHand.GetItems().Contains(ItemsToDelete[i]))
                {
                    // Remove the Item from it's respective slot
                    if (ItemsToDelete[i].GetComponent<Drag>().inItemPool)
                    {
                        itemPool.RemoveItem(ItemsToDelete[i]);
                    }
                    else
                    {
                        attackHand.RemoveItem(ItemsToDelete[i]);
                    }

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
        if (itemPool.GetItems().Count != itemPool.GetMaxSlots())
        {
            itemPool.InstantiateItem(PlayerManager.Instance.GetRandomItem(), itemPool.transform);
        }
    }

    /// <summary>
    /// Remove Items from AttackHand and return them to the ItemPool
    /// </summary>
    public void ClearItems()
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
