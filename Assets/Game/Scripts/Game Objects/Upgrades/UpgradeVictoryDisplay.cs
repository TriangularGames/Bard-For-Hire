using UnityEngine;
using UnityEngine.UI;

public class UpgradeVictoryDisplay : MonoBehaviour
{
    public void Setup(UpgradeData upgrade)
    {
        SetSprite(upgrade);
    }
    private void SetSprite(UpgradeData upgrade)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = upgrade.icon;
    }
}
