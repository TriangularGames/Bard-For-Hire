using UnityEngine;
using UnityEngine.UI;

public class UpgradeShopSlot : ShopSlot
{
    // Data of the upgrade that is in the slot
    UpgradeData _data;

    public UpgradeData GetData()
    {
        return _data;
    }

    public void SetupSlotInfo(UpgradeData item)
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
