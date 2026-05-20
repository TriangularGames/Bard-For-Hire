using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats")]
    [SerializeField] public int health;
    [SerializeField] public int coinReward;

    [Header("Enemy Info")]
    [SerializeField] public Sprite icon;
    [SerializeField] public AnimatorOverrideController animator;
    [SerializeField] string Description;

    [Header("Additional Bonus")]
    [SerializeField] bool hasBonus;
    [SerializeField] int bonusStat;
    [SerializeField] bool isMultiplier;
}
