using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CalculateScoreState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly int _rollResult;
    private CancellationTokenSource _cts;

    public CalculateScoreState(List<ItemData> items, int index, int rollResult)
    {
        _items = items;
        _index = index;
        _rollResult = rollResult;
    }

    public void EnterState(CombatManager cm)
    {
        _cts = new CancellationTokenSource();
        ExecuteScoring(cm, _cts.Token);
    }

    public void ExitState(CombatManager cm)
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void UpdateState(CombatManager cm) { }

    private async void ExecuteScoring(CombatManager cm, CancellationToken ct)
    {
        try
        {
            await GameObject.FindWithTag("ScoreManager")
                .GetComponent<ScoreManager>()
                .CalculateScore(_items, _index, _rollResult);

            if (ct.IsCancellationRequested) return;

            if (_index + 1 < _items.Count)
            {
                DiceRoller roller = GameObject.FindWithTag("ScoreManager").GetComponent<ScoreManager>().roller;
                cm.SwitchState(new DiceRollState(_items, roller, _index + 1)); // re-roll if more items are are played
            }
            else
            {
                cm.SwitchState(new DefaultCombatState());
            }
        }
        catch (TaskCanceledException) { }
    }
}