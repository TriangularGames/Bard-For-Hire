using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DiceRollState : ICombatState
{
     private readonly DiceRoller _roller;
    private readonly int _modifier;
    private readonly bool _withAdvantage;
    private CancellationTokenSource _cts;

    // ScoreManager awaits this to get the roll result
    public Task<int> RollTask => _rollTCS.Task;
    private readonly TaskCompletionSource<int> _rollTCS = new TaskCompletionSource<int>();

    public DiceRollState(DiceRoller roller, int modifier, bool withAdvantage)
    {
        _roller = roller;
        _modifier = modifier;
        _withAdvantage = withAdvantage;
    }

    public void EnterState(CombatManager cm)
    {
        _cts = new CancellationTokenSource();
        ExecuteRoll(_cts.Token);
    }

    public void ExitState(CombatManager cm)
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void UpdateState(CombatManager cm) { }

    private async void ExecuteRoll(CancellationToken ct)
    {
        try
        {
            int result = _withAdvantage
                ? await _roller.ExecuteRollWithAdvantage(_modifier, ct) 
                : await _roller.ExecuteRollDie(_modifier, ct);         

            if (ct.IsCancellationRequested) return;
            _rollTCS.TrySetResult(result);
            CombatManager.Instance.ResumePreviousState();
        }
        catch (TaskCanceledException)
        {
            _rollTCS.TrySetCanceled();
        }
    }
}
