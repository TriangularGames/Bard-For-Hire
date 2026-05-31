using UnityEngine;

public class ConsumablePool : BaseItemContainer
{
    public void InstantiateConsumable(
        ConsumableData consumable
    )
    {
        GameObject consumableSpawned = AssetManager.Instance.Spawn("Consumable", transform);

        consumableSpawned.GetComponent<ConsumableController>().consumableData = consumable;

        consumableSpawned.GetComponent<ConsumableController>().Setup();

        consumableSpawned.GetComponent<ConsumableController>().SetTextColor(Color.white);

        AddItem(consumableSpawned);
    }
}
