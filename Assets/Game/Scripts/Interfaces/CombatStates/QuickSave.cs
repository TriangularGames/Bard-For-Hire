using System.Collections.Generic;
using UnityEngine;

public class QuickSaveState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly ScoreManager _sm;
    private float _timer;

    public QuickSaveState(List<ItemData> items, int index, ScoreManager sm)
    {
        _items = items;
        _index = index;
        _sm = sm;
    }

    public void EnterState(CombatManager cm)
    {
        _timer = 0f;
        _sm.roller.upgradeNotifText.text = "Quick Save";
        _sm.ApplyQuickSave(_index, _sm.pendingItems[_index]);
    }

    public void ExitState(CombatManager cm)
    {
        _sm.roller.upgradeNotifText.text = "";
    }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;
        _timer += Time.deltaTime;

        if (_timer >= 0.8f)
            cm.SwitchState(new BetweenItemState(_items, _index, _sm));
    }
}