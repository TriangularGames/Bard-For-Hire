using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public List<UpgradeID> Upgrades = new List<UpgradeID>();


    private void Awake()
    {
        Instance = this;
    }

    public void AddUpgrade(UpgradeData upgrade)
    {
        Upgrades.Add(upgrade.UpgradeID);

    }

    public bool HasUpgrade(UpgradeID id)
    {
        return Upgrades.Contains(id);
    }

        public void ClearUpgrades()
    {
        Upgrades.Clear();
    }
}
