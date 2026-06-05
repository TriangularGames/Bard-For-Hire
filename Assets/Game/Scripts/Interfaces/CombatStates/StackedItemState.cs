using System.Collections.Generic;
using UnityEngine;

public class StackedItemsState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly DiceRoller _roller;
    private readonly List<GameObject> _itemObjects;

    private List<RectTransform> _rectTransforms;
    private List<Vector3> _startPositions;
    private Vector3 _displayWorldPos;

    private Vector3 _stackOffset = new Vector3(-0.05f, 0f, -0.01f);
    private float _lerpDuration = 0.5f;
    private float _holdDuration = 0.5f;

    private int _currentItemIndex;
    private float _lerpTimer;
    private float _holdTimer;
    private bool _allLerpsDone;
    private bool _transitioned;

    public StackedItemsState(List<ItemData> items, DiceRoller roller, List<GameObject> itemObjects)
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
        _currentItemIndex = 0; // start with first item

        ScoreManager sm = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
        sm.InitializeRound(_items);

        // Get itemDisplay world position as the target
        _displayWorldPos = sm.itemDisplay.GetComponent<RectTransform>().position;

        // Store start positions and rect transforms
        _rectTransforms = new List<RectTransform>();
        _startPositions = new List<Vector3>();

        for (int i = 0; i < _itemObjects.Count; i++)
        {
            RectTransform rect = _itemObjects[i].GetComponent<RectTransform>();
            _rectTransforms.Add(rect);
            _startPositions.Add(rect.position);
        }

        // First item gets the highest order, subsequent items get lower orders
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
            _holdTimer += Time.unscaledDeltaTime;
            if (_holdTimer >= _holdDuration)
            {
                _transitioned = true;
                cm.SwitchState(new DiceRollState(_items, _roller));
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

        _lerpTimer += Time.unscaledDeltaTime;
        float t = Mathf.Clamp01(_lerpTimer / _lerpDuration);
        float smoothT = Mathf.SmoothStep(0f, 1f, t);

        // first item goes to display position
        // each subsequent item goes to display position + offset * index
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
                // Fill slots now that all items have visually moved away
                GameObject.FindWithTag("ItemManager")
                    .GetComponent<ItemManager>()
                    .GrabNewItems(_itemObjects.Count);

                _allLerpsDone = true;
            }
        }
    }
}