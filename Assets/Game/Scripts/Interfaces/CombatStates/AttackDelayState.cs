using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used for applying the attack damage after a hit, including timing for the attack animation and transitioning to the next state (between item or next item) after the attack is applied.
/// </summary>
public class AttackDelayState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly int _finalRoll;
    private readonly int _totalDamage;
    private readonly ScoreManager _sm;
    private float _timer;

    public AttackDelayState(List<ItemData> items, int index, int finalRoll, int totalDamage, ScoreManager sm)
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
        _timer += Time.unscaledDeltaTime;

        if (_timer >= 0.1f * _sm.GameSpeed)
        {
            ItemData item = _sm.pendingItems[_index];
            _sm.ApplyAttack(_index, _totalDamage, item, _finalRoll);
            cm.SwitchState(new BetweenItemState(_items, _index, _sm));
        }
    }
}