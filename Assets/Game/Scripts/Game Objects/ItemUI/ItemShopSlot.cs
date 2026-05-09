using UnityEngine;
using UnityEngine.UI;

public class ItemShopSlot : ShopSlot
{
    // Data of the Item that is in the slot
    ItemData data;

    public override void SetupSlotInfo()
    {
        value.text = data.cost.ToString();
        GetComponent<Image>().sprite = data.icon;
    }

    public override void Purchase()
    {
        // Subtract money from player
        EventBus.Publish(new PurchaseEvent(int.Parse(value.text)));
    }
}
