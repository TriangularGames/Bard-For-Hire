using UnityEngine;
using static UnityEditor.Progress;

public class ConsumableEffectManager : MonoBehaviour
{
    public static ConsumableEffectManager Instance;
    public bool selectingItemToDestroy;
    public bool selectingItemToClone;
    public bool selectingItemToPolymorph;

    private void Awake()
    {
        Instance = this;
    }

    public void UseConsumable(ConsumableData consumable)
    {
        switch (consumable.Type)
        {
            // this is for the cosumable "Focus Potion" (Reduces Roll DC by 2 for this turn)
            case ConsumableID.FocusPotion:
                UpgradeFightingManager.Instance.tempDCReduce = 2; break;

            case ConsumableID.PoisonPotion:
                foreach (Transform enemyLocation in GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().spawnPoints)
                {
                    // Check if the location has an enemy in it
                    if (enemyLocation.transform.childCount > 0)
                    {
                        // Get the enemy at this location
                        GameObject enemy = enemyLocation.transform.GetChild(0).gameObject;

                        if (enemy.GetComponent<EnemyController>().GetHealth() > 0)
                        {
                            EventBus.Publish<DamageTakenEvent>(
                                    new DamageTakenEvent(enemyLocation.transform.GetChild(0).gameObject.GetEntityId(), 10));
                            break;
                        }
                    }
                }
                break;

            case ConsumableID.SharpeningStone:
                UpgradeFightingManager.Instance.tempDamgeIncrease = 1.3f; break;

            case ConsumableID.LuckPotion:
                    UpgradeFightingManager.Instance.rollAbove10 = true; break;

            case ConsumableID.RerollPotion:
                UpgradeFightingManager.Instance.reroll = true; break;

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
