using UnityEngine;
using UnityEngine.UI;

public class UpgradeVictoryDisplay : MonoBehaviour
{
    public UpgradeData upgradeData;

    public void Setup(UpgradeData upgrade)
    {
        upgradeData = upgrade;
        SetSprite();
    }
    private void SetSprite()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = upgradeData.icon;
    }
}
