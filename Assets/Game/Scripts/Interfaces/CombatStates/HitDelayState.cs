using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used for the delay after an item attack hits but before the damage bonuses are shown,
/// including timing for the hit animation and transitioning to the next state (damage bonus) after the hit is revealed.
/// </summary>
public class HitDelayState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly int _finalRoll;
    private readonly ScoreManager _sm;
    private float _timer;
    private float HitDelayDuration = 0.3f;

    public HitDelayState(List<ItemData> items, int index, int finalRoll, ScoreManager sm)
    {
        _items = items;
        _index = index;
        _finalRoll = finalRoll;
        _sm = sm;
    }

    public void EnterState(CombatManager cm)
    {
        _timer = 0f;
        _sm.ShowHit(_index);
    }

    public void ExitState(CombatManager cm) { }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;
        _timer += Time.deltaTime;

        if (_timer >= HitDelayDuration * _sm.GameSpeed)
        {
            ItemData item = _sm.pendingItems[_index];
            int totalDamage = UpgradeFightingManager.Instance.GetBonusDamage(item, _index, out var bonuses);
            cm.SwitchState(new DamageBonusState(_items, _index, _finalRoll, totalDamage, bonuses, item.Damage, _sm));
        }
    }
}