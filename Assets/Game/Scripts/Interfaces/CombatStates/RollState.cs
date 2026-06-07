using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used for rolling the die, including handling the shuffle animation and calculating the final roll result
/// </summary>
public class RollState : ICombatState
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

    // advantage rolls
    private int _advantageRollA;
    private int _advantageRollB;

    private ScoreManager _sm;

    public RollState(List<ItemData> items, DiceRoller roller, int index, bool withAdvantage)
    {
        _items = items;
        _roller = roller;
        _index = index;
        _withAdvantage = withAdvantage;
        _isSingleRoll = false;
    }

    public void EnterState(CombatManager cm)
    {
        _sm = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>();

        // Setup item display
        _sm.SetupItemDisplay(_index);

        // Calculate modifier
        _modifier = 0;
        if (!_isSingleRoll)
        {
            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.SkillProficiency))
            {
                _modifier += 2;
            }
            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.GreatWeaponMaster) && _items != null)
            {
                _modifier += Mathf.RoundToInt(_items[_index].Playable * 0.25f);
            }
            if (UpgradeFightingManager.Instance.tempDCReduce > 0)
            {
                _modifier += UpgradeFightingManager.Instance.tempDCReduce;
            }
        }

        bool gotAdvantage = _withAdvantage || UpgradeFightingManager.Instance.shadowThiefActive || UpgradeFightingManager.Instance.UseComeback();

        // Reset per-roll crit flag before each roll
        UpgradeFightingManager.Instance.rolledNat20 = false;

        if (gotAdvantage)
        {
            if (UpgradeFightingManager.Instance.UseComeback())
            {
                UpgradeFightingManager.Instance.UsingComeback();
                _sm.roller.upgradeNotifText.text = "Comeback!";
            }

            var (a, b, chosen) = _roller.RollAdvantage();
            _advantageRollA = a;
            _advantageRollB = b;
            _natRoll = chosen;

            _roller.displayAdvantage.gameObject.SetActive(true);
        }
        else
        {
            _natRoll = _roller.RollNat();
            _roller.displayAdvantage.gameObject.SetActive(false);
        }

        // Chosen natural 20 (after upgrades, before +modifier)
        if (_natRoll == 20)
            UpgradeFightingManager.Instance.rolledNat20 = true;

        // Setup shuffle animation
        _shuffleTimer = 0f;
        _shuffleElapsed = 0f;
        _shuffleInterval = _roller.numberChangeyInterval;
        _shuffleDone = false;

        _roller.display.gameObject.SetActive(true);
        _roller.displayRoll.gameObject.SetActive(true);
        if (_withAdvantage)
        {
            _roller.displayAdvantage.gameObject.SetActive(true);
        }
        _roller.display.text = "Rolling die...";

        int final = Mathf.Clamp(_natRoll + _modifier, 1, 20);

        AudioManager.Instance.PlayClip("DieRoll");
    }

    public void ExitState(CombatManager cm) { }

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
        _shuffleTimer += Time.deltaTime;

        if (_shuffleTimer >= _shuffleInterval)
        {
            _shuffleTimer = 0f;
            _shuffleElapsed += _shuffleInterval;

            // Show random number
            _roller.displayRoll.text = Random.Range(1, 21).ToString();
            _roller.displayRoll.text = Random.Range(1, 21).ToString();
            if (_withAdvantage)
                _roller.displayAdvantage.text = Random.Range(1, 21).ToString();

            // Slow down over time
            if (_shuffleElapsed > _roller.changeyDuration * 0.5f)
            {
                _shuffleInterval = Mathf.Lerp(
                    _roller.numberChangeyInterval,
                    _roller.numberChangeyInterval * 16f,
                    (_shuffleElapsed - _roller.changeyDuration * 0.6f) / (_roller.changeyDuration * 0.4f));
            }

            // Shuffle done
            if (_shuffleElapsed >= _roller.changeyDuration)
            {
                _shuffleDone = true;
                _roller.display.text = "You rolled:";
                if (_withAdvantage)
                {
                    _roller.displayRoll.text = _advantageRollA.ToString();
                    _roller.displayAdvantage.text = _advantageRollB.ToString();
                }
                else
                {
                    _roller.displayRoll.text = _natRoll.ToString();
                }
                int final = Mathf.Clamp(_natRoll + _modifier, 1, 20);

                // Transition to show modifier/crit state, pass result forward
                cm.SwitchState(new ShowRollResultState(_sm, _roller, _natRoll, _modifier, final, _items, _index, _isSingleRoll, _onSingleRollDone));
            }
        }
    }
}