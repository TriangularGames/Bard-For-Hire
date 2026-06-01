using UnityEngine;

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
        icon.sprite = _data.icon;
        icon.color = new Color(1f, 1f, 1f, 1f);
        buy.gameObject.SetActive(true);
    }

    public override void ClearInfo()
    {
        base.ClearInfo();
        _data = null;
    }

    private void Update()
    {
        if (_data != null && PlayerManager.Instance.GetCoinAmount() < _data.cost)
        {
            buy.interactable = false;
        }
        else
        {
            buy.interactable = true;
        }
    }

    public override void SelectSlot(bool select)
    {
        if (_data != null)
        {
            if (PlayerManager.Instance.GetCoinAmount() < _data.cost)
            {
                buy.interactable = false;
            }
            base.SelectSlot(select);
        }
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

/// <summary>
/// Event for when an Item is purchased
/// </summary>
public struct ItemBoughtEvent
{
    public ItemData data;

    public ItemBoughtEvent(ItemData _data)
    {
        data = _data;
    }
}
