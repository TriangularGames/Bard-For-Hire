using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "AudienceMemberData", menuName = "Scriptable Objects/AudienceMemberData")]
public class AudienceMemberData : ScriptableObject
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
