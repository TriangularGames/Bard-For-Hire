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
        transform.GetChild(0).GetComponent<Image>().sprite = itemData.icon;
    }

    private void SetDamageTxt()
    {
        if (itemData.Mult)
        {
            damageTxt.text = "x" + itemData.Damage.ToString();
        }
        else
        {
            damageTxt.text = "Dmg " + ((itemData.Damage + itemData.bonusDamageStacks));
        }
        
    }

    public void SetDamageDisplay(int value)
    {
        damageTxt.text = "Dmg " + value;
    }

    public void DisableText()
    {
        damageTxt.text = "";
        playableText.text = "";
    }

    public void SetDamageWithBonus(int baseD, UpgradeFightingManager.DamageBonus bonus)
    {
        if (bonus.amount > 0)
            damageTxt.text = $"Dmg {baseD}  <color=yellow>+ {bonus.amount} {bonus.source}</color>";
        else if (bonus.amount < 0)
            damageTxt.text = $"Dmg {baseD}  <color=red>{bonus.amount} {bonus.source}</color>";
    }

    private void SetPlayableTxt()
    {
        playableText.text = "Roll (" + itemData.Playable.ToString() + ")";
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
