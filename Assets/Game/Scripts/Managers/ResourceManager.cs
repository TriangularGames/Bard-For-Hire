using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public ItemData[] ItemData;
    public UpgradeData[] UpgradeData;
    public ConsumableData[] ConsumableData;
    public EnemyData[] EnemyData;

    private void Start()
    {
        ItemData = Resources.LoadAll<ItemData>("ScriptableObjects/Items");
        UpgradeData = Resources.LoadAll<UpgradeData>("ScriptableObjects/Upgrades");
        ConsumableData = Resources.LoadAll<ConsumableData>("ScriptableObjects/Consumables");
        EnemyData = Resources.LoadAll<EnemyData>("ScriptableObjects/Enemies");
    }
}
