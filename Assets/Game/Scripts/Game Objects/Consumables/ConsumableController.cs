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

    private void SetSprite()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = consumableData.icon;
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
