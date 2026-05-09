using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    [SerializeField] public ItemData itemData;

    [SerializeField] private TMP_Text scoreTxt;
    [SerializeField] private TMP_Text playableText;

    public void Setup()
    {
        SetSprite();
        SetScoreTxt();
        SetPlayableTxt();
    }

    private void SetSprite()
    {
        GetComponent<Image>().sprite = itemData.icon;
    }

    private void SetScoreTxt()
    {
        if (itemData.Mult)
        {
            scoreTxt.text = "x" + itemData.Score.ToString();
        }
        else
        {
            scoreTxt.text = "+" + itemData.Score.ToString();
        }
        
    }

    private void SetPlayableTxt()
    {
        playableText.text = "(" + itemData.Playable.ToString() + ")";
    }
}
