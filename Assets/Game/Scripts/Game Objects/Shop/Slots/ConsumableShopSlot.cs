using UnityEngine;
using UnityEngine.UI;

public class ConsumableShopSlot : ShopSlot
{
    // Data of the Item that is in the slot
    private ConsumableData _data;

    public ConsumableData GetData()
    {
        return _data;
    }

    public void SetupSlotInfo(ConsumableData item)
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
