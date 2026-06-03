using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used for rolling the die, including handling the shuffle animation and calculating the final roll result
/// </summary>
public class DiceRollState : ICombatState
{
    private readonly DiceRoller _roller;
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly bool _withAdvantage;

    private int _natRoll;
    private float _shuffleTimer;
    private float _shuffleInterval;
    private float _shuffleElapsed;
    private bool _shuffleDone;

    // For single roll via DiceRoller.RollDie
    private int _modifier;
    private bool _isSingleRoll;
    private System.Action<int> _onSingleRollDone;

    // Single roll constructor
    public DiceRollState(DiceRoller roller, int modifier, bool withAdvantage, System.Action<int> onDone)
    {
        _roller = roller;
        _modifier = modifier;
        _withAdvantage = withAdvantage;
        _isSingleRoll = true;
        _onSingleRollDone = onDone;
        _items = null;
    }

    // Multi-item roll constructor
    public DiceRollState(List<ItemData> items, DiceRoller roller, int index = 0)
    {
        _items = items;
        _roller = roller;
        _index = index;
        _isSingleRoll = false;
    }

    public void EnterState(CombatManager cm)
    {
        // Calculate modifier
        _modifier = 0;
        if (!_isSingleRoll)
        {
            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.SkillProficiency))
                _modifier += 2;
            if (UpgradeFightingManager.Instance.tempDCReduce > 0)
                _modifier += UpgradeFightingManager.Instance.tempDCReduce;
        }

        // Roll the nat immediately
        bool useAdvantage = _withAdvantage ||
            (!_isSingleRoll && UpgradeManager.Instance.HasUpgrade(UpgradeID.EarlyAdvantage) && _index == 0);

        _natRoll = useAdvantage ? _roller.RollAdvantage() : _roller.RollNat();

        // Setup shuffle animation
        _shuffleTimer = 0f;
        _shuffleElapsed = 0f;
        _shuffleInterval = _roller.numberChangeyInterval;
        _shuffleDone = false;

        _roller.display.gameObject.SetActive(true);
        _roller.displayRoll.gameObject.SetActive(true);
        _roller.displayAdvantage.gameObject.SetActive(false);
        _roller.display.text = "Rolling die...";
        AudioManager.Instance.PlayClip("DieRoll");
    }

    public void ExitState(CombatManager cm)
    {
        
    }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;

        if (!_shuffleDone)
        {
            UpdateShuffle(cm);
            return;
        }
    }

    private void UpdateShuffle(CombatManager cm)
    {
        _shuffleTimer += Time.unscaledDeltaTime;

        if (_shuffleTimer >= _shuffleInterval)
        {
            _shuffleTimer = 0f;
            _shuffleElapsed += _shuffleInterval;

            // Show random number
            _roller.displayRoll.text = Random.Range(1, 21).ToString();

            // Slow down over time
            if (_shuffleElapsed > _roller.changeyDuration * 0.5f)
                _shuffleInterval = Mathf.Lerp(
                    _roller.numberChangeyInterval,
                    _roller.numberChangeyInterval * 16f,
                    (_shuffleElapsed - _roller.changeyDuration * 0.6f) / (_roller.changeyDuration * 0.4f));

            // Shuffle done
            if (_shuffleElapsed >= _roller.changeyDuration)
            {
                _shuffleDone = true;
                _roller.display.text = "You rolled:";
                _roller.displayRoll.text = _natRoll.ToString();

                int final = Mathf.Clamp(_natRoll + _modifier, 1, 20);

                // Transition to show modifier/crit state, pass result forward
                cm.SwitchState(new ShowRollResultState(_roller, _natRoll, _modifier, final, _items, _index, _isSingleRoll, _onSingleRollDone));
            }
        }
    }
}