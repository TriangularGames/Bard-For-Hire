using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // string content, string header, string rarity = "", string type = "", bool isWeakness = false, string attack = "", string roll = ""
        if (gameObject.activeSelf)
        {
            ShopSlot slot = GetComponent<ShopSlot>();
            if (slot is ItemShopSlot)
            {
                ItemShopSlot itemSlot = (ItemShopSlot)slot;
                ItemData data = itemSlot.GetData();
                if (data != null)
                {
                    TooltipSystem.Instance.Show(data.ItemDescription, data.ItemName, data.Rarity.ToString(),
                        data.ItemType.ToString(), false, data.Damage.ToString(), data.Playable.ToString());
                }
            }
            else if (slot is UpgradeShopSlot)
            {
                UpgradeShopSlot upgradeSlot = (UpgradeShopSlot)slot;
                UpgradeData data = upgradeSlot.GetData();
                if (data != null)
                {
                    TooltipSystem.Instance.Show(data.UpgradeDescription, data.UpgradeName, data.Rarity.ToString());
                }
            }
            else if (slot is ConsumableShopSlot)
            {
                ConsumableShopSlot consumableSlot = (ConsumableShopSlot)slot;
                ConsumableData data = consumableSlot.GetData();
                if (data != null)
                {
                    TooltipSystem.Instance.Show(data.ConsumableDescription, data.ConsumableName, data.Rarity.ToString());
                }
            }
            else if (GetComponent<InventorySlot>())
            {
                ItemData data = GetComponent<InventorySlot>().GetData();
                if (data != null)
                {
                    TooltipSystem.Instance.Show(data.ItemDescription, data.ItemName, data.Rarity.ToString(),
                            data.ItemType.ToString(), false, data.Damage.ToString(), data.Playable.ToString());
                }
            }
            else if (GetComponent<ItemController>())
            {
                ItemData data = GetComponent<ItemController>().itemData;
                if (data != null)
                {
                    TooltipSystem.Instance.Show("", data.ItemName, "",
                        data.ItemType.ToString(), false, data.Damage.ToString(), data.Playable.ToString());
                }
            }
            else if (GetComponent<UpgradeController>())
            {
                UpgradeData data = GetComponent<UpgradeController>().upgradeData;
                if (data != null)
                {
                    TooltipSystem.Instance.Show(data.UpgradeDescription, data.UpgradeName, data.Rarity.ToString());
                }
            }
            else if (GetComponent<ConsumableController>())
            {
                ConsumableData data = GetComponent<ConsumableController>().consumableData;
                if (data != null)
                {
                    TooltipSystem.Instance.Show(data.ConsumableDescription, data.ConsumableName, data.Rarity.ToString());
                }
            }
            else if (GetComponent<EnemyInfo>())
            {
                EnemyData data = GetComponent<EnemyInfo>().enemyData;
                if (data != null)
                {
                    TooltipSystem.Instance.Show(data.Description, data.Name, "", "", true, data.weakness.ToString());
                }
            }
            else if (GetComponent<EnemyController>())
            {
                EnemyData data = GetComponent<EnemyController>().enemyData;
                if (data != null)
                {
                    TooltipSystem.Instance.Show("", data.Name, "", "", true, data.weakness.ToString());
                }
            }
        }

    }

    private void OnDestroy()
    {
        TooltipSystem.Instance.Hide();
    }

    private void OnDisable()
    {
        TooltipSystem.Instance.Hide();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.Hide();
    }

}
