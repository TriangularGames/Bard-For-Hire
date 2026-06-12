using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public Button discardBtn;
    public Button clearBtn;
    public Button attackBtn;
    public Button inventoryBtn;

    [HideInInspector] public List<GameObject> ItemsSelected;
    [SerializeField] private int selectionLimit = 4;
    [SerializeField] private TMP_Text selectionCountText;

    private bool scoringCompleted = true;
    
    // Number of Items that have been Discarded
    private int itemsDiscarded = 0;

    // Max discards a player can make
    public static int MAXDiscards = 2;
    private int discardsLeft = MAXDiscards;

    public ItemPool itemPool;

    private List<GameObject> _attackItems;

    private void OnEnable()
    {
        EventBus.Subscribe<ItemUsedEvent>(DeleteItem);
        EventBus.Subscribe<ScoringCompletedEvent>(PrepNewRound);

        discardBtn.onClick.AddListener(delegate { AudioManager.Instance.Error(); });
        clearBtn.onClick.AddListener(delegate { AudioManager.Instance.Error(); });
        attackBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        inventoryBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ItemUsedEvent>(DeleteItem);
        EventBus.Unsubscribe<ScoringCompletedEvent>(PrepNewRound);

        discardBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Error(); });
        clearBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Error(); });
        attackBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        inventoryBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
    }

    public bool HasRoom()
    {
        if (ItemsSelected.Count < selectionLimit) return true;
        return false;
    }

    private void UpdateSelectionText()
    {
        if (selectionCountText != null)
            selectionCountText.text = $"{ItemsSelected.Count}/{selectionLimit}";
    }

    private void DeleteItem(ItemUsedEvent e)
    {
        if (_attackItems == null || e.attackIndex < 0 || e.attackIndex >= _attackItems.Count) return;

        GameObject toDelete = _attackItems[e.attackIndex];
        if (toDelete == null) return;

        _attackItems[e.attackIndex] = null;
        itemPool.RemoveItem(toDelete);
        Destroy(toDelete);
    }

    private void Start()
    {
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.ActionSurge))
        {
            selectionLimit += 1;
        }
        UpdateSelectionText();
        ItemsSelected = new List<GameObject>();
        discardsLeft = MAXDiscards;
        discardBtn.transform.GetComponentInChildren<TMP_Text>().text = "Discard " + discardsLeft.ToString() + "/" + MAXDiscards;
# if UNITY_EDITOR
        Debug.Assert(itemPool = GameObject.FindWithTag("ItemPool").GetComponent<ItemPool>(), "ItemManager requires ItemPool");
#else
        itemPool = GameObject.FindWithTag("ItemPool").GetComponent<ItemPool>();
#endif
    }

    private void Update()
    {
        // Update button interactability
        if (discardsLeft == 0)
        {
            discardBtn.interactable = false;
        }
        else
        {
            discardBtn.interactable = true;
        }

        if (!scoringCompleted)
        {
            discardBtn.interactable = false;
            clearBtn.interactable = false;
            inventoryBtn.interactable = false;
        }
        else
        {
            if (discardsLeft != 0)
            {
                discardBtn.interactable = true;
            }
            clearBtn.interactable = true;
            inventoryBtn.interactable = true;
        }

        // TODO: make button not pop up/out of existence immediately
        if (ItemsSelected.Count == 0 || !scoringCompleted)
        {
            attackBtn.gameObject.SetActive(false);
            attackBtn.interactable = false;
        }
        else
        {
            attackBtn.gameObject.SetActive(true);
            attackBtn.interactable = true;
        }
    }

    /// <summary>
    /// When discarding Items, destroy them.
    /// </summary>
    public void DiscardItems()
    {
        itemsDiscarded = 0;
        if (discardsLeft != 0)
        {
            if (ItemsSelected.Count > 0)
            {
                // If unused Weapons is less than the amount needed to be refilled
                // Refresh Weapons before drawing
                if (PlayerManager.Instance.itemsNotUsed.Count < ItemsSelected.Count)
                {
                    PlayerManager.Instance.RefreshItems();
                }

                for (int i = 0; i < ItemsSelected.Count; i++)
                {
                    if (itemPool.GetItems().Contains(ItemsSelected[i]))
                    {
                        Destroy(ItemsSelected[i]);
                        EventBus.Publish(new ItemDiscardedEvent(ItemsSelected[i].GetComponent<ItemController>().itemData));
                        itemsDiscarded++;
                    }
                }
                itemPool.RemoveAll(ItemsSelected);
                ItemsSelected.Clear();
                GrabNewItems(itemsDiscarded);
                discardsLeft--;
                discardBtn.transform.GetComponentInChildren<TMP_Text>().text = "Discard " + discardsLeft.ToString() + "/" + MAXDiscards;
            }
        }
        UpdateSelectionText();
    }

    /// <summary>
    /// When round is over, or Items are discarded- get new Items from the inventory to take their place
    /// </summary>
    public void GrabNewItems(int amount)
    {
        int emptySlots = itemPool.GetMaxSlots() - itemPool.GetItems().Count;
        int toGrab = Mathf.Min(amount, emptySlots);
        if (toGrab <= 0) return;

        if (PlayerManager.Instance.itemsNotUsed.Count < toGrab)
        {
            PlayerManager.Instance.RefreshItems();
        }

        for (int i = 0; i < toGrab; i++)
        {
            itemPool.InstantiateItem(PlayerManager.Instance.GetRandomItem());
        }
    }

    /// <summary>
    /// Deselect all Items selected
    /// </summary>
    public void ClearItems()
    {
        foreach (GameObject item in itemPool.storedObjects)
        {
            item.GetComponent<Select>().Deselect();
        }
        UpdateSelectionText();
    }

    /// <summary>
    /// Sends Item List to ScoreManager for final score total
    /// </summary>
    public void CalculateScore()
    {
        scoringCompleted = false;
        List<ItemData> itemData = new List<ItemData>();
        List<GameObject> itemObjects = new List<GameObject>(ItemsSelected);

        foreach (GameObject itemObj in ItemsSelected)
        {
            itemData.Add(itemObj.GetComponent<ItemController>().itemData);
        }

        BeginAttack(itemObjects);

        DiceRoller roller = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>().roller;
        CombatManager.Instance.SwitchState(new ItemLineUpState(itemData, roller, itemObjects));
    }

    private void PrepNewRound(ScoringCompletedEvent e)
    {
        _attackItems?.Clear();
        scoringCompleted = true;
    }

    public void SelectItem(GameObject item)
    {
        if (HasRoom())
        {
            ItemsSelected.Add(item);
            Relabel();
            UpdateSelectionText();
        }
    }

    public void DeselectItem(GameObject item, TMP_Text num)
    {
        ItemsSelected.Remove(item);
        num.text = "";
        Relabel();
        UpdateSelectionText();
    }

    // Relabel selected items
    public void Relabel()
    {
        for (int i = 0; i <= ItemsSelected.Count - 1; i++)
        {
            GameObject text = ItemsSelected[i].transform.GetChild(ItemsSelected[i].transform.childCount - 1).gameObject;
            text.GetComponent<TMP_Text>().text = (i + 1).ToString();
        }
    }

    public void BeginAttack(List<GameObject> attackItems)
    {
        _attackItems = new List<GameObject>(attackItems);
        // Hand selection is done � only attack snapshot matters now
        ItemsSelected.Clear();
        UpdateSelectionText();
    }

    public GameObject GetAttackItem(int index)
    {
        if (_attackItems == null || index < 0 || index >= _attackItems.Count) return null;
        return _attackItems[index];
    }
    public int GetAttackItemCount()
    {
        return _attackItems?.Count ?? 0;
    }

    public void ShowItemInventory()
    {
        MenuManager.Instance.SwitchState(new InventoryMenuState());
    }
}

/// <summary>
/// Event for when an Item is Discarded
/// </summary>
public struct ItemDiscardedEvent
{
    public ItemData item;

    public ItemDiscardedEvent(ItemData _item)
    {
        item = _item;
    }
}
