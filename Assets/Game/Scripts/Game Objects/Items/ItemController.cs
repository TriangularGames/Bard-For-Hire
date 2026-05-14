using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    [SerializeField] public ItemData itemData;

    [SerializeField] private TMP_Text damageTxt;
    [SerializeField] private TMP_Text playableText;

    public void Setup()
    {
        SetSprite();
        SetDamageTxt();
        SetPlayableTxt();
    }

    private void SetSprite()
    {
        transform.GetChild(1).GetComponent<Image>().sprite = itemData.icon;
    }

    private void SetDamageTxt()
    {
        if (itemData.Mult)
        {
            damageTxt.text = "x" + itemData.Damage.ToString();
        }
        else
        {
            damageTxt.text = "ATk " + itemData.Damage.ToString();
        }
        
    }

    private void SetPlayableTxt()
    {
        playableText.text = "D (" + itemData.Playable.ToString() + ")";
    }
}
