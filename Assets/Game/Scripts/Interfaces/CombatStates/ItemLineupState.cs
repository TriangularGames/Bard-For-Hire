using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State for stacking all selected items under the attack display area, then transitioning to RedrawItemsState after a brief hold.
/// </summary>
public class ItemLineUpState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly DiceRoller _roller;
    private readonly List<GameObject> _itemObjects;

    private List<RectTransform> _rectTransforms;
    private List<Vector3> _startPositions;
    private Vector3 _displayWorldPos;
    private Transform _stackParent;

    private Vector3 _stackOffset = new Vector3(-0.05f, 0f, 0.01f);
    private float _lerpDuration = 0.5f;
    private float _holdDuration = 0.5f;

    private int _currentItemIndex;
    private float _lerpTimer;
    private float _holdTimer;
    private bool _allLerpsDone;
    private bool _transitioned;

    public ItemLineUpState(List<ItemData> items, DiceRoller roller, List<GameObject> itemObjects)
    {
        _items = items;
        _roller = roller;
        _itemObjects = itemObjects;
    }

    public void EnterState(CombatManager cm)
    {
        _lerpTimer = 0f;
        _holdTimer = 0f;
        _allLerpsDone = false;
        _transitioned = false;
        _currentItemIndex = 0;

        ScoreManager sm = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
        sm.InitializeRound(_items);

        _displayWorldPos = sm.itemDisplay.GetComponent<RectTransform>().position;
        // Stack items under the attack display area, NOT under Hand's GridLayoutGroup
        _stackParent = sm.itemDisplay.transform.parent;

        _rectTransforms = new List<RectTransform>();
        _startPositions = new List<Vector3>();

        for (int i = 0; i < _itemObjects.Count; i++)
        {
            RectTransform rect = _itemObjects[i].GetComponent<RectTransform>();
            _rectTransforms.Add(rect);
            _startPositions.Add(rect.position);
        }

        for (int i = 0; i < _itemObjects.Count; i++)
        {
            Canvas itemCanvas = _itemObjects[i].GetComponent<Canvas>();
            if (itemCanvas != null)
            {
                itemCanvas.sortingOrder = 100 - i;
            }
        }
    }

    public void ExitState(CombatManager cm) { }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;
        if (_transitioned) return;

        if (!_allLerpsDone)
        {
            UpdateSequentialLerp(cm);
        }
        else
        {
            _holdTimer += Time.deltaTime;
            if (_holdTimer >= _holdDuration)
            {
                _transitioned = true;
                _roller.StartCombatRoll(_items, 0);
            }
        }
    }

    private void UpdateSequentialLerp(CombatManager cm)
    {
        if (_currentItemIndex >= _itemObjects.Count)
        {
            _allLerpsDone = true;
            return;
        }

        if (_lerpTimer == 0f)
        {
            DetachFromHand(_itemObjects[_currentItemIndex]);
        }

        _lerpTimer += Time.deltaTime;
        float t = Mathf.Clamp01(_lerpTimer / _lerpDuration);
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        Vector3 target = _displayWorldPos + _stackOffset * _currentItemIndex;

        _rectTransforms[_currentItemIndex].position = Vector3.Lerp(
            _startPositions[_currentItemIndex],
            target,
            smoothT
        );

        if (t >= 1f)
        {
            _lerpTimer = 0f;
            _currentItemIndex++;

            if (_currentItemIndex >= _itemObjects.Count)
            {
                _allLerpsDone = true;
            }
        }
    }

    // Detach the item from the Hand's tracking and reparent it to the stack parent, while keeping its world position
    private void DetachFromHand(GameObject item)
    {
        ItemManager itemManager = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();

        // Turn off selection display & disable text
        item.GetComponent<Select>().RemoveDisplay();
        item.GetComponent<ItemController>().DisableText();

        // Remove from pool tracking lists only — don't destroy yet
        itemManager.itemPool.RemoveItem(item);

        ItemSlot slot = itemManager.itemPool.GetComponent<ItemSlot>();
        if (slot != null)
        {
            slot.RemoveObject(item);
        }

        // Reparent away from Hand grid, keep world position
        item.transform.SetParent(_stackParent, true);
        item.transform.SetSiblingIndex(0);

        // Prevent dragging stacked cards during combat
        Drag drag = item.GetComponent<Drag>();
        if (drag != null)
        {
            drag.enabled = false;
        }

        Select select = item.GetComponent<Select>();
        if (select != null && select.IsSelected)
        {
            item.GetComponent<Select>().ClearSelectionVisual();
        }
    }
}