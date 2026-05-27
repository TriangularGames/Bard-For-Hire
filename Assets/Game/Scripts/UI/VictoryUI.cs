using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    public void OnClick_GoToShop()
    {
        GameManager.Instance.SwitchState(new ShopState());
        MenuManager.Instance.SwitchState(new DefaultMenuState());
    }
}
