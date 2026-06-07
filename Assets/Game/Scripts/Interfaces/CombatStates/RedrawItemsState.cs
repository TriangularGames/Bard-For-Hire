using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// State for redrawing new items after stacking, then transitioning to DiceRollState on the next update tick.
/// </summary>
public class RedrawItemsState : ICombatState
{
    private bool _transitioned;

    private const float EndCombatDuration = 1.5f;
    private float _endCombatTimer = 0f;

    public void EnterState(CombatManager cm)
    {
        _endCombatTimer = 0f;

        ItemManager itemManager = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();

        // Safety: ensure stacked items are not still tracked in the hand
        itemManager.itemPool.RemoveAll(itemManager.ItemsSelected);

        // Only fill slots that are actually empty
        int emptySlots = itemManager.itemPool.GetMaxSlots() - itemManager.itemPool.GetItems().Count;

        if (emptySlots > 0)
        {
            itemManager.GrabNewItems(emptySlots);
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
        if (_endCombatTimer < EndCombatDuration) { _endCombatTimer += Time.deltaTime; return; }

        _transitioned = true;
        cm.SwitchState(new DefaultCombatState());
    }
}