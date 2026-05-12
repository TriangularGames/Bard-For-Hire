using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// AttackHand class to hold Item information for Scoring
/// </summary>
public class AttackHand : BaseItemContainer
{
    public Button attackBtn;

    private void OnEnable()
    {
        EventBus.Subscribe<ItemRemovedEvent>(DeleteItems);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ItemRemovedEvent>(DeleteItems);
    }

    private void Start()
    {
        ClearItems();
    }

    private void Update()
    {
        if (GetItems().Count == 0)
        {
            attackBtn.interactable = false;
        }
        else
        {
            attackBtn.interactable = true;
        }
    }

    /// <summary>
    /// Clears Items in AttackHand returns them to ItemPool
    /// </summary>
    private void ClearItems()
    {
        GetItems().Clear();
    }

    /// <summary>
    /// Clears Items in AttackHand after scoring is completed
    /// </summary>
    private void DeleteItems(ItemRemovedEvent e)
    {
        // This should eventually be edited to allow for effects and such when they're removed
        foreach (GameObject item in GetItems())
        {
            Destroy(item);
        }
        ClearItems();
    }

    /// <summary>
    /// Goes through all Items to ensure proper order for scoring
    /// </summary>
    private void VerifyOrder()
    {
        List<GameObject> orderedItems = new List<GameObject>();
        foreach (GameObject item in GetItems())
        {
            orderedItems.Add(item);
        }
        ClearItems();
        foreach (GameObject orderedItem in orderedItems)
        {
            AddItem(orderedItem);
        }
    }

    /// <summary>
    /// Sends Item List to ScoreManager for final score total
    /// </summary>
    public void CalculateScore()
    {
        VerifyOrder();
        List<ItemData> itemData = new List<ItemData>();
        foreach (GameObject itemObj in GetItems())
        {
            itemData.Add(itemObj.GetComponent<ItemController>().itemData);
        }
        ScoreManager.Instance.CalculateScore(itemData);
        //DeleteItems();
    }
}
