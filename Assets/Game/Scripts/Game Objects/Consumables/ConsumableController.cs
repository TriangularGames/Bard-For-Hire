using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsumableController : MonoBehaviour
{
    public ConsumableData consumableData;

    [SerializeField] private TMP_Text nameTxt;

    public void Setup()
    {
        SetSprite();

        if (nameTxt != null)
        {
            SetName();
        }
    }

    public void Clear()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = null;
        transform.GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        nameTxt.text = "";
    }

    private void SetSprite()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = consumableData.icon;
        transform.GetChild(0).GetComponent<Image>().color = Color.white;
    }

    public void SetTextColor(Color color)
    {
        nameTxt.color = color;
    }

    private void SetName()
    {
        nameTxt.text = consumableData.ConsumableName;
    }
}
