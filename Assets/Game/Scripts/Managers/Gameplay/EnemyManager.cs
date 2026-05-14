using System.Collections.Generic;
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
    [SerializeField] List<GameObject> enemies;

    /// <summary>
    /// Spawnpoints for the enemies
    /// </summary>
    [SerializeField] public List<Transform> spawnPoints;

    /// <summary>
    /// Total Score value for these Enemies
    /// </summary>
    [SerializeField] int totalScore;

    public bool AreEnemiesAlive()
    {
        foreach(Transform point in spawnPoints)
        {
            if (point.childCount > 0)
            {
                return true;
            }
        }
        return false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnEnemies();

        GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>().score = totalScore;
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            int memberType = Random.Range(0, memberTypes.Count);
            for (int a = 0; a < ResourceManager.Instance.EnemyData.Length; a++)
            {
                if (ResourceManager.Instance.EnemyData[a].name == memberTypes[memberType])
                {
                    GameObject enemySpawned = AssetManager.Instance.Spawn("Enemy", spawnPoints[i]);
                    EnemyData data = ResourceManager.Instance.EnemyData[a];
                    enemySpawned.GetComponent<EnemyController>().enemyData = data;

                    enemySpawned.GetComponent<EnemyController>().Setup();
                    enemySpawned.name = data.name + " " + enemySpawned.GetEntityId();

                    totalScore += data.health;
                    enemies.Add(enemySpawned);
                }
            }
        }
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
