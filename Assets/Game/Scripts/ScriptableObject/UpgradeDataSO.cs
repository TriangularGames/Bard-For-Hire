using UnityEngine;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/UpgradeData")]
public class UpgradeData : ScriptableObject
{
    [Header("Upgrade Type")]
    [SerializeField] public UpgradeType UpgradeType;

    [Header("Visual Data")]
    [SerializeField] public Sprite icon;

    [Header("Purchase Cost")]
    [SerializeField] public int cost;
}
