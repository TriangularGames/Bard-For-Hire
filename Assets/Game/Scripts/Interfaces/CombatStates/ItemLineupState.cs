using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private int _placementIndex;
    private int _stackChildCount;

    private Image _itemImage;
    private List<Color> _startColors;

    private Vector3 _stackOffset = new Vector3(-0.09f, -0.03f, 0.01f);
    private float _lerpDuration = 0.5f;
    private float _holdDuration = 0.5f;

    private int _currentItemIndex;
    private float _lerpTimer;
    private float _holdTimer;
    private bool _allLerpsDone;
    private bool _transitioned;

    private Animator _banner;
    private bool _bannerDown = false;

    private ScoreManager _sm;

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
        _currentItemIndex = _items.Count - 1;
        _placementIndex = 0;
        _bannerDown = false;

        _banner = GameObject.FindWithTag("RollBanner").GetComponent<Animator>();
        _sm = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();

        _sm.InitializeRound(_items);

        _displayWorldPos = _sm.itemDisplay.GetComponent<RectTransform>().position;
        // Stack items under the attack display area, NOT under Hand's GridLayoutGroup
        _stackParent = _sm.itemDisplay.transform.parent;
        _stackChildCount = _stackParent.transform.childCount - 1;

        _rectTransforms = new List<RectTransform>();
        _startPositions = new List<Vector3>();
        _startColors = new List<Color>();

        for (int i = 0; i < _itemObjects.Count; i++)
        {
            RectTransform rect = _itemObjects[i].GetComponent<RectTransform>();
            _rectTransforms.Add(rect);
            _startPositions.Add(rect.position);
        }

        for (int i = _itemObjects.Count - 1; i > 0; i--)
        {
            Transform itemTransform = _itemObjects[i].GetComponent<Transform>();
            itemTransform.localPosition = new Vector3(itemTransform.localPosition.x,
                itemTransform.localPosition.y,
                itemTransform.localPosition.z + i);
        }

        for (int i = 0; i < _itemObjects.Count; i++)
        {
            Image img = _itemObjects[i].GetComponent<ItemController>().GetImage();
            _startColors.Add(img.color);
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
            // Lower the roll banner
            if (!_bannerDown)
            {
                _banner.SetTrigger("Lower");
            }
            if (_banner.GetCurrentAnimatorStateInfo(0).IsName("Lowered"))
            {
                _bannerDown = true;
            }

            if (_holdTimer >= _holdDuration && _bannerDown)
            {
                _transitioned = true;
                _roller.StartCombatRoll(_items, 0);
            }
        }
    }

    private void UpdateSequentialLerp(CombatManager cm)
    {
        if (_currentItemIndex < 0)
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

        _itemImage = _itemObjects[_currentItemIndex].GetComponent<ItemController>().GetImage();

        Color targetFade = new Color(_startColors[_currentItemIndex].r,
            _startColors[_currentItemIndex].g,
            _startColors[_currentItemIndex].b,
            _startColors[_currentItemIndex].a / (_currentItemIndex * 2));

        _itemImage.color = Color.Lerp(
            _startColors[_currentItemIndex],
            targetFade,
            smoothT
        );


        if (t >= 1f)
        {
            _lerpTimer = 0f;
            _currentItemIndex--;

            if (_currentItemIndex < 0)
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
        // Disables TooltipTrigger to prevent Tooltips on items being used for attacks
        item.GetComponent<TooltipTrigger>().enabled = false;

        // Remove from pool tracking lists only — don't destroy yet
        itemManager.itemPool.RemoveItem(item);

        // Fade the image based on distance from ItemDisplay
        //item.GetComponent<ItemController>().FadeImage(_currentItemIndex);

        // Reparent away from Hand grid, keep world position
        item.transform.SetParent(_stackParent, true);
        item.transform.SetSiblingIndex(_stackChildCount + _placementIndex);
        _placementIndex++;

        Select select = item.GetComponent<Select>();
        if (select != null && select.IsSelected)
        {
            item.GetComponent<Select>().ClearSelectionVisual();
        }
    }
}