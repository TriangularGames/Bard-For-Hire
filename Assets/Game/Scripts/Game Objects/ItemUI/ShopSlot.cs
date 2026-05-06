using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ShopSlot : MonoBehaviour
{
    [SerializeField] public TMP_Text value;
    [SerializeField] public Button buy;

    private void Update()
    {
        if (value != null || value.text != "")
        {
            if (PlayerManager.Instance.GetCoinAmount() < int.Parse(value.text))
            {
                buy.interactable = false;
            }
        }
    }
    public abstract void SetupSlotInfo();

    public abstract void Purchase();
}
