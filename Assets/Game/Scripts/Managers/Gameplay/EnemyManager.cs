using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    [Header("Enemy Data")]
    [SerializeField] List<string> memberTypes;
    [SerializeField] public int daysTilBoss = 3;
    [SerializeField] private int daysBeforePooling = 10;
    public bool isBossDay;
    public bool isBossGone;
    public EnemyData bossData;
    public ItemType disabledItem;
    public bool hasDisabled;
    public float amountOff = 0.15f;
    private float healthMult = 1f;
    private float dailyMult = 1f;
    private float TotalMult => healthMult * dailyMult;

    /// <summary>
    /// Default Starting Round Data
    /// </summary>
    [SerializeField] private RoundData DEFAULTRoundData;

    /// <summary>
    /// Number of Enemies for the encounter
    /// </summary>
    [SerializeField] private RoundData roundData;
      
    private List<EnemyData> nextEncounter = new List<EnemyData>();
    private int bossesKilled;
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
        EventBus.Subscribe<ResetGameEvent>(ResetGame);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<EnemyDefeatedEvent>(RemoveEnemy);
        EventBus.Unsubscribe<EnterCombatEvent>(CombatSetup);
        EventBus.Unsubscribe<EnterShopEvent>(ShopSetup);
        EventBus.Unsubscribe<ResetGameEvent>(ResetGame);
    }

    // Reset to Game Defaults
    private void ResetGame(ResetGameEvent e)
    {
        currentDay = 0;
        bossesKilled = 0;
        healthMult = 1f;
        dailyMult = 1f;
        enemies.Clear();
        roundData = DEFAULTRoundData;
        isBossDay = false;
        isBossGone = false;
    }

    public override void Awake()
    {
        base.Awake();
        enemies = new List<GameObject>();
    }

    private void CombatSetup(EnterCombatEvent @event)
    {
        hasDisabled = false;
        isBossGone = isBossDay;
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

        int coins = enemyC.enemyData.coinReward;
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.MercenaryContract)) coins = Mathf.RoundToInt(coins * 1.3f);
        EventBus.Publish(new MoneyEarnedEvent(coins, enemyC.enemyData.Name, enemyC.gameObject.transform.parent.gameObject));

        if (enemies.Contains(e.enemy))
        {
            index = enemies.IndexOf(e.enemy);
            Destroy(e.enemy);
        }

        if (index != -1)
        {
            enemies.RemoveAt(index);
        }

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

        if (isBossGone)
        {
            bossesKilled++;
            healthMult *= roundData.bossVictoryMultiplier;
        }
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

        if (isBossDay && currentDay < daysBeforePooling)
        {
            List<EnemyData> list = new List<EnemyData>();
            foreach (EnemyData enemy in ResourceManager.Instance.EnemyData)
            {
                if (enemy.isBoss & enemy.ShowUpThisDay(currentDay))
                {
                    list.Add(enemy);
                }
            }
            bossData = list[Random.Range(0, list.Count)];
            nextEncounter.Add(bossData);
            return;
        }
        float scaleHealth = Mathf.Pow(roundData.dailyHealthMultiplier, currentDay);
        int minHealth = Mathf.RoundToInt(roundData.startMinTotalHealth * scaleHealth);
        int maxHealth = Mathf.RoundToInt(roundData.startMaxTotalHealth * scaleHealth);
        int attempts = 0;
        int maxAttempts = 20;
        List<EnemyData> bestAttempt = null;
        float bestDeviation = float.MaxValue;

        while (attempts < maxAttempts)
        {
            attempts++;
            List<EnemyData> attemptedGuys = MakeEncounter(minHealth, maxHealth);
            if (attemptedGuys == null)
            {
                continue;
            }
            int totaScaledHealth = 0;
            foreach (EnemyData enemyData in attemptedGuys)
            {
                totaScaledHealth += enemyData.GetScaledUpHealth(TotalMult);
            }
            int target = (minHealth + maxHealth) / 2;
            float deviation = Mathf.Abs(totaScaledHealth - target) / (float)target;

            if (deviation < bestDeviation)
            {
                bestDeviation = deviation;
                bestAttempt = attemptedGuys;
            }

            if (deviation <= amountOff)
            {
                nextEncounter = attemptedGuys;
                return;
            }
        }
        if (bestAttempt != null)
        {
            nextEncounter = bestAttempt;
        }

    }

    private List<EnemyData> MakeEncounter(int minBudget, int maxBudget) { 

        List<EnemyData> encGuy = new List<EnemyData>();
        int remainingHealth = Random.Range(minBudget, maxBudget + 1);

        while (remainingHealth > 0 && encGuy.Count < roundData.maxEnemies)
        {
            List<EnemyData> affordableGuys = new List<EnemyData>();
            foreach (EnemyData enemy in ResourceManager.Instance.EnemyData)
            {
                if (!enemy.ShowUpThisDay(currentDay) || (enemy.GetScaledUpHealth(TotalMult) > remainingHealth)) continue;
                bool isNormalEnemy = !enemy.isBoss;
                bool isPoolableBoss = enemy.isBoss && currentDay >= daysBeforePooling;

                if (isNormalEnemy || isPoolableBoss)
                    affordableGuys.Add(enemy);
            }

            if (affordableGuys.Count == 0)
            {
                EnemyData cheapest = null;
                foreach (EnemyData enemy in ResourceManager.Instance.EnemyData)
                {
                    if (!enemy.ShowUpThisDay(currentDay)) continue;
                    bool isNormalEnemy = !enemy.isBoss;
                    bool isPoolableBoss = enemy.isBoss && currentDay >= daysBeforePooling;
                    if ((isNormalEnemy || isPoolableBoss) && (cheapest == null || enemy.GetScaledUpHealth(TotalMult) < cheapest.GetScaledUpHealth(TotalMult)))
                        cheapest = enemy;
                }
                if (cheapest != null) encGuy.Add(cheapest);
                break;
            }

            EnemyData chosen = affordableGuys[Random.Range(0, affordableGuys.Count)];
            encGuy.Add(chosen);
            remainingHealth -= chosen.GetScaledUpHealth(TotalMult);
        }
        return encGuy.Count > 0 ? encGuy : null;
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
            EnemyController controller = enemySpawned.GetComponent<EnemyController>();
            controller.enemyData = data;
            controller.SetScaledHealth(data.GetScaledUpHealth(TotalMult));
            controller.Setup();
            enemySpawned.name = data.name + " " + enemySpawned.GetEntityId();

            enemies.Add(enemySpawned);

            if (enemies.Count > 0)
            {
               enemies[0].GetComponent<EnemyController>().SetIndicator();
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
                displayComponent.Setup(nextEncounter[i], nextEncounter[i].GetScaledUpHealth(TotalMult));
        }
    }

    /// <summary>
    /// Helper to check if an enemy is currently in dying state (for item effects that trigger on enemy death, but before EnemyDefeatedEvent is published)
    /// </summary>
    /// <returns></returns>
    public bool IsAnyEnemyDying()
    {
        foreach (GameObject enemy in enemies)
        {
            if (enemy != null && enemy.GetComponent<EnemyController>().GetHealth() <= 0) return true;
        }
        return false;
    }
}

public struct MoneyEarnedEvent
{
    public int coinAmount;
    // Reason being either Early Completion/Enemy Name
    public string reason;
    public GameObject location;

    public MoneyEarnedEvent(int _coinAmount, string _reason, GameObject _loc)
    {
        coinAmount = _coinAmount;
        reason = _reason;
        location = _loc;
    }
}
