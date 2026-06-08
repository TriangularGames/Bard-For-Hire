using System.Collections.Generic;
using UnityEngine;

public class BonusAttackState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly ScoreManager _sm;
    private float _timer;
    private const float NotifDuration = 1.2f;
    private bool _attacking;
    private (string name, int damage, ItemData item) _current;

    public BonusAttackState(List<ItemData> items, int index, ScoreManager sm)
    {
        _items = items;
        _index = index;
        _sm = sm;
    }

    public void EnterState(CombatManager cm)
    {
        _timer = 0f;
        _attacking = false;
        DequeueNext(cm);
    }

    public void ExitState(CombatManager cm) { }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;
        _timer += Time.deltaTime;

        if (!_attacking && _timer >= NotifDuration)
        {

            _sm.AttackEnemy(_current.item, _current.damage);
            _attacking = true;
            _timer = 0f;
        }
        else if (_attacking && _timer >= 1.5f * _sm.GameSpeed)
        {
            if (_sm.BonusAttackQueue.Count > 0)
            {
                DequeueNext(cm);
            }
            else
            {
                cm.SwitchState(new RemoveUsedItemState(_items, _index, _sm));
            }
        }
    }

    private void DequeueNext(CombatManager cm)
    {
        _current = _sm.BonusAttackQueue.Dequeue();
        _sm.roller.upgradeNotifText.text = _current.name;
        _attacking = false;
        _timer = 0f;
    }

}
