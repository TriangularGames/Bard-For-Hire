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

        SetName();
    }

    private void SetSprite()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = consumableData.icon;
    }

    private void SetName()
    {
        nameTxt.text = consumableData.ConsumableName;
    }
}
