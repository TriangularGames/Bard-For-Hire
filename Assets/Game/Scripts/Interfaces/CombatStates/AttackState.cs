using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used for applying the attack damage after a hit, including timing for the attack animation and
/// transitioning to the next state (between item or next item) after the attack is applied.
/// </summary>
public class AttackState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly int _finalRoll;
    private readonly int _totalDamage;
    private readonly ScoreManager _sm;
    private float _timer;
    private const float AttackDelayDuration = 1.0f;


    public AttackState(List<ItemData> items, int index, int finalRoll, int totalDamage, ScoreManager sm)
    {
        _items = items;
        _index = index;
        _finalRoll = finalRoll;
        _totalDamage = totalDamage;
        _sm = sm;
    }

    public void EnterState(CombatManager cm) => _timer = 0f;
    public void ExitState(CombatManager cm) { }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;
        _timer += Time.deltaTime;

        if (_timer >= AttackDelayDuration * _sm.GameSpeed)
        {
            ItemData item = _sm.pendingItems[_index];
            _sm.ApplyAttack(_index, _totalDamage, item, _finalRoll);
        }

        if (_timer >= 1.0f * _sm.GameSpeed)
        {
            if (_sm.BonusAttackQueue.Count > 0 && _timer >= 1.0f)
                cm.SwitchState(new BonusAttackState(_items, _index, _sm));
            else
                cm.SwitchState(new RemoveUsedItemState(_items, _index, _sm));
        }
    }
}