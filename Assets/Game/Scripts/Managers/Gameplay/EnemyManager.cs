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

    [Header("Enemies")]
    /// <summary>
    /// Current active Enemies
    /// </summary>
    [SerializeField] List<GameObject> enemies;

    /// <summary>
    /// Spawnpoints for the enemies
    /// </summary>
    [SerializeField] public List<Transform> spawnPoints;

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

    private void RemoveEnemy(EnemyDefeatedEvent e)
    {
        int index = -1;
        if (enemies.Contains(e.enemy))
        {
            index = enemies.IndexOf(e.enemy);
            Destroy(e.enemy);
        }

        enemies.RemoveAt(index);
    }

    public bool AreEnemiesAlive()
    {
        bool alive = enemies.Count > 0 ? true : false;
        return alive;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // NOTE: UNCOMMENT THIS WHEN USING COMBATTEST SCENE
        //SpawnEnemies();
    }

    private void CombatSetup(EnterCombatEvent @event)
    {
        SpawnEnemies();
    }

    private void ShopSetup(EnterShopEvent @event)
    {
        LookAhead();
    }

    private void SpawnEnemies()
    {
        // TODO: add event call that calls this function when entering Combat
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
    }

    /// <summary>
    /// Called when entering Shop to setup Data for next Combat and Display to player
    /// </summary>
    private void LookAhead()
    {
        // Generate list of enemies based on Round # specific data
        // send data to panel in Shop
        // TODO: make functionality
        // TODO: Add event call when entering shop that calls this function
    }
}
