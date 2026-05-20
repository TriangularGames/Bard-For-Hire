
public class TooltipSystem : Singleton<TooltipSystem>
{
    public Tooltip toolTip;
    public void Show(string content, string header = "", string attack = "", string roll = "")
    {
        toolTip.SetText(content, header, attack, roll);
        toolTip.gameObject.SetActive(true);
    }

    public void Hide()
    {
        toolTip.gameObject.SetActive(false);
    }
}
