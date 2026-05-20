using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GetComponent<ItemShopSlot>())
        {
            ItemData data = GetComponent<ItemShopSlot>().GetData();
            TooltipSystem.Instance.Show(data.ItemDescription, data.ItemName);
        }
        else if (GetComponent<UpgradeShopSlot>())
        {
            UpgradeData data = GetComponent<UpgradeShopSlot>().GetData();
            TooltipSystem.Instance.Show(data.UpgradeDescription, data.UpgradeName);
        }
        else if (GetComponent<ConsumableShopSlot>())
        {
            ConsumableData data = GetComponent<ConsumableShopSlot>().GetData();
            TooltipSystem.Instance.Show(data.ConsumableDescription, data.ConsumableName);
        }
            
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipSystem.Instance.Hide();
    }

}
