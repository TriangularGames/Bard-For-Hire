using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [Header("Enemy Data")]
    /// <summary>
    /// Enemy types available for this Performance
    /// </summary>
    [SerializeField] List<string> memberTypes;
    [SerializeField] public int daysTilBoss = 3;
    public bool isBossDay;
    public EnemyData bossData;
    public ItemType disabledItem;
    public bool hasDisabled;

    /// <summary>
    /// Number of Enemies for the encounter
    /// </summary>
    /// 

    [SerializeField] private RoundData roundData;

    private List<EnemyData> nextEncounter = new List<EnemyData>();

    public int currentDay = 0;

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

    private void CombatSetup(EnterCombatEvent @event)
    {
        hasDisabled = false;

        // generate encounter first
        if (nextEncounter.Count == 0)
        {
            GenerateRound();
        }
        SpawnEnemies();
        nextEncounter.Clear();
    }

    private void RemoveEnemy(EnemyDefeatedEvent e)
    {
        int index = -1;
        EnemyController enemyC = e.enemy.GetComponent<EnemyController>();
        PlayerManager.Instance.Coins += enemyC.enemyData.coinReward;
        PlayerManager.Instance.SetCoinText();

        if (enemies.Contains(e.enemy))
        {
            index = enemies.IndexOf(e.enemy);
            Destroy(e.enemy);
        }

        enemies.RemoveAt(index);
        // Set new first enemy in list to active target (as list is ordered)
        if (enemies.Count > 0)
        {
            enemies[0].GetComponent<EnemyController>().SetIndicator();
        }
    }

    public bool AreEnemiesAlive()
    {
        bool alive = enemies.Count > 0 ? true : false;
        return alive;
    }

    private void ShopSetup(EnterShopEvent @event)
    {
        currentDay++;
        isBossDay = (currentDay % daysTilBoss == 0 && currentDay != 0);
        GenerateRound();
        LookAhead();
    }

    public void GenerateNext()
    {
        GenerateRound();
    }

    private void GenerateRound()
    {
        nextEncounter.Clear();
        bossData = null;

        if (isBossDay)
        {
            List<EnemyData> list = new List<EnemyData>();
            foreach (EnemyData enemy in ResourceManager.Instance.EnemyData)
            {
                if (enemy.isBoss)
                {
                    list.Add(enemy);
                }
            }
            bossData = list[Random.Range(0, list.Count)];
            nextEncounter.Add(bossData);
            return;
        }

        int minHealth = roundData.startMinTotalHealth + (currentDay * roundData.startMinTotalHealth);
        int maxHealth = roundData.startMaxTotalHealth + (currentDay * roundData.startMaxTotalHealth);
        int targetHealth = Random.Range(minHealth, maxHealth + 1);
        int remainingHealth = targetHealth;

        while (remainingHealth > 0 && nextEncounter.Count < roundData.maxEnemies)
        {
            List<EnemyData> affordableGuys = new List<EnemyData>();
            foreach (EnemyData enemy in ResourceManager.Instance.EnemyData)
            {
                if (!enemy.isBoss & enemy.health <= remainingHealth)
                    affordableGuys.Add(enemy);
            }

            if (affordableGuys.Count == 0)
            {
                EnemyData cheapest = ResourceManager.Instance.EnemyData[0];
                foreach (EnemyData enemy in ResourceManager.Instance.EnemyData)
                {
                    if (!enemy.isBoss & enemy.health < cheapest.health)
                        cheapest = enemy;
                }
                nextEncounter.Add(cheapest);
                break;
            }

            EnemyData chosen = affordableGuys[Random.Range(0, affordableGuys.Count)];
            nextEncounter.Add(chosen);
            remainingHealth -= chosen.health;
        }
    }

    /// <summary>
    /// Called when entering Combat scene to setup Enemies
    /// </summary>
    /// 
    private void SpawnEnemies()
    {
        enemies.Clear();
        spawnPoints.Clear();

        GameObject spawnPointHolder = GameObject.FindWithTag("SpawnPoints");
        for (int c = 0; c < spawnPointHolder.transform.childCount; c++)
        {
            spawnPoints.Add(spawnPointHolder.transform.GetChild(c));
        }

        for (int i = 0; i < nextEncounter.Count; i++)
        {
            EnemyData data = nextEncounter[i];
            GameObject enemySpawned = AssetManager.Instance.Spawn("Enemy", spawnPoints[i]);
            enemySpawned.GetComponent<EnemyController>().enemyData = data;

            enemySpawned.GetComponent<EnemyController>().Setup();
            enemySpawned.name = data.name + " " + enemySpawned.GetEntityId();

            enemies.Add(enemySpawned);

            if (enemies.Count > 0)
            {
               enemies[0].GetComponent<EnemyController>().SetIndicator();
            }
            //    int memberType = Random.Range(0, memberTypes.Count);
            //    for (int a = 0; a < ResourceManager.Instance.EnemyData.Length; a++)
            //    {
            //        if (ResourceManager.Instance.EnemyData[a].name == memberTypes[memberType])
            //        {
            //            GameObject enemySpawned = AssetManager.Instance.Spawn("Enemy", spawnPoints[i]);
            //            EnemyData data = ResourceManager.Instance.EnemyData[a];
            //            enemySpawned.GetComponent<EnemyController>().enemyData = data;

            //            enemySpawned.GetComponent<EnemyController>().Setup();
            //            enemySpawned.name = data.name + " " + enemySpawned.GetEntityId();

            //            enemies.Add(enemySpawned);
            //        }
            //    }
            //}

            //// Set indicator of First enemy On

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
        GameObject enemyDisplayHolder = GameObject.FindWithTag("EnemyDisplays");
        enemyDisplays.Clear();

        for (int c = 0; c < enemyDisplayHolder.transform.childCount; c++)
        {
            enemyDisplays.Add(enemyDisplayHolder.transform.GetChild(c).gameObject);
        }

        foreach (GameObject disp in enemyDisplays)
        {
            disp.SetActive(false);
        }

        for(int i = 0; i < nextEncounter.Count && i < enemyDisplays.Count; i++)
        {
            enemyDisplays[i].SetActive(true);
            EnemyDisplay displayComponent = enemyDisplays[i].GetComponent<EnemyDisplay>();

            if (displayComponent != null)
                displayComponent.Setup(nextEncounter[i]);
        }
    }

}
