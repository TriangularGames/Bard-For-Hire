using System.Collections.Generic;
using UnityEngine;

public class ConsumableManager : MonoBehaviour
{
    public static ConsumableManager Instance;

    private List<ConsumableData> consumables;
    public ConsumablePool consumablePool;

    [SerializeField] private List<GameObject> consumableDisplays;

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
    /// <param name="consumable"></param>
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
