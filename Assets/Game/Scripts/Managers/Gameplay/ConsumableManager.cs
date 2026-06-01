using System;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
    public static ConsumableManager Instance;

    private List<ConsumableData> consumables;
    public ConsumablePool consumablePool;

    [SerializeField] private List<GameObject> consumableDisplays;

    private void OnEnable()
    {
        EventBus.Subscribe<ScoringCompletedEvent>(RefreshConsumableDisplays);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ScoringCompletedEvent>(RefreshConsumableDisplays);
    }

    private void RefreshConsumableDisplays(ScoringCompletedEvent @event)
    {
        foreach (GameObject consumable in consumableDisplays)
        {
            ConsumableController controller = consumable.GetComponent<ConsumableController>();
            if (controller.consumableData != null)
            {
                controller.consumableData = null;
                controller.Clear();                
            }
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetupConsumables();
        RefreshConsumables();
    }

    private void SetupConsumables()
    {
        consumables = new List<ConsumableData>();
        consumables.AddRange(PlayerManager.Instance.consumableInventory);
    }

    public void AddConsumable(ConsumableData consumable)
    {
        consumables.Add(consumable);

        RefreshConsumables();
    }

    /// <summary>
    /// Remove Consumable on use and add to Display
    /// </summary>
    /// <param name="consumable">Consumable to be removed</param>
    public void RemoveConsumable(ConsumableData consumable)
    {
        consumables.Remove(consumable);

        foreach (GameObject display in consumableDisplays)
        {
            ConsumableController controller = display.GetComponent<ConsumableController>();
            if (controller.consumableData == null)
            {
                controller.consumableData = consumable;
                controller.Setup();
                display.SetActive(true);
                // TODO: change this to an event call instead
                PlayerManager.Instance.consumableInventory.Remove(consumable);
                return;
            }
        }

        RefreshConsumables();
    }

    public void RefreshConsumables()
    {
        for (int i = 0; i < consumablePool.storedObjects.Count; i++)
        {
            Destroy(consumablePool.storedObjects[i]);
        }

        consumablePool.storedObjects.Clear();

        for (int i = 0; i < consumables.Count; i++)
        {
            consumablePool.InstantiateConsumable(consumables[i]);
        }
    }
}
