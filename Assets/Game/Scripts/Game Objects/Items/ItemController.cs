using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemController : MonoBehaviour
{
    [SerializeField] public ItemData itemData;

    [SerializeField] protected Image icon;
    [SerializeField] private GameObject dmgBanner;
    [SerializeField] protected TMP_Text damageTxt;
    [SerializeField] private GameObject playableBanner;
    [SerializeField] private TMP_Text playableText;

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
        if (itemData.Mult)
        {
            damageTxt.text = "x" + itemData.Damage.ToString();
        }
        else
        {
            damageTxt.text = (itemData.Damage + itemData.bonusDamageStacks).ToString();
        }
        
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

    public void SetDamageWithBonus(int baseD, UpgradeFightingManager.DamageBonus bonus)
    {
        if (bonus.amount > 0)
            damageTxt.text = $"{baseD}  <color=yellow>+ {bonus.amount} {bonus.source}</color>";
        else if (bonus.amount < 0)
            damageTxt.text = $"{baseD}  <color=red>{bonus.amount} {bonus.source}</color>";
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
