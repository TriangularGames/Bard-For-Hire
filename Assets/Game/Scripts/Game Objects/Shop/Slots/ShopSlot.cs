using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ShopSlot : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] public TMP_Text value;
    [SerializeField] public Image icon;
    [SerializeField] public Button buy;

    [HideInInspector] public bool _isSelected = false;
    [HideInInspector] public bool _Purchased = false;

    private void Start()
    {
        buy.onClick.AddListener(Purchase);
        buy.gameObject.SetActive(false);
        _Purchased = false;
    }

    public abstract void Purchase();

    public void OnPointerDown(PointerEventData eventData)
    {
        _isSelected = !_isSelected;
        SelectSlot(_isSelected);
    }

    public virtual void ClearInfo()
    {
        value.text = "";
        icon.sprite = null;
        icon.color = new Color(1f, 1f, 1f, 0f);
        buy.gameObject.SetActive(false);
    }

    public virtual void SelectSlot(bool select)
    {
        buy.gameObject.SetActive(select);
        EventBus.Publish<ItemSelectedEvent>(new ItemSelectedEvent(gameObject.GetEntityId()));
    }

    public void Deselect()
    {
        _isSelected = false;
        buy.gameObject.SetActive(_isSelected);
    }
}
