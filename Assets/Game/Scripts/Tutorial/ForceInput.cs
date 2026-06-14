using UnityEngine;
using UnityEngine.UI;

public class ForceInput : Singleton<ForceInput>
{
    private int requiredSelectionCount = 0;
    private Button attackButton;

    private void Start() // Get the item manager and attack button
    {
        ItemManager iman = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();
        attackButton = iman.attackBtn;
    }

    private void Update() // Checks if selection amount is the correct amount for the tutorial stuff
    {
        if (requiredSelectionCount <= 0 || attackButton == null) return;

        ItemManager iman = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();
        if (iman == null) return;

        bool met = iman.ItemsSelected.Count >= requiredSelectionCount;
        attackButton.interactable = met;
    }

    public void RequireSelectionCount(int count) // The selection amount required for tutorial
    {
        requiredSelectionCount = count;
    }

    public void ClearRequirements() // Remove those requirements
    {
        requiredSelectionCount = 0;
    }
}
