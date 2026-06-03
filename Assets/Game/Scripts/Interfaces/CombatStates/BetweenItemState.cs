using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used after an item attack to show the result before moving to the next item or ending the combat phase. Handles timing and checks for end of combat conditions.
/// </summary>
public class BetweenItemState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly ScoreManager _sm;
    private float _timer;

    public BetweenItemState(List<ItemData> items, int index, ScoreManager sm)
    {
        _items = items;
        _index = index;
        _sm = sm;
    }

    public void EnterState(CombatManager cm) => _timer = 0f;
    public void ExitState(CombatManager cm) { }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;
        _timer += Time.unscaledDeltaTime;

        if (_timer >= 0.8f * _sm.GameSpeed)
        {
            if (!EnemyManager.Instance.AreEnemiesAlive())
            {
                _sm.FinalizeScore();
                cm.SwitchState(new DefaultCombatState());
                return;
            }

            if (_index + 1 < _items.Count)
            {
                // Next item
                cm.SwitchState(new DiceRollState(_items, _sm.roller, _index + 1));
            }
            else
            {
                // All items done
                _sm.FinalizeScore();
                cm.SwitchState(new DefaultCombatState());
            }
        }
    }
}