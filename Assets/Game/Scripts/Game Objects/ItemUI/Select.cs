using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Select : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    private Image selection;
    [SerializeField] private TMP_Text selectionNum;
    public void SetImage(Sprite _selection) { selection.sprite = _selection; }
    private bool isSelected = false;

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

    private void Awake()
    {
# if UNITY_EDITOR
        Debug.Assert(selection = transform.GetChild(0).GetComponent<Image>(), "GameObject requires an Image component");
#else
        selection = transform.GetChild(0).GetComponent<Image>();
#endif
        selection.color = new Color(selection.color.r, selection.color.g, selection.color.b, 0f);
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
            //selection.color = Color.blue;
            if (!isSelected)
            {
                selection.sprite = null;
                selectionNum.text = "";
                selection.color = new Color(0f, 0f, 256f, 0.5f);
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
                selection.color = new Color(1f, 1f, 1f, 1f);
                _itemManager.SelectItem(gameObject, selection, selectionNum);
                isSelected = true;
            }
            else
            {
                selection.color = new Color(0f, 0f, 256f, 0.5f);
                if (_itemManager.ItemsSelected.Contains(gameObject))
                {
                    _itemManager.DeselectItem(gameObject, selection, selectionNum);
                    isSelected = false;
                }
            }
        }
    }

    public void Deselect()
    {
        isSelected = false;
        selection.color = new Color(1f, 1f, 1f, 0f);
        if (_itemManager.ItemsSelected.Contains(gameObject))
        {
            _itemManager.DeselectItem(gameObject, selection, selectionNum);
            isSelected = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SelectionEnabled)
        {
            if (!isSelected)
            {
                selection.sprite = null;
                selection.color = new Color(1f, 1f, 1f, 0f);
            }
        }
    }
}
