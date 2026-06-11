using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used for the delay after an item attack misses but before the next action is taken,
/// including timing for the miss animation and transitioning to the next state (second chance, quick save, or between item)
/// after the miss is revealed and any applicable upgrades are checked.
/// </summary>
public class MissState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly ScoreManager _sm;
    private float _timer;
    private float MissDelayDuration = 1.0f;

    public MissState(List<ItemData> items, int index, ScoreManager sm)
    {
        _items = items;
        _index = index;
        _sm = sm;
    }

    public void EnterState(CombatManager cm)
    {
        _timer = 0f;
        _sm.ShowMiss();

        ItemDisplayController itemDisplay = _sm.itemDisplay.gameObject.GetComponent<ItemDisplayController>();
        MissDelayDuration = itemDisplay.stateInfo.length;
    }

    public void ExitState(CombatManager cm) { }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;
        _timer += Time.deltaTime;

        if (_timer >= MissDelayDuration)
        {
            if (UpgradeFightingManager.Instance.CanUseSecondChance())
            {
                _sm.roller.upgradeNotifText.text = "Second Chance";
                cm.SwitchState(new RollState(_items, _sm.roller, _index, false));
            }
            else if (UpgradeFightingManager.Instance.CanUseQuickSave())
            {
                _sm.roller.upgradeNotifText.text = "Quick Save";
                _sm.ApplyQuickSave(_index, _sm.pendingItems[_index]);
                cm.SwitchState(new MoveLineupState(_items, _index, _sm));
            }
            else
            {
                UpgradeFightingManager.Instance.FailedAction();
                cm.SwitchState(new RemoveUsedItemState(_items, _index, _sm));
            }
        }
    }
}