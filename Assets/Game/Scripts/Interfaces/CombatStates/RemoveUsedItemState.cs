using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Lerps the used stacked item off screen, then destroys it and hands off to BetweenItemState.
/// </summary>
public class RemoveUsedItemState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly ScoreManager _sm;

    private const float LerpDuration = 1.0f;

    private GameObject _stackedItem;
    private RectTransform _itemRect;
    private Vector3 _lerpStart;
    private Vector3 _lerpTarget;
    private float _timer;

    public RemoveUsedItemState(List<ItemData> items, int index, ScoreManager sm)
    {
        _items = items;
        _index = index;
        _sm = sm;
    }

    public void EnterState(CombatManager cm)
    {
        if (_stackedItem != null)
        {
            _stackedItem.SetActive(true);
        }

        _timer = 0f;

        ItemManager itemManager = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();
        _stackedItem = itemManager.GetAttackItem(_index);

        if (_stackedItem != null)
        {
            _itemRect = _stackedItem.GetComponent<RectTransform>();
            _lerpStart = _itemRect.position;
            _lerpTarget = GetExitPosition(itemManager);
        }

        _sm.HideItemDisplay();
    }

    public void ExitState(CombatManager cm) 
    {

    }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;

        _timer += Time.deltaTime;
        float duration = LerpDuration;
        float t = Mathf.Clamp01(_timer / duration);
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        if (_itemRect != null)
        {
            _itemRect.position = Vector3.Lerp(_lerpStart, _lerpTarget, smoothT);
            if (Vector3.Distance(_itemRect.position, _lerpTarget) < 1f)
            {
                FinishAndTransition(cm);
                return;
            }
        }
        if (t < 1f) return;

        FinishAndTransition(cm);
    }

    private void FinishAndTransition(CombatManager cm)
    {
        EventBus.Publish(new ItemUsedEvent(_sm.pendingItems[_index], _index));
        cm.SwitchState(new MoveLineupState(_items, _index, _sm, skipWait: true));
    }

    private Vector3 GetExitPosition(ItemManager itemManager)
    {
        RectTransform hand = itemManager.itemPool.GetComponent<RectTransform>();
        return hand.position + Vector3.down * 150f;
    }
}