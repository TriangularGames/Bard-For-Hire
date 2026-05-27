using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private TMP_Text quantityText;
    private ItemData _data;

    public ItemData GetData()
    {
        return _data;
    }

    public void SetupSlotInfo(ItemData item, int quantity)
    {
        _data = item;

        GetComponent<Image>().sprite = _data.icon;
        GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);

        quantityText.text = "x" + quantity.ToString();
    }
}
