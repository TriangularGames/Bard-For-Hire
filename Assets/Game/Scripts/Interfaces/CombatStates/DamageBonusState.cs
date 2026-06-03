using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used for applying and showing damage bonuses after an item attack hits, including timing for revealing each bonus and updating the displayed damage accordingly. Transitions to the next state (attack delay) after all bonuses have been processed.
/// </summary>
public class DamageBonusState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly int _finalRoll;
    private readonly int _totalDamage;
    private readonly List<UpgradeFightingManager.DamageBonus> _bonuses;
    private readonly ScoreManager _sm;

    private int _baseDamage;
    private int _bonusIndex;
    private float _timer;
    private bool _showingBonus;

    public DamageBonusState(List<ItemData> items, int index, int finalRoll, int totalDamage, List<UpgradeFightingManager.DamageBonus> bonuses, int baseDamage, ScoreManager sm)
    {
        _items = items;
        _index = index;
        _finalRoll = finalRoll;
        _totalDamage = totalDamage;
        _bonuses = bonuses;
        _baseDamage = baseDamage;
        _sm = sm;
    }

    public void EnterState(CombatManager cm)
    {
        _bonusIndex = 0;
        _timer = 0f;
        _showingBonus = false;

        // Set initial damage text
        var ic = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>().pendingItems[_index];
        SetDamageText(_baseDamage);
    }

    public void ExitState(CombatManager cm) { }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;

        if (_bonuses == null || _bonusIndex >= _bonuses.Count)
        {
            FinishBonuses(cm);
            return;
        }

        if (_bonusIndex >= _bonuses.Count) return;

        var bonus = _bonuses[_bonusIndex];
        if (bonus.amount <= 0) { _bonusIndex++; return; }

        _timer += Time.unscaledDeltaTime;

        if (!_showingBonus && _timer >= 0.4f)
        {
            // Show bonus text
            var itemDisplay = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>().pendingItems[_index];
            SetBonusText(_baseDamage, bonus);
            _showingBonus = true;
            _timer = 0f;
        }
        else if (_showingBonus && _timer >= 0.7f)
        {
            _baseDamage += bonus.amount;
            SetDamageText(_baseDamage);
            _showingBonus = false;
            _timer = 0f;
            _bonusIndex++;

            if (_bonusIndex >= _bonuses.Count)
            {
                FinishBonuses(cm);
            }
        }
    }

    private void SetDamageText(int value)
    {
        // Access itemDisplay's damageTxt via ItemController
        var sm = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();
        sm.pendingItems[_index].ToString(); // just to reference — update to match your ItemController access
    }

    private void SetBonusText(int baseD, UpgradeFightingManager.DamageBonus bonus)
    {
        // Same as above — update to match your ItemController's damageTxt reference
    }

    private void FinishBonuses(CombatManager cm)
    {
        cm.SwitchState(new AttackDelayState(_items, _index, _finalRoll, _totalDamage, _sm));
    }
}