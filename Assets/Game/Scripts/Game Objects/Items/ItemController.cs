using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Threading.Tasks;
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

    public void ShowBonusLabel(int currentBase, int amount, string source)
    {
        if (amount >= 0)
        {
            damageTxt.text = $"Dmg {currentBase}  <color=yellow>+{amount} {source}</color>";
        }
        else
        {
            damageTxt.text = $"Dmg {currentBase}  <color=red>{amount} {source}</color>"; ;
        }
    }

    public void SetDamageTxtRaw(int value)
    {
        damageTxt.text = ("Dmg ") + value.ToString();
    }

    private void SetSprite()
    {
        if (transform.childCount == 3)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = itemData.icon;
        }
        else
        {
            transform.GetChild(1).GetComponent<Image>().sprite = itemData.icon;
        }
        
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

    private void SetPlayableTxt()
    {
        playableText.text = "Roll (" + itemData.Playable.ToString() + ")";
    }
}
