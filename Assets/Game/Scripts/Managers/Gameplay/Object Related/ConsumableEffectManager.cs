using UnityEngine;

public class ConsumableEffectManager : MonoBehaviour
{
    public static ConsumableEffectManager Instance;
    public bool selectingItemToDestroy;
    public bool selectingItemToClone;
    public bool selectingItemToPolymorph;
    public bool isScoring = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<RoundStartedEvent>(OnScoringStarted);
        EventBus.Subscribe<RoundEndedEvent>(OnScoringEnded);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<RoundStartedEvent>(OnScoringStarted);
        EventBus.Unsubscribe<RoundEndedEvent>(OnScoringEnded);
    }

    private void OnScoringStarted(RoundStartedEvent e) => isScoring = true;
    private void OnScoringEnded(RoundEndedEvent e) => isScoring = false;

    public void UseConsumable(ConsumableData consumable)
    {
        switch (consumable.Type)
        {
            // this is for the cosumable "Focus Potion" (Reduces Roll DC by 2 for this turn)
            case ConsumableID.FocusPotion:
                UpgradeFightingManager.Instance.tempDCReduce = 2; break;

            case ConsumableID.PoisonPotion:
                PoisonFirstEnemy();
                break;

            case ConsumableID.SharpeningStone:
                UpgradeFightingManager.Instance.tempDamgeIncrease = 1.3f; break;

            case ConsumableID.LuckPotion:
                UpgradeFightingManager.Instance.rollAbove10 = true; break;

            case ConsumableID.PotionOfMelting:
                selectingItemToDestroy = true;
                break;

            case ConsumableID.PotionOfPolymorph:
                selectingItemToPolymorph = true;
                break;

            case ConsumableID.PotionOfCloning:
                selectingItemToClone = true;
                break;

        }
    }

    private void PoisonFirstEnemy()
    {
        foreach (Transform spawnPoint in EnemyManager.Instance.spawnPoints)
        {
            if (spawnPoint.childCount == 0) continue;

            GameObject enemy = null;
            for (int i = 0; i < spawnPoint.childCount; i++)
            {
                if (spawnPoint.GetChild(i).GetComponent<EnemyController>() != null)
                {
                    enemy = spawnPoint.GetChild(i).gameObject;
                    break;
                }
            }

            if (enemy == null) continue;
            if (enemy.GetComponent<EnemyController>().GetHealth() <= 0) continue;

            EventBus.Publish(new DamageTakenEvent(enemy.GetEntityId(), 5, false, false));
            return;
        }
    }

    public void DestroyItem(ItemData item)
    {
        PlayerManager.Instance.RemoveInventoryItem(item);
        selectingItemToDestroy = false;
    }

    public ItemData PolymorphItem(ItemData item)
    {
        selectingItemToPolymorph = false;
        return PlayerManager.Instance.TransformInventoryItem(item);
    }
    public void CloneItem(ItemData item)
    {
        PlayerManager.Instance.CloneInventoryItem(item);
        selectingItemToClone = false;
    }
}
