using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (gameObject.activeSelf)
        {
            if (GetComponent<ItemShopSlot>() && GetComponent<ItemShopSlot>().GetData() != null)
            {
                ItemData data = GetComponent<ItemShopSlot>().GetData();
                TooltipSystem.Instance.Show(data.ItemDescription, data.ItemName, data.Damage.ToString(), data.Playable.ToString());
            }
            else if (GetComponent<UpgradeShopSlot>() && GetComponent<UpgradeShopSlot>().GetData() != null)
            {
                UpgradeData data = GetComponent<UpgradeShopSlot>().GetData();
                TooltipSystem.Instance.Show(data.UpgradeDescription, data.UpgradeName);
            }
            else if (GetComponent<UpgradeController>() && GetComponent<UpgradeController>().upgradeData != null)
            {
                UpgradeData data = GetComponent<UpgradeController>().upgradeData;
                TooltipSystem.Instance.Show(data.UpgradeDescription, data.UpgradeName);
            }
            else if (GetComponent<ConsumableShopSlot>() && GetComponent<ConsumableShopSlot>().GetData() != null)
            {
                ConsumableData data = GetComponent<ConsumableShopSlot>().GetData();
                TooltipSystem.Instance.Show(data.ConsumableDescription, data.ConsumableName);
            }
            else if (GetComponent<ConsumableController>() && GetComponent<ConsumableController>().consumableData != null)
            {
                ConsumableData data = GetComponent<ConsumableController>().consumableData;
                TooltipSystem.Instance.Show(data.ConsumableDescription, data.ConsumableName);
            }
        }
        // TODO: add for EnemyDisplay

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
