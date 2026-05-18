using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    public List<UpgradeData> Upgrades = new List<UpgradeData>();


    private void Awake()
    {
        Instance = this;
    }

    public void AddUpgrade(UpgradeData upgrade)
    {
        Upgrades.Add(upgrade);

    }

    public bool HasUpgrade(string name)
    {
        for (int i = 0; i < Upgrades.Count; i++)
        {
            if (Upgrades[i].name == name) return true;
        }
        return false;
    }

    public void ClearUpgrades()
    {
        Upgrades.Clear();
    }
}
