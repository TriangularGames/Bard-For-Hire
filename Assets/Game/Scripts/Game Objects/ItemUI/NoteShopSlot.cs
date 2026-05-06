using UnityEngine;
using UnityEngine.UI;

public class NoteShopSlot : ShopSlot
{
    // Data of the note that is in the slot
    NoteData data;

    public override void SetupSlotInfo()
    {
        value.text = data.cost.ToString();
        GetComponent<Image>().sprite = data.icon;
    }

    public override void Purchase()
    {
        // Subtract money from player
        EventBus.Publish(new PurchaseEvent(int.Parse(value.text)));
    }
}
