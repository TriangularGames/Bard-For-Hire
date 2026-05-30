using UnityEngine;

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
        icon.sprite = _data.icon;
        icon.color = new Color(0.5f, 0.4f, 0.06f, 1f);
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
    }

    public override void SelectSlot(bool select)
    {
        if (_data != null)
        {
            if (PlayerManager.Instance.GetCoinAmount() < _data.cost
                || PlayerManager.Instance.consumableInventory.Count == PlayerManager.Instance.MAXConsumables)
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
        EventBus.Publish(new ConsumableBoughtEvent(_data));
        _Purchased = true;
        ClearInfo();
    }

    
}
