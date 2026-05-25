
using System.Collections;
using UnityEngine;

public class TooltipSystem : Singleton<TooltipSystem>
{
    public Tooltip toolTip;
    public void Show(string content, string header, string rarity = "", string type = "", bool isWeakness = false, string attack = "", string roll = "")
    {
        toolTip.SetText(content, header, rarity, type, isWeakness, attack, roll);
        StartCoroutine("DelayShow");
        
    }

    public void Hide()
    {
        StopCoroutine("DelayShow");
        if (gameObject.scene.isLoaded && this != null && toolTip != null)
        {
            toolTip.gameObject.SetActive(false);
        }
    }

    IEnumerator DelayShow()
    {
        yield return new WaitForSeconds(0.5f);
        toolTip.Pivot();
        toolTip.gameObject.SetActive(true);
    }
}
