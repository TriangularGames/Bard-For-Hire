using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class ItemShopSlot : ShopSlot
{
    // Data of the Item that is in the slot
    private ItemData _data;

    public ItemData GetData()
    {
        return _data;
    }

    public void SetupSlotInfo(ItemData item)
    {
        _data = item;

        value.text = _data.cost.ToString();
        GetComponent<Image>().sprite = _data.icon;
    }

    public override void Purchase()
    {
        // Subtract money from player
        EventBus.Publish(new PurchaseEvent(int.Parse(value.text)));
    }

    public override void SelectSlot(bool select)
    {
        base.SelectSlot(select);
        EventBus.Publish<ItemSelectedEvent>(new ItemSelectedEvent(gameObject.GetEntityId()));
    }
}

/// <summary>
/// Event for when an Item in the shop is Selected
/// </summary>
public struct ItemSelectedEvent
{
    public EntityId id;

    public ItemSelectedEvent(EntityId _id)
    {
        id = _id;
    }
}
