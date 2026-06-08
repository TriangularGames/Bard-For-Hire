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
        icon.color = Color.white;
        buy.gameObject.SetActive(true);
    }

    public override void ClearInfo()
    {
        base.ClearInfo();
        _data = null;
    }

    private void Update()
    {
        if (_data != null && (PlayerManager.Instance.GetCoinAmount() < _data.cost || PlayerManager.Instance.consumableInventory.Count == PlayerManager.Instance.MAXConsumables))
        {
            buy.interactable = false;
        }
        else
        {
            buy.interactable = true;
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

/// <summary>
/// Event for when a Consumable is purchased
/// </summary>
public struct ConsumableBoughtEvent
{
    public ConsumableData data;

    public ConsumableBoughtEvent(ConsumableData _data)
    {
        data = _data;
    }
}
