using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// State used for the second chance option after an item attack misses, including timing for the second chance animation and transitioning to the next state (dice roll) after the second chance is revealed and the re-roll is triggered.
/// </summary>
public class SecondChanceState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly ScoreManager _sm;
    private float _timer;
    private bool _rolled;

    public SecondChanceState(List<ItemData> items, int index, ScoreManager sm)
    {
        _items = items;
        _index = index;
        _sm = sm;
    }

    public void EnterState(CombatManager cm)
    {
        _timer = 0f;
        _sm.roller.upgradeNotifText.text = "Second Chance";
    }

    public void ExitState(CombatManager cm)
    {
        _sm.roller.upgradeNotifText.text = "";
    }

    public void UpdateState(CombatManager cm)
    {
        if (PauseManager.Instance.IsPaused) return;
        _timer += Time.unscaledDeltaTime;

        if (_timer >= 0.8f && !_rolled)
        {
            _rolled = true;
            _sm.roller.RollDie(0, result =>
            {
                CombatManager.Instance.SwitchState(new CalculateScoreState(_items, _index, result));
            });
        }
    }
}