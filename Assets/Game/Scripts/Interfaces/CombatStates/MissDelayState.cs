using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used for the delay after an item attack misses but before the next action is taken, including timing for the miss animation and transitioning to the next state (second chance, quick save, or between item) after the miss is revealed and any applicable upgrades are checked.
/// </summary>
public class MissDelayState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly ScoreManager _sm;
    private float _timer;

    public MissDelayState(List<ItemData> items, int index, ScoreManager sm)
    {
        _items = items;
        _index = index;
        _sm = sm;
    }

    public void EnterState(CombatManager cm)
    {
        _timer = 0f;
        _sm.ShowMiss();
    }

    public void ExitState(CombatManager cm) { }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;
        _timer += Time.unscaledDeltaTime;

        if (_timer >= 0.1f * _sm.GameSpeed)
        {
            if (UpgradeFightingManager.Instance.CanUseSecondChance())
                cm.SwitchState(new SecondChanceState(_items, _index, _sm));
            else if (UpgradeFightingManager.Instance.CanUseQuickSave())
                cm.SwitchState(new QuickSaveState(_items, _index, _sm));
            else
            {
                UpgradeFightingManager.Instance.FailedAction();
                cm.SwitchState(new BetweenItemState(_items, _index, _sm));
            }
        }
    }
}