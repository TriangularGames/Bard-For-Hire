using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Select : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    private Image selection;
    public void SetImage(Sprite _selection) { selection.sprite = _selection; }
    private bool isSelected = false;

    private void Awake()
    {
# if UNITY_EDITOR
        Debug.Assert(selection = transform.GetChild(0).GetComponent<Image>(), "GameObject requires an Image component");
#else
        selection = transform.GetChild(0).GetComponent<Image>();
#endif
        selection.color = new Color(selection.color.r, selection.color.g, selection.color.b, 0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //selection.color = Color.blue;
        if (!isSelected)
        {
            selection.sprite = null;
            selection.color = new Color(0f, 0f, 256f, 0.5f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
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

        if (!isSelected && GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().HasRoom())
        {
            selection.color = new Color(1f, 1f, 1f, 1f);
            GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().SelectItem(gameObject, selection);
            isSelected = true;
        }
        else
        {
            selection.color = new Color(0f, 0f, 256f, 0.5f);
            if (GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().ItemsSelected.Contains(gameObject))
            {
                GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().DeselectItem(gameObject, selection);
                isSelected = false;
            }
        }
    }

    public void Deselect()
    {
        isSelected = false;
        selection.color = new Color(1f, 1f, 1f, 0f);
        if (GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().ItemsSelected.Contains(gameObject))
        {
            GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().DeselectItem(gameObject, selection);
            isSelected = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            selection.sprite = null;
            selection.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}
