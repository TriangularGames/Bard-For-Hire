using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used for showing the result of a dice roll, including the natural roll, modifier, and final value.
/// Handles timing for revealing the result and transitions to the next state based on whether
/// it's a single roll or part of a multi-item attack sequence.
/// </summary>
public class ShowRollResultState : ICombatState
{
    private readonly DiceRoller _roller;
    private readonly int _nat;
    private readonly int _modifier;
    private readonly int _final;
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly bool _isSingleRoll;
    private readonly System.Action<int> _onSingleRollDone;

    private float _timer;
    private float _duration;

    public ShowRollResultState(DiceRoller roller, int nat, int modifier, int final, List<ItemData> items, int index, bool isSingleRoll, System.Action<int> onDone)
    {
        _roller = roller;
        _nat = nat;
        _modifier = modifier;
        _final = final;
        _items = items;
        _index = index;
        _isSingleRoll = isSingleRoll;
        _onSingleRollDone = onDone;
    }

    public void EnterState(CombatManager cm)
    {
        _timer = 0f;

        if (_modifier != 0)
        {
            _roller.displayModifier.text = $"+ {_modifier}";
            _duration = _roller.revealPause * 0.4f;
        }
        else
        {
            _duration = _roller.revealPause * 0.8f;
        }

        // Show crit/miss
        if (_nat == 20)
        {
            _roller.displayRoll.color = Color.yellow;
            _roller.displayCrit.text = "CRITICAL HIT!";
            _roller.displayCrit.color = Color.yellow;
        }
        else if (_nat == 1)
        {
            _roller.displayRoll.color = Color.red;
            _roller.displayCrit.text = "CRITICAL MISS!";
            _roller.displayCrit.color = Color.red;
        }
    }

    public void ExitState(CombatManager cm) { }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;

        _timer += Time.deltaTime;
        if (_timer >= _duration)
        {
            // Show final value
            _roller.displayRoll.text = _final.ToString();
            _roller.displayRoll.color = Color.white;
            _roller.displayCrit.text = "";
            _roller.displayModifier.text = "";

            if (_isSingleRoll)
            {
                _onSingleRollDone?.Invoke(_final);
            }
            else
            {
                cm.SwitchState(new CalculateScoreState(_items, _index, _final));
            }
        }
    }
}