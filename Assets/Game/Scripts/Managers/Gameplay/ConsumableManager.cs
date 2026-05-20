using System.Collections.Generic;
using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
    public static ConsumableManager Instance;
    public List<ConsumableData> consumables = new List<ConsumableData>();
    public ConsumablePool consumablePool;
    public int maxConsumables = 2;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshConsumables();
    }

    public void AddConsumable(ConsumableData consumable)
    {
        if (consumables.Count >= maxConsumables)
        {
            return;
        }

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
