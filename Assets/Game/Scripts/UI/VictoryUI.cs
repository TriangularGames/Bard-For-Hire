using System;
using TMPro;
using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    [SerializeField] TMP_Text coinRewardTxt;

    private void OnEnable()
    {
        EventBus.Subscribe<VictoryEvent>(SetText);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<VictoryEvent>(SetText);
    }

    private void SetText(VictoryEvent e)
    {
        coinRewardTxt.text = e.textContent;
    }

    public void GoToShop()
    {
        GameManager.Instance.SwitchState(new ShopState());
        MenuManager.Instance.SwitchState(new DefaultMenuState());
    }
}
