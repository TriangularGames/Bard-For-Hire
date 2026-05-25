using UnityEngine;

[CreateAssetMenu(fileName = "Consumable", menuName = "Scriptable Objects/Consumable")]
public class ConsumableData : ScriptableObject
{
    [Header("Consumable Info")]
    [SerializeField] public string ConsumableName;
    [SerializeField] public string ConsumableDescription;
    [Tooltip("The coin amount to purchase this consumable.")]
    [SerializeField] public int cost;

    [Header("Consumable Descriptors")]
    [SerializeField] public ConsumableID Type;
    [SerializeField] public ObjectRarity Rarity;

    [Header("Visual Data")]
    [SerializeField] public Sprite icon;
}
