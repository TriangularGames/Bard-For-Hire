
public class TooltipSystem : Singleton<TooltipSystem>
{
    public Tooltip toolTip;
    public void Show(string content, string header = "")
    {
        toolTip.SetText(content, header);
        toolTip.gameObject.SetActive(true);
    }

    public void Hide()
    {
        toolTip.gameObject.SetActive(false);
    }
}
