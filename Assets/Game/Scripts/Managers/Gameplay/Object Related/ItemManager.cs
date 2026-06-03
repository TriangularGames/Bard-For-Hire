using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public Button discardBtn;
    public Button clearBtn;
    public Button attackBtn;

    [HideInInspector] public List<GameObject> ItemsSelected;
    [SerializeField] private int selectionLimit = 4;
    [SerializeField] private List<Sprite> selectionBoxes;
    [SerializeField] private Transform lineupParent;
    [SerializeField] private float lineupMoveSpeed = 8f;
    [SerializeField] private TMP_Text selectionCountText;

    private bool scoringCompleted = true;
    
    // Number of Items that have been Discarded
    private int itemsDiscarded = 0;

    // Max discards a player can make
    public static int MAXDiscards = 2;
    private int discardsLeft = MAXDiscards;

    public ItemPool itemPool;

    private void OnEnable()
    {
        EventBus.Subscribe<ItemUsedEvent>(DeleteItem);
        EventBus.Subscribe<ScoringCompletedEvent>(PrepNewRound);

        discardBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        clearBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
        attackBtn.onClick.AddListener(delegate { AudioManager.Instance.Confirm(); });
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ItemUsedEvent>(DeleteItem);
        EventBus.Unsubscribe<ScoringCompletedEvent>(PrepNewRound);

        discardBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        clearBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
        attackBtn.onClick.RemoveListener(delegate { AudioManager.Instance.Confirm(); });
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
        GameObject toDelete = null;
        foreach (GameObject item in ItemsSelected)
        {
            if (item.GetComponent<ItemController>().itemData == e.item)
            {
                toDelete = item;
            }
        }

        if (toDelete != null)
        {
            ItemsSelected.Remove(toDelete);
            itemPool.RemoveItem(toDelete);
            Destroy(toDelete);
        }
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
        discardBtn.transform.GetComponentInChildren<TMP_Text>().text = "Discard x" + discardsLeft.ToString();
# if UNITY_EDITOR
        Debug.Assert(itemPool = GameObject.FindWithTag("ItemPool").GetComponent<ItemPool>(), "ItemManager requires ItemPool");
#else
        itemPool = GameObject.FindWithTag("ItemPool").GetComponent<ItemPool>();
#endif
    }

    private void Update()
    {
        if (discardsLeft != 0)
        {
            discardBtn.interactable = true;
        }
        else
        {
            discardBtn.interactable = false;
        }

        if (!scoringCompleted)
        {
            discardBtn.interactable = false;
            clearBtn.interactable = false;
        }
        else
        {
            discardBtn.interactable = true;
            clearBtn.interactable = true;
        }

        if (ItemsSelected.Count == 0 || !scoringCompleted)
        {
            attackBtn.interactable = false;
        }
        else
        {
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
                discardBtn.transform.GetComponentInChildren<TMP_Text>().text = "Discard x" + discardsLeft.ToString();
            }
        }
        UpdateSelectionText();
    }

    /// <summary>
    /// When round is over, or Items are discarded- get new Items from the inventory to take their place
    /// </summary>
    public void GrabNewItems(int amount)
    {
        Debug.Log("Getting new items!");
        if (itemPool.GetItems().Count != itemPool.GetMaxSlots())
        {
            // Checking if unused weapons count is more than what needs to be grabbed to fill the space
            if (PlayerManager.Instance.itemsNotUsed.Count < amount)
            {
                PlayerManager.Instance.RefreshItems(); 
            }
            for (int i = 0; i < amount; i++)
            {
                itemPool.InstantiateItem(PlayerManager.Instance.GetRandomItem());
            }

        }
    }

    /// <summary>
    /// Deselect all Items selected
    /// </summary>
    public void ClearItems()
    {
        // TODO: fix this
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
        foreach (GameObject itemObj in ItemsSelected)
        {
            itemData.Add(itemObj.GetComponent<ItemController>().itemData);
        }

        DiceRoller roller = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>().roller;
        CombatManager.Instance.SwitchState(new DiceRollState(itemData, roller));
    }

    private void PrepNewRound(ScoringCompletedEvent e)
    {
        GrabNewItems(e.count);
        scoringCompleted = true;
    }

    public void SelectItem(GameObject item, Image selection)
    {
        if (HasRoom())
        {
            ItemsSelected.Add(item);
            selection.sprite = selectionBoxes[ItemsSelected.IndexOf(item)];
            UpdateSelectionText();
        }
    }

    public void DeselectItem(GameObject item, Image selection)
    {
        ItemsSelected.Remove(item);
        selection.color = new Color(0f, 0f, 0f, 0f);
        foreach (GameObject selectedItem in ItemsSelected)
        {
            selectedItem.GetComponent<Select>().SetImage(selectionBoxes[ItemsSelected.IndexOf(selectedItem)]);
        }
        UpdateSelectionText();
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
