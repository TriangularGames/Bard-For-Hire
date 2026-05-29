using System;
using UnityEngine;

public class UpgradePool : BaseItemContainer
{
    public void BringEmIn(UpgradeData upgrade, Action onSold = null)
    {
        GameObject upgradeSpawned = AssetManager.Instance.Spawn("Upgrade", transform);
        upgradeSpawned.GetComponent<UpgradeController>().upgradeData = upgrade;
        upgradeSpawned.GetComponent<UpgradeController>().Setup(onSold);
        upgradeSpawned.GetComponent<UpgradeController>().SetTextColor(Color.black);
        AddItem(upgradeSpawned);
    }
}
