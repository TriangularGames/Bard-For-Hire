using System.Collections.Generic;
using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
    public static ConsumableManager Instance;

    private List<ConsumableData> consumables;
    public ConsumablePool consumablePool;

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

    public void RemoveConsumable(ConsumableData consumable)
    {
        consumables.Remove(consumable);

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
