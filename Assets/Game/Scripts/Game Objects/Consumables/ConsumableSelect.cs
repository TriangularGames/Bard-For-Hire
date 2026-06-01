using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableSelect : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    private Image selection;

    private ConsumableController consumableController;

    void Awake()
    {
        selection = transform.GetChild(1).GetComponent<Image>();

        consumableController = GetComponent<ConsumableController>();

        selection.color = new Color(1f, 1f, 1f, 0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        selection.color = new Color(0f, 0f, 1f, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selection.color = new Color(1f, 1f, 1f, 0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ConsumableEffectManager.Instance.UseConsumable(consumableController.consumableData);

        ConsumableManager.Instance.RemoveConsumable(consumableController.consumableData);

        PlayerManager.Instance.consumableInventory.Remove(consumableController.consumableData);

        Destroy(gameObject);
    }
}
