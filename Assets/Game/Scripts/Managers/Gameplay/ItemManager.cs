using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public Button discardBtn;
    public Button attackBtn;

    [HideInInspector] public List<GameObject> ItemsSelected;
    [SerializeField] private int selectionLimit = 4;
    [SerializeField] private List<Sprite> selectionBoxes;
    [SerializeField] private Transform lineupParent;
    [SerializeField] private float lineupMoveSpeed = 8f;

    private bool scoringCompleted = true;
    
    // Number of Items that have been Discarded
    private int itemsDiscarded = 0;

    // Max discards a player can make
    public int MAXDiscards = 3;

    public ItemPool itemPool;

    private void OnEnable()
    {
        EventBus.Subscribe<ItemUsedEvent>(DeleteItem);
        EventBus.Subscribe<ScoringCompletedEvent>(PrepNewRound);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ItemUsedEvent>(DeleteItem);
        EventBus.Unsubscribe<ScoringCompletedEvent>(PrepNewRound);
    }

    public bool HasRoom()
    {
        if (ItemsSelected.Count < selectionLimit) return true;
        return false;
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
            ItemsSelected = new List<GameObject>();
        MAXDiscards = 3;
        discardBtn.transform.GetComponentInChildren<TMP_Text>().text = "Discard x" + MAXDiscards.ToString();
# if UNITY_EDITOR
        Debug.Assert(itemPool = GameObject.FindWithTag("ItemPool").GetComponent<ItemPool>(), "ItemManager requires ItemPool");
#else
        itemPool = GameObject.FindWithTag("ItemPool").GetComponent<ItemPool>();
#endif
    }

    private void Update()
    {
        if (MAXDiscards != 0)
        {
            discardBtn.interactable = true;
        }
        else
        {
            discardBtn.interactable = false;
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
        if (MAXDiscards != 0)
        {
            if (ItemsSelected.Count > 0)
            {
                if (PlayerManager.Instance.itemsNotUsed.Count != 0)
                {
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
                    MAXDiscards--;
                    discardBtn.transform.GetComponentInChildren<TMP_Text>().text = "Discard x" + MAXDiscards.ToString();
                }
                else
                {
                    Debug.Log("Item Inventory is Empty! Cannot Discard.");
                }
            }
        }
    }

    /// <summary>
    /// When round is over, or Items are discarded- get new Items from the inventory to take their place
    /// </summary>
    public void GrabNewItems(int amount)
    {
        Debug.Log("Getting new items!");
        if (itemPool.GetItems().Count != itemPool.GetMaxSlots())
        {
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
    }

    /// <summary>
    /// Sends Item List to ScoreManager for final score total
    /// </summary>
    public async void CalculateScore()
    {
        scoringCompleted = false;
        List<ItemData> itemData = new List<ItemData>();
        List<GameObject> orderedUp = new List<GameObject>(ItemsSelected);
        foreach (GameObject itemObj in orderedUp)
        {
            itemData.Add(itemObj.GetComponent<ItemController>().itemData);
        }
        await Lineup(orderedUp, itemData);
    }

    private async Task Lineup(List<GameObject> itemObjects, List<ItemData> itemData)
    {
        List<Vector3> startPositions = new List<Vector3>();
        foreach (GameObject objecte in itemObjects)
        {
            startPositions.Add(objecte.transform.position);
            objecte.transform.SetParent(lineupParent, true);
        }

        await Task.Yield();
        List<Vector3> targetPositions = new List<Vector3>();
        foreach (GameObject obj in itemObjects)
            targetPositions.Add(obj.transform.position);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * lineupMoveSpeed;
            for (int i = 0; i < itemObjects.Count; i++)
                itemObjects[i].transform.position = Vector3.Lerp(startPositions[i], targetPositions[i], Mathf.SmoothStep(0f, 1f, t));
            await Task.Yield();
        }

        await Task.Delay(300);

        await GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>().CalculateScore(itemData);
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
    }
}
