using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used for calculating the score of an item attack after the dice roll result is shown.
/// Determines if the attack hits or misses based on the item's requirements and the final roll result,
/// and transitions to the appropriate next state (hit or miss delay) while also checking for end of combat conditions.
/// </summary>
public class CalculateScoreState : ICombatState
{
    private readonly ScoreManager _sm;
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly int _rollResult;
    private bool _initialized;

    public CalculateScoreState(ScoreManager sm, List<ItemData> items, int index, int rollResult)
    {
        _sm = sm;
        _items = items;
        _index = index;
        _rollResult = rollResult;
    }

    public void EnterState(CombatManager cm)
    {
        _initialized = true;
    }

    public void ExitState(CombatManager cm) { }
    public void UpdateState(CombatManager cm)
    {
        if (!_initialized) return;

        if (!EnemyManager.Instance.AreEnemiesAlive())
        {
            _sm.FinalizeScore();
            cm.SwitchState(new DefaultCombatState());
            return;
        }

        int finalRoll = UpgradeFightingManager.Instance.GetBonusRoll(_rollResult);
        ItemData item = _sm.pendingItems[_index];

        if (item.Playable <= finalRoll)
        {
            cm.SwitchState(new HitState(_items, _index, finalRoll, _sm));
        }
        else
        {
            cm.SwitchState(new MissState(_items, _index, _sm));
        }

        _initialized = false;
    }
}