using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        // string content, string header, string rarity = "", string type = "", bool isWeakness = false, string attack = "", string roll = ""
        if (gameObject.activeSelf)
        {
            if (GetComponent<ItemShopSlot>() && GetComponent<ItemShopSlot>().GetData() != null)
            {
                ItemData data = GetComponent<ItemShopSlot>().GetData();
                TooltipSystem.Instance.Show(data.ItemDescription, data.ItemName, data.Rarity.ToString(), data.ItemType.ToString(), false, data.Damage.ToString(), data.Playable.ToString());
            }
            else if (GetComponent<UpgradeShopSlot>() && GetComponent<UpgradeShopSlot>().GetData() != null)
            {
                UpgradeData data = GetComponent<UpgradeShopSlot>().GetData();
                TooltipSystem.Instance.Show(data.UpgradeDescription, data.UpgradeName, data.Rarity.ToString());
            }
            else if (GetComponent<UpgradeController>() && GetComponent<UpgradeController>().upgradeData != null)
            {
                UpgradeData data = GetComponent<UpgradeController>().upgradeData;
                TooltipSystem.Instance.Show(data.UpgradeDescription, data.UpgradeName, data.Rarity.ToString());
            }
            else if (GetComponent<ConsumableShopSlot>() && GetComponent<ConsumableShopSlot>().GetData() != null)
            {
                ConsumableData data = GetComponent<ConsumableShopSlot>().GetData();
                TooltipSystem.Instance.Show(data.ConsumableDescription, data.ConsumableName, data.Rarity.ToString());
            }
            else if (GetComponent<ConsumableController>() && GetComponent<ConsumableController>().consumableData != null)
            {
                ConsumableData data = GetComponent<ConsumableController>().consumableData;
                TooltipSystem.Instance.Show(data.ConsumableDescription, data.ConsumableName, data.Rarity.ToString());
            }
            else if (GetComponent<EnemyInfo>() && GetComponent<EnemyInfo>().enemyData != null)
            {
                EnemyData data = GetComponent<EnemyInfo>().enemyData;
                TooltipSystem.Instance.Show(data.Description, data.Name, "", "", true, data.weakness.ToString());
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
