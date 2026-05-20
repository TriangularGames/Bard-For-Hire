using UnityEngine;

public class ConsumablePool : BaseItemContainer
{
    public GameObject consumablePrefab;

    public void InstantiateConsumable(
        ConsumableData consumable
    )
    {
        GameObject consumableSpawned = Instantiate(consumablePrefab, transform);

        consumableSpawned.GetComponent<ConsumableController>().consumableData = consumable;

        consumableSpawned.GetComponent<ConsumableController>().Setup();

        AddItem(consumableSpawned);
    }
}
