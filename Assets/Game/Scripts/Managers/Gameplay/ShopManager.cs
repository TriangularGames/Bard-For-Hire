using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] GameObject NoteLayout;
    [SerializeField] GameObject UpgradeLayout;

    [SerializeField] Button rerollBtn;

    // TODO: setup rerollCost
    public int rerollCost = 5;

    private void Awake()
    {
        SetupShop();
    }

    private void Update()
    {
        if (PlayerManager.Instance.GetCoinAmount() < rerollCost)
        {
            rerollBtn.interactable = false;
        }
    }

    public void SetupShop()
    {
        // This will generate the 5 notes and 3 upgrades available for purchase

        // TODO: figure out how it gets the data to input. Presumably it'll use the resource manager for that?

        // Setup the Note slots
        for (int i = 0; i < NoteLayout.transform.childCount; i++)
        {
            ShopSlot slot = NoteLayout.transform.GetChild(i).gameObject.GetComponent<ShopSlot>();
            // Add data to the slot
            slot.SetupSlotInfo();
        }

        // Setup the Upgrade slots
        for (int j = 0; j < UpgradeLayout.transform.childCount; j++)
        {
            ShopSlot slot = UpgradeLayout.transform.GetChild(j).gameObject.GetComponent<ShopSlot>();
            // Add data to the slot
            slot.SetupSlotInfo();
        }
    }

    public void ReRoll()
    {
        // Subtract coins from player
        EventBus.Publish(new PurchaseEvent(rerollCost));

        //Generate different notes and upgrades
    }

    public void NextRound()
    {
        // Switch to Performance scene
    }

    public void ViewInventory()
    {
        // Show a pop-up view of the inventory
    }
}
