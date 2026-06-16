using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    [SerializeField] public ItemData itemData;

    [SerializeField] protected Image icon;
    [SerializeField] protected GameObject dmgBanner;
    [SerializeField] protected TMP_Text damageTxt;
    [SerializeField] protected GameObject playableBanner;
    [SerializeField] protected TMP_Text playableText;

    public void Setup()
    {
        SetSprite();
        SetDamageTxt();
        SetPlayableTxt();
    }

    private void SetSprite()
    {
        icon.sprite = itemData.icon;
    }

    protected virtual void SetDamageTxt()
    {
        damageTxt.text = (itemData.Damage + itemData.bonusDamageStacks).ToString();
    }

    public void DisableText()
    {
        damageTxt.text = "";
        playableText.text = "";
        dmgBanner.SetActive(false);
        playableBanner.SetActive(false);
    }

    public void FadeImage(int value)
    {
        ResetColor();
        icon.color = new Color(icon.color.r, icon.color.g, icon.color.b, icon.color.a / (value * 2));
    }

    private void ResetColor()
    {
        icon.color = Color.white;
    }

    private void SetPlayableTxt()
    {
        playableText.text = itemData.Playable.ToString();
    }

    public void HideDisplayText()
    {
        if (damageTxt != null)
        {
            damageTxt.gameObject.SetActive(false);
        }
        if (playableText != null)
        {
            playableText.gameObject.SetActive(false);
        }
    }
}
