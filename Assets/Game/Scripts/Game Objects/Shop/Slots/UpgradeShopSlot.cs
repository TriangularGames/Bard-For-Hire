using UnityEngine;

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

        value.text = "$" + _data.cost.ToString();
        icon.sprite = _data.icon;
        icon.color = new Color(0f, 0f, 0f, 1f);
        buy.gameObject.SetActive(true);
    }

    public override void ClearInfo()
    {
        base.ClearInfo();
        _data = null;
    }

    private void Update()
    {
        if (_data != null && (PlayerManager.Instance.GetCoinAmount() < _data.cost || PlayerManager.Instance.upgradeInventory.Count == PlayerManager.Instance.MAXUpgrades))
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
        EventBus.Publish(new PurchaseEvent(_data.cost));
        EventBus.Publish(new UpgradeBoughtEvent(_data));
        _Purchased = true;
        ClearInfo();
    }
}

/// <summary>
/// Event for when an Upgrade is purchased
/// </summary>
public struct UpgradeBoughtEvent
{
    public UpgradeData data;

    public UpgradeBoughtEvent(UpgradeData _data)
    {
        data = _data;
    }
}
