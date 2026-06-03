using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public class DiceRollState : ICombatState
{
    private readonly DiceRoller _roller;
    private readonly List<ItemData> _items;
    private readonly int _index;
    private readonly int _modifier;
    private readonly bool _withAdvantage;
    private CancellationTokenSource _cts; // used to cancel async tasks when exiting state

    public Task<int> RollTask => _rollTCS.Task;
    private readonly TaskCompletionSource<int> _rollTCS = new TaskCompletionSource<int>();

    // Single roll
    public DiceRollState(DiceRoller roller, int modifier, bool withAdvantage)
    {
        _roller = roller;
        _modifier = modifier;
        _withAdvantage = withAdvantage;
        _items = null;
    }

    // Multi-item roll
    public DiceRollState(List<ItemData> items, DiceRoller roller, int index = 0)
    {
        _items = items;
        _roller = roller;
        _index = index;
    }

    public void EnterState(CombatManager cm)
    {
        _cts = new CancellationTokenSource();
        if (_items != null)
        {
            ExecuteItemRoll(cm, _cts.Token);
        }
        else
        {
            ExecuteRoll(_cts.Token);
        }
    }

    public void ExitState(CombatManager cm)
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void UpdateState(CombatManager cm) { }

    // Single roll for DiceRoller
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

    // Rolls one item then hands off to CalculateScoreState
    private async void ExecuteItemRoll(CombatManager cm, CancellationToken ct)
    {
        try
        {
            int modifier = 0;
            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.SkillProficiency))
            {
                modifier += 2;
            }
            if (UpgradeFightingManager.Instance.tempDCReduce > 0)
            {
                modifier += UpgradeFightingManager.Instance.tempDCReduce;
            }

            int result;
            if (UpgradeManager.Instance.HasUpgrade(UpgradeID.EarlyAdvantage) && _index == 0)
            {
                result = await _roller.ExecuteRollWithAdvantage(modifier, ct);
            }
            else
            {
                result = await _roller.ExecuteRollDie(modifier, ct);
            }

            if (ct.IsCancellationRequested) return;

            cm.SwitchState(new CalculateScoreState(_items, _index, result)); // Calculate score once roll is complete
        }
        catch (TaskCanceledException) { }
    }
}