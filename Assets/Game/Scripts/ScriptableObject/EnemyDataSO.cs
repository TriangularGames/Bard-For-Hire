using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    [SerializeField] public int baseStat;

    [Header("Member Info")]
    [SerializeField] Image icon;
    [SerializeField] string Description;

    [Header("Additional Bonus")]
    [SerializeField] bool hasBonus;
    [SerializeField] int bonusStat;
    [SerializeField] bool isMultiplier;
}
