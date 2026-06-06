using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Select : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text selectionNum;
    [SerializeField] private GameObject icon;

    private bool isSelected = false;
    public bool IsSelected => isSelected;

    private bool SelectionEnabled = true;

    private ItemManager _itemManager;

    private void OnEnable()
    {
        EventBus.Subscribe<ScoringStartedEvent>(DisableSelection);
        EventBus.Subscribe<ScoringEndedEvent>(EnableSelection);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<ScoringStartedEvent>(DisableSelection);
        EventBus.Unsubscribe<ScoringEndedEvent>(EnableSelection);
    }

    private void EnableSelection(ScoringEndedEvent e)
    {
        SelectionEnabled = true;
    }

    private void DisableSelection(ScoringStartedEvent e)
    {
        SelectionEnabled = false;
    }

    private void Start()
    {
        if (GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>())
        {
            _itemManager = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SelectionEnabled)
        {
            if (!isSelected)
            {
                selectionNum.text = "";
                icon.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (SelectionEnabled)
        {
            //destroy
            if (ConsumableEffectManager.Instance.selectingItemToDestroy)
            {
                ConsumableEffectManager.Instance.DestroyItem(GetComponent<ItemController>().itemData);

                Destroy(gameObject);

                return;
            }

            // clonin
            if (ConsumableEffectManager.Instance.selectingItemToClone)
            {
                ConsumableEffectManager.Instance.CloneItem(GetComponent<ItemController>().itemData);

                return;
            }

            // polymorphin
            if (ConsumableEffectManager.Instance.selectingItemToPolymorph)
            {
                ItemData newItem = ConsumableEffectManager.Instance.PolymorphItem(GetComponent<ItemController>().itemData);

                GetComponent<ItemController>().itemData = newItem;

                GetComponent<ItemController>().Setup();

                return;
            }

            if (!isSelected && _itemManager.HasRoom())
            {
                _itemManager.SelectItem(gameObject, selectionNum);
                icon.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                isSelected = true;
            }
            else
            {
                if (_itemManager.ItemsSelected.Contains(gameObject))
                {
                    _itemManager.DeselectItem(gameObject, selectionNum);
                    icon.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    isSelected = false;
                }
            }
        }
    }

    public void Deselect()
    {
        isSelected = false;
        if (_itemManager.ItemsSelected.Contains(gameObject))
        {
            _itemManager.DeselectItem(gameObject, selectionNum);
            isSelected = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SelectionEnabled)
        {
            if (!isSelected)
            {
                selectionNum.text = "";
                icon.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
        }
    }

    public void ClearSelectionVisual()
    {
        isSelected = false;

        ItemController item = GetComponent<ItemController>();
        if (item != null)
        {
            item.HideDisplayText();
        }
    }
}
