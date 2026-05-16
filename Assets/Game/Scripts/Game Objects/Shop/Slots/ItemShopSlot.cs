using Unity.VisualScripting;
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
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }

    public override void ClearInfo()
    {
        base.ClearInfo();
        _data = null;
    }

    public override void Purchase()
    {
        // Subtract money from player
        EventBus.Publish(new PurchaseEvent(int.Parse(value.text)));
        EventBus.Publish(new ItemBoughtEvent(_data));
        _Purchased = true;
        ClearInfo();
    }
}
