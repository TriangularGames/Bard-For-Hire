using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
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

    /// <summary>
    /// Current active Enemies
    /// </summary>
    List<GameObject> enemies;

    /// <summary>
    /// Spawnpoints for the enemies
    /// </summary>
    public List<Transform> spawnPoints;

    /// <summary>
    /// Enemy Display Objects
    /// </summary>
    public List<GameObject> enemyDisplays;

    private void OnEnable()
    {
        EventBus.Subscribe<EnemyDefeatedEvent>(RemoveEnemy);
        EventBus.Subscribe<EnterCombatEvent>(CombatSetup);
        EventBus.Subscribe<EnterShopEvent>(ShopSetup);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyDefeatedEvent>(RemoveEnemy);
        EventBus.Unsubscribe<EnterCombatEvent>(CombatSetup);
        EventBus.Unsubscribe<EnterShopEvent>(ShopSetup);
    }

    public override void Awake()
    {
        base.Awake();
        enemies = new List<GameObject>();
    }

    private void RemoveEnemy(EnemyDefeatedEvent e)
    {
        int index = -1;
        if (enemies.Contains(e.enemy))
        {
            index = enemies.IndexOf(e.enemy);
            Destroy(e.enemy);
        }

        enemies.RemoveAt(index);
        // Set new first enemy in list to active target (as list is ordered)
        enemies[0].GetComponent<EnemyController>().SetIndicator();
    }

    public bool AreEnemiesAlive()
    {
        bool alive = enemies.Count > 0 ? true : false;
        return alive;
    }

    private void CombatSetup(EnterCombatEvent @event)
    {
        SpawnEnemies();
    }

    private void ShopSetup(EnterShopEvent @event)
    {
        LookAhead();
    }

    /// <summary>
    /// Called when entering Combat scene to setup Enemies
    /// </summary>
    private void SpawnEnemies()
    {
        GameObject spawnPointHolder = GameObject.FindWithTag("SpawnPoints");
        for (int c = 0; c < spawnPointHolder.transform.childCount; c++)
        {
            spawnPoints.Add(spawnPointHolder.transform.GetChild(c));
        }

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

                    enemies.Add(enemySpawned);
                }
            }
        }

        // Set indicator of First enemy On
        enemies[0].GetComponent<EnemyController>().SetIndicator();
    }

    /// <summary>
    /// Called when entering Shop to setup Data for next Combat and Display to player
    /// </summary>
    private void LookAhead()
    {
        // Generate list of enemies based on Round # specific data
        // send data to panel in Shop
        // TODO: make functionality
        GameObject enemyDisplayHolder = GameObject.FindWithTag("EnemyDisplays");
        for (int c = 0; c < enemyDisplayHolder.transform.childCount; c++)
        {
            enemyDisplays.Add(enemyDisplayHolder.transform.GetChild(c).gameObject);
        }

        // Similar to spawning, goes through each display to place icon, and number of enemies
        // The Icon object also has a script that requires the EnemyData for the tool-tip functionality
    }
}
