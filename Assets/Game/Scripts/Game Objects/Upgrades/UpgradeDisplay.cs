using UnityEngine;

public class UpgradeDisplay : MonoBehaviour
{
    public static UpgradeDisplay Instance;

    public UpgradePool upgradePool;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshUpgrades();
    }

    public void RefreshUpgrades()
    {
        for (int i = 0; i < upgradePool.storedObjects.Count; i++)
            Destroy(upgradePool.storedObjects[i]);
        upgradePool.storedObjects.Clear();

        foreach (UpgradeData upgrade in PlayerManager.Instance.upgradeInventory)
            upgradePool.BringEmIn(upgrade, RefreshUpgrades);
    }
}
