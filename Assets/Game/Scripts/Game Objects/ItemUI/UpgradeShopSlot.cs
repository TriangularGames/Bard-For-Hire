using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopSlot : ShopSlot
{
    // Data of the upgrade that is in the slot
    UpgradeData data;

    public override void SetupSlotInfo()
    {
        // Setup slot data
        value.text = data.cost.ToString();
        GetComponent<Image>().sprite = data.icon;
    }

    public override void Purchase()
    {
        // Subtract money from player
        EventBus.Publish(new PurchaseEvent(int.Parse(value.text)));
    }
}
