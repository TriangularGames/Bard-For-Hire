using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeController : MonoBehaviour
{
    public UpgradeData upgradeData;

    [SerializeField] private TMP_Text nameTxt;

    public void Setup()
    {
        SetSprite();

        SetName();
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
