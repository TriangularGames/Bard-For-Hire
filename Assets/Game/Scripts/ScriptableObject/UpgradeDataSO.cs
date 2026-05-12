using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    [Header("Upgrade Info")]
    [SerializeField] public string UpgradeName;
    [SerializeField] public string UpgradeDescription;
    [Tooltip("The coin amount to purchase this upgrade.")]
    [SerializeField] public int cost;

    [Header("Upgrade Descriptors")]
    [SerializeField] public UpgradeType UpgradeType;
    [SerializeField] public ObjectRarity Rarity;

    [Header("Visual Data")]
    [SerializeField] public Sprite icon;
}
