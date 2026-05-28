using UnityEngine;

public class TempShop : MonoBehaviour
{
    [SerializeField] ItemController item;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get a random item to display
        int randomItem = Random.Range(0, ResourceManager.Instance.ItemData.Length);
        item.itemData = ResourceManager.Instance.ItemData[randomItem];
        // Add it to active player inventory
        PlayerManager.Instance.itemInventory.Add(item.itemData);
    }

    public void NextCombat()
    {
        GameManager.Instance.SwitchState(new CombatState());
    }
}
