using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Enemy Stats")]
    [SerializeField] public int health;
    [SerializeField] public int coinReward;
    [SerializeField] public ItemType weakness;

    [Header("Enemy Info")]
    [SerializeField] public Sprite icon;
    [SerializeField] public AnimatorOverrideController animator;
    [SerializeField] public string Name;
    [SerializeField] public string Description;
    [SerializeField] public bool isBoss;
    [SerializeField] public BossAbilities ability;

    [Header("Day Appearance")]
    [SerializeField] public int minDay = 0;
    [SerializeField] public int maxDay = 0;

    [SerializeField] public float yPos = 0;

    public int GetScaledUpHealth(float healthMult)
    {
        return Mathf.RoundToInt(health * healthMult);
    }

    public bool ShowUpThisDay(int day)
    {
        if (day < minDay) {
            return false;
        }
        if (maxDay > 0 && day > maxDay) { 
            return false;
        }
        return true;
    }
}
