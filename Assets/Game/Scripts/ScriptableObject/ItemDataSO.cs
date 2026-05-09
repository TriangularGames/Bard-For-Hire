using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Type")]
    [SerializeField] public ItemType ItemType;
    [SerializeField] public bool Rest;

    [Header("Scoring Value")]
    [SerializeField] public int Score;
    [SerializeField] public bool Mult;

    [Header("Playable Value")]
    [SerializeField] public int Playable;

    [Header("Visual Data")]
    [SerializeField] public Sprite icon;

    [Header("Purchase Cost")]
    [SerializeField] public int cost;
}
