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
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
    }

    public override void ClearInfo()
    {
        base.ClearInfo();
        _data = null;
    }

    public override void SelectSlot(bool select)
    {
        if (PlayerManager.Instance.GetCoinAmount() < _data.cost)
        {
            buy.interactable = false;
        }
        base.SelectSlot(select);        
    }

    public override void Purchase()
    {
        if (PlayerManager.Instance.upgradeInventory.Count != PlayerManager.Instance.MAXUpgrades)
        {
            // Subtract money from player
            EventBus.Publish(new PurchaseEvent(int.Parse(value.text)));
            EventBus.Publish(new UpgradeBoughtEvent(_data));
            _Purchased = true;
            ClearInfo();
        }
        else
        {
            // Indicate to player their upgrade inventory is full. or perhaps we prompt them for
            // removing one?
        }
    }
}
