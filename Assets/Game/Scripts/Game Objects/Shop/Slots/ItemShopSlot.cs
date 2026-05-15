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
}
