using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// State for redrawing new items after stacking, then transitioning to DiceRollState on the next update tick.
/// </summary>
public class RedrawItemsState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly DiceRoller _roller;
    private readonly List<GameObject> _itemObjects;
    private readonly int _count;

    private bool _transitioned;

    public RedrawItemsState(List<ItemData> items, DiceRoller roller, List<GameObject> itemObjects)
    {
        _items = items;
        _roller = roller;
        _itemObjects = itemObjects;
        _count = itemObjects.Count;
    }

    public void EnterState(CombatManager cm)
    {
        ItemManager itemManager = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();

        // Safety: ensure stacked items are not still tracked in the hand
        itemManager.itemPool.RemoveAll(_itemObjects);

        ItemSlot slot = itemManager.itemPool.GetComponent<ItemSlot>();
        if (slot != null)
        {
            slot.storedObjects.RemoveAll(obj => _itemObjects.Contains(obj));
        }

        // Only fill slots that are actually empty
        int emptySlots = itemManager.itemPool.GetMaxSlots() - itemManager.itemPool.GetItems().Count;
        int toGrab = Mathf.Min(_count, emptySlots);

        if (toGrab > 0)
        {
            itemManager.GrabNewItems(toGrab);
        }

        // Force grid to reflow
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            itemManager.itemPool.GetComponent<RectTransform>()
        );
    }

    public void ExitState(CombatManager cm) { }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;
        if (_transitioned) return;

        _transitioned = true;
        cm.SwitchState(new DiceRollState(_items, _roller));
    }
}