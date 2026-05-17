using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Select : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerExitHandler
{
    private Image selection;
    private bool isSelected = false;

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
        if (!isSelected && GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().HasRoom())
        {
            selection.color = new Color(0f, 256f, 0f, 0.5f);
            GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().ItemsSelected.Add(gameObject);
            isSelected = true;
        }
        else
        {
            selection.color = new Color(0f, 0f, 256f, 0.5f);
            if (GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().ItemsSelected.Contains(gameObject))
            {
                GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().ItemsSelected.Remove(gameObject);
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
            GameObject.FindWithTag("ItemManager").GetComponent<ItemManager>().ItemsSelected.Remove(gameObject);
            isSelected = false;
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
