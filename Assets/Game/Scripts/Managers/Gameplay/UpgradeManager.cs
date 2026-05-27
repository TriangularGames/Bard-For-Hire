using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    private List<UpgradeData> Upgrades;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Upgrades.Clear();
        Upgrades.AddRange(PlayerManager.Instance.upgradeInventory);
    }

    public void AddUpgrade(UpgradeData upgrade)
    {
        Upgrades.Add(upgrade);
    }

    public bool HasUpgrade(UpgradeID id)
    {
        for (int i = 0; i < Upgrades.Count; i++)
        {
            if (Upgrades[i].UpgradeID == id)
            {
                return true;
            }
        }

        return false;
    }


    public void ClearUpgrades()
    {
        Upgrades.Clear();
    }
}
