using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UpgradeController : MonoBehaviour
{
    public UpgradeData upgradeData;
    [SerializeField] private GameObject sellButtonObj;
    [SerializeField] private TMP_Text sellButtonText;
    [SerializeField] private TMP_Text nameTxt;
    private bool isSelected = false;
    private Action soldTimey;

    public void Setup(Action onSold = null)
    {
        soldTimey = onSold;
        sellButtonObj.SetActive(false);
        sellButtonObj?.GetComponent<Button>().onClick.AddListener(OnSellClicked);
        GetComponent<Button>()?.onClick.AddListener(OnItemClicked);
        SetSprite();
        SetName();
    }

    public void SetTextColor(Color color)
    {
        nameTxt.color = color;
    }

    public void SetSelected(bool selected)
    {
        isSelected = selected;

        sellButtonObj.SetActive(selected);
        if (selected && sellButtonText != null)
        {
            int sellValue = Mathf.Max(1, upgradeData.cost / 2);
            sellButtonText.text = $"Sell for ${sellValue}";
        }
    }

    private void OnItemClicked()
    {
        foreach (UpgradeController other in transform.parent.GetComponentsInChildren<UpgradeController>())
        {
            if (other != this) other.SetSelected(false);
        }

        SetSelected(!isSelected);
    }

    private void OnSellClicked()
    {
        int sellValue = Mathf.Max(1, upgradeData.cost / 2);
        PlayerManager.Instance.Coins += sellValue;
        PlayerManager.Instance.SetCoinText();

        PlayerManager.Instance.upgradeInventory.Remove(upgradeData);

        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.ClearUpgrades();
            foreach (UpgradeData upgrade in PlayerManager.Instance.upgradeInventory)
                UpgradeManager.Instance.AddUpgrade(upgrade);
        }

        soldTimey?.Invoke();
        Destroy(gameObject);
    }

    private void SetSprite()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = upgradeData.icon;
    }

    private void SetName()
    {
        nameTxt.text = upgradeData.UpgradeName;
    }
}
