using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Select : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    private Image selection;
    private bool isSelected = false;

    private void OnEnable()
    {
        EventBus.Subscribe<DragEvent>(DeselectItem);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<DragEvent>(DeselectItem);
    }

    private void Awake()
    {
        Debug.Assert(selection = transform.GetChild(0).GetComponent<Image>(), "GameObject requires an Image component");
        selection.color = new Color(selection.color.r, selection.color.g, selection.color.b, 0f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //selection.color = Color.blue;
        if (!isSelected)
        {
            selection.color = new Color(0f, 0f, 256f, 0.5f);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isSelected)
        {
            selection.color = new Color(0f, 256f, 0f, 0.5f);
            ItemManager.Instance.ItemsToDelete.Add(gameObject);
            isSelected = true;
        }
        else
        {
            selection.color = new Color(0f, 0f, 256f, 0.5f);
            if (ItemManager.Instance.ItemsToDelete.Contains(gameObject))
            {
                ItemManager.Instance.ItemsToDelete.Remove(gameObject);
                isSelected = false;
            }
        }
    }

    /// <summary>
    /// If item is being dragged, remove it from being selected
    /// </summary>
    /// <param name="e">Data containing what item is being dragged</param>
    private void DeselectItem(DragEvent e)
    {
        if (gameObject == e.item)
        {
            selection.color = new Color(selection.color.r, selection.color.g, selection.color.b, 0f);
            isSelected = false;
            if (ItemManager.Instance.ItemsToDelete.Contains(gameObject))
            {
                ItemManager.Instance.ItemsToDelete.Remove(gameObject);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            selection.color = new Color(1f, 1f, 1f, 0f);
        }
    }
}
