using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour
{
    // Data for what note/upgrade is in it
    [SerializeField] TMP_Text value;
    [SerializeField] Button buy;

    public void SetupSlotInfo()
    {
        value.text = "";
        // setup visual display as well
    }

    public void Purchase()
    {
        // if item is a note, add to inventory
        // if item is an upgrade, add to upgrade list (?)
    }
}
