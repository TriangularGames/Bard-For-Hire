using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// State used for showing the result of a dice roll, including the natural roll, modifier, and final value.
/// Handles timing for revealing the result and transitions to the next state based on whether
/// it's a single roll or part of a multi-item attack sequence.
/// </summary>
public class ShowRollResultState : ICombatState
{
    private readonly ScoreManager _sm;
    private readonly DiceRoller _roller;
    private readonly int _nat;
    private readonly int _modifier;
    private readonly int _final;
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly bool _isSingleRoll;
    private readonly System.Action<int> _onSingleRollDone;
    private readonly bool _usedAdvantage;
    private readonly int _advantageA;
    private readonly int _advantageB;
    private TMP_Text _targetDisplay;
    private TMP_Text _targetBonusDisplay;
    private float _timer;
    private float _showModifierDuration;
    private float _applyModifierDuration;
    private bool _modifierShown;
    public ShowRollResultState(ScoreManager sm, DiceRoller roller, int nat, int modifier,
        int final, List<ItemData> items, int index, bool isSingleRoll, System.Action<int> onDone, int advantageA, int advantageB, bool usedAdvantage)
    {
        _sm = sm;
        _roller = roller;
        _nat = nat;
        _modifier = modifier;
        _final = final;
        _items = items;
        _index = index;
        _isSingleRoll = isSingleRoll;
        _onSingleRollDone = onDone;
        _advantageA = advantageA;
        _advantageB = advantageB;
        _usedAdvantage = usedAdvantage;
    }

    public void EnterState(CombatManager cm)
    {
        _timer = 0f;
        _modifierShown = false;

        if (_usedAdvantage)
        {
            _targetDisplay = (_advantageA >= _advantageB) ? _roller.displayRoll : _roller.displayAdvantage;
            _targetBonusDisplay = (_advantageA >= _advantageB) ? _roller.displayModifier : _roller.displayAdvantageModifier;
        }
        else
        {
            _targetDisplay = _roller.displayRoll;
            _targetBonusDisplay = _roller.displayModifier;
        }

        bool hasModifier = _modifier != 0 && _nat != 1 && _nat != 20;

        _showModifierDuration = hasModifier ? _roller.revealPause * 0.4f : 0f;
        _applyModifierDuration = hasModifier ? _roller.revealPause * 0.4f : _roller.revealPause * 0.8f;

        if (_usedAdvantage)
        {
            if (_advantageA > _advantageB)
                _roller.displayAdvantage.color = Color.red;
            else if (_advantageB > _advantageA)
                _roller.displayRoll.color = Color.red;
        }

        // Show crit/miss
        if (_nat == 20)
        {
            if (_usedAdvantage)
            {
                if (_advantageA == 20) _roller.displayRoll.color = Color.yellow;
                if (_advantageB == 20) _roller.displayAdvantage.color = Color.yellow;
            }
            else
            {
                _roller.displayRoll.color = Color.yellow;
            }
            _roller.displayCrit.text = "CRITICAL HIT!";
            AudioManager.Instance.PlayClip("Nat20");
            _roller.displayCrit.color = Color.yellow;
        }
        else if (_nat == 1)
        {
            _roller.displayRoll.color = Color.red;
            _roller.displayCrit.text = "CRITICAL MISS!";
            AudioManager.Instance.PlayClip("Nat1");
            _roller.displayCrit.color = Color.red;
        }

        if (!hasModifier)
        {
            _modifierShown = true;
        }
    }

    public void ExitState(CombatManager cm) { }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;

        _timer += Time.deltaTime;

        if (!_modifierShown)
        {
            if (_timer >= _showModifierDuration)
            {
                _targetBonusDisplay.text = $"+{_modifier}</color>";
                _modifierShown = true;
                _timer = 0f;
            }
            return;
        }

        if (_timer >= _applyModifierDuration)
        {
            // Show final Value
            _roller.displayRoll.text = "";
            if (_usedAdvantage) _roller.displayAdvantage.text = "";

            _targetBonusDisplay.text = "";
            _targetDisplay.text = _final.ToString();

            if (_usedAdvantage)
            {
                _roller.displayRoll.color = Color.white;
                _roller.displayAdvantage.color = Color.white;
            }

            _roller.displayCrit.text = "";

            if (_isSingleRoll)
            {
                _onSingleRollDone?.Invoke(_final);
            }
            else
            {
                cm.SwitchState(new CalculateScoreState(_sm, _items, _index, _final));
            }
        }
    }
}