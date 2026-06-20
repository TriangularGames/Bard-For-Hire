using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConsumableSelect : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    [SerializeField] private GameObject useButtonObject;
    [SerializeField] private Button useButton;

    private Image selection;

    private ConsumableController consumableController;
    private bool isSelected;

    private static ConsumableSelect thisOneSelected;

    void Awake()
    {
        selection = transform.GetChild(1).GetComponent<Image>();

        consumableController = GetComponent<ConsumableController>();

        selection.color = new Color(1f, 1f, 1f, 0f);

        useButtonObject.SetActive(false);
        useButton.onClick.AddListener(OnUseClick);
    }

    private void Update()
    {
        if (isSelected && useButtonObject != null && useButtonObject.activeSelf)
            UpdateUseButton();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ConsumableEffectManager.Instance.isScoring) return;
        if (!isSelected)
            selection.color = new Color(0f, 0f, 1f, 0.5f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected) 
            selection.color = new Color(1f, 1f, 1f, 0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (ConsumableEffectManager.Instance.isScoring) return;
        if (thisOneSelected != null && thisOneSelected != this)
        {
            thisOneSelected.NoSelect();
        }

        if (isSelected)
        {
            NoSelect();
            return;
        }
        isSelected = true;
        thisOneSelected = this;
        selection.color = new Color(0f, 0.8f, 1f, 0.5f);

        useButtonObject.SetActive(true);
        UpdateUseButton();

    }

    public void UpdateUseButton()
    {
        ConsumableID type = consumableController.consumableData.Type; 
        ItemManager itemManager = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();
        int selectedCount = itemManager.ItemsSelected.Count;

        bool canUseIt;
        switch (type)
        {
            case ConsumableID.PotionOfCloning:
                canUseIt = selectedCount == 1;
                break;

            case ConsumableID.PotionOfPolymorph:
                canUseIt = selectedCount == 1;
                break;

            case ConsumableID.PotionOfMelting:
                canUseIt = selectedCount == 2;
                break;

            default:
                canUseIt = true;
                break;
        }
        useButton.interactable = canUseIt;

    }

    public void NoSelect()
    {
        isSelected = false;
        selection.color = new Color(1f, 1f, 1f, 0f);
        useButtonObject.SetActive(false);
        if (thisOneSelected == this) 
            thisOneSelected = null;

    }

    public void OnUseClick()
    {
        ItemManager itemManager = GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>();
        List<GameObject> selectedItems = new List<GameObject>(itemManager.ItemsSelected);
        ConsumableEffectManager.Instance.UseConsumable(consumableController.consumableData, selectedItems);

        ConsumableManager.Instance.RemoveConsumable(consumableController.consumableData);

        PlayerManager.Instance.consumableInventory.Remove(consumableController.consumableData);
        thisOneSelected = null;
        Destroy(gameObject);
    }
}
