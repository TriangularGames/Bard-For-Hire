using UnityEngine;
using static UnityEditor.AnimationUtility;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Info")]
    [SerializeField] public string ItemName;
    [SerializeField] public string ItemDescription;
    [Tooltip("The coin amount to purchase this item.")]
    [SerializeField] public int cost;
    [SerializeField] public int target = 1;

    [Header("Item Descriptors")]
    [SerializeField] public ItemType ItemType;
    [SerializeField] public ObjectRarity Rarity;

    [Header("Weapon Bonus")]
    [SerializeField] public WeaponBonus weaponBonus = WeaponBonus.None;
    [SerializeField] public int bonusDamageStacks = 0;

    [Header("Damage Value")]
    [SerializeField] public int Damage;
    [Tooltip("If the damage is a multiplier")]
    [SerializeField] public bool Mult;

    [Header("Playable Value")]
    [SerializeField] public int Playable;

    [Header("Visual Data")]
    [SerializeField] public Sprite icon;
    
}
