using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<ItemShopSlot>())
        {
            ItemData data = GetComponent<ItemShopSlot>().GetData();
            TooltipSystem.Instance.Show(data.ItemDescription, data.ItemName, data.Damage.ToString(), data.Playable.ToString());
        }
        else if (GetComponent<UpgradeShopSlot>())
        {
            UpgradeData data = GetComponent<UpgradeShopSlot>().GetData();
            TooltipSystem.Instance.Show(data.UpgradeDescription, data.UpgradeName, "");
        }
        else if (GetComponent<ConsumableShopSlot>())
        {
            ConsumableData data = GetComponent<ConsumableShopSlot>().GetData();
            TooltipSystem.Instance.Show(data.ConsumableDescription, data.ConsumableName, "");
        }
        // TODO: add for EnemyDisplay
            
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.Hide();
    }

}
