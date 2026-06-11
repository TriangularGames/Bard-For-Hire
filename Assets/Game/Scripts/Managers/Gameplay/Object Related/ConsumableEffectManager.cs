using System.Collections.Generic;
using UnityEngine;

public class ConsumableEffectManager : MonoBehaviour
{
    public static ConsumableEffectManager Instance;
    public bool isScoring = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        EventBus.Subscribe<ScoringStartedEvent>(OnScoringStarted);
        EventBus.Subscribe<ScoringEndedEvent>(OnScoringEnded);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ScoringStartedEvent>(OnScoringStarted);
        EventBus.Unsubscribe<ScoringEndedEvent>(OnScoringEnded);
    }

    private void OnScoringStarted(ScoringStartedEvent e) => isScoring = true;
    private void OnScoringEnded(ScoringEndedEvent e) => isScoring = false;

    public void UseConsumable(ConsumableData consumable, List<GameObject> selectedItems)
    {
        AudioManager.Instance.PlayClip("Potion");
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
                DestroyItems(selectedItems);
                break;

            case ConsumableID.PotionOfPolymorph:
                ItemController polymorphTarget =selectedItems[0].GetComponent<ItemController>();
                polymorphTarget.itemData = PolymorphItem(polymorphTarget.itemData);
                polymorphTarget.Setup();
                polymorphTarget.GetComponent<Select>().Deselect();
                break;

            case ConsumableID.PotionOfCloning:
                CloneItem(selectedItems[0].GetComponent<ItemController>().itemData);
                selectedItems[0].GetComponent<Select>().Deselect();
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

            EventBus.Publish(new DamageTakenEvent(enemy.GetEntityId(), 5, "", false, false));
            return;
        }
    }

    private void DestroyItems(List<GameObject> items)
    {
        foreach (GameObject itemObj in items)
        {
            ItemData item = itemObj.GetComponent<ItemController>().itemData;

            PlayerManager.Instance.RemoveInventoryItem(item);
            itemObj.GetComponent<Select>().Deselect();
            Destroy(itemObj);
        }
    }

    public ItemData PolymorphItem(ItemData item)
    {
        return PlayerManager.Instance.TransformInventoryItem(item);
    }
    public void CloneItem(ItemData item)
    {
        PlayerManager.Instance.CloneInventoryItem(item);
    }
}
