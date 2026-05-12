using System.Collections.Generic;
using System.Linq.Expressions;
using TMPro;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemy Data")]
    /// <summary>
    /// Enemy types available for this Performance
    /// </summary>
    [SerializeField] List<string> memberTypes;

    /// <summary>
    /// Number of Enemies for the encounter
    /// </summary>
    [SerializeField] int numberOfEnemies;

    [Header("Enemies")]
    /// <summary>
    /// Current active Enemies
    /// </summary>
    [SerializeField] List<EnemyData> enemies;

    /// <summary>
    /// Total Score value for this Audience
    /// </summary>
    [SerializeField] int totalScore;

    [Header("UI")]
    [SerializeField] TMP_Text scoreToBeat;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            int memberType = Random.Range(0, memberTypes.Count);
            for (int a = 0; a < ResourceManager.Instance.EnemyData.Length; a++)
            {
                if (ResourceManager.Instance.EnemyData[a].name == memberTypes[memberType])
                {
                    enemies.Add(ResourceManager.Instance.EnemyData[a]);
                }
            }
        }

        for (int j = 0; j < numberOfEnemies; j++)
        {
            Debug.Log("Enemy: " + enemies[j].name);
            totalScore += enemies[j].health;
        }

        Debug.Log("Total Score: " + totalScore);
        scoreToBeat.text = totalScore.ToString();
    }

    /// <summary>
    /// Takes a Value to either add or remove from Total Score
    /// </summary>
    /// <param name="val">Value to add or remove</param>
    public void EditScore(int val)
    {
        totalScore += val;
    }
}
