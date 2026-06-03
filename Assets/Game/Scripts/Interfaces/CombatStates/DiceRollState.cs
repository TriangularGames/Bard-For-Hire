using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DiceRollState : ICombatState
{
    private readonly DiceRoller _roller;
    private readonly List<ItemData> _items;
    private readonly int _modifier;
    private readonly bool _withAdvantage;
    private CancellationTokenSource _cts;

    // ✅ Used by DiceRoller.RollDie / RollWithAdvantage — single roll
    public Task<int> RollTask => _rollTCS.Task;
    private readonly TaskCompletionSource<int> _rollTCS = new TaskCompletionSource<int>();
    public DiceRollState(DiceRoller roller, int modifier, bool withAdvantage)
    {
        _roller = roller;
        _modifier = modifier;
        _withAdvantage = withAdvantage;
        _items = null;
    }

    // ✅ Used by ItemManager — rolls for all items then transitions to CalculateScoreState
    public DiceRollState(List<ItemData> items, DiceRoller roller)
    {
        _items = items;
        _roller = roller;
    }

    public void EnterState(CombatManager cm)
    {
        _cts = new CancellationTokenSource();

        if (_items != null)
            ExecuteAllRolls(cm, _cts.Token);
        else
            ExecuteRoll(_cts.Token);
    }

    public void ExitState(CombatManager cm)
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void UpdateState(CombatManager cm) { }

    // Single roll — used by DiceRoller.RollDie/RollWithAdvantage
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

    // Multi-item roll — used by ItemManager, handles all modifier/advantage logic
    private async void ExecuteAllRolls(CombatManager cm, CancellationToken ct)
    {
        try
        {
            List<int> rollResults = new List<int>();

            for (int i = 0; i < _items.Count; i++)
            {
                if (ct.IsCancellationRequested) return;

                // ✅ Modifier logic lives here now, not in ScoreManager
                int modifier = 0;
                if (UpgradeManager.Instance.HasUpgrade(UpgradeID.SkillProficiency))
                    modifier += 2;
                if (UpgradeFightingManager.Instance.tempDCReduce > 0)
                    modifier += UpgradeFightingManager.Instance.tempDCReduce;

                // ✅ Advantage logic lives here now, not in ScoreManager
                int result;
                if (UpgradeManager.Instance.HasUpgrade(UpgradeID.EarlyAdvantage) && i == 0)
                    result = await _roller.ExecuteRollWithAdvantage(modifier, ct);
                else
                    result = await _roller.ExecuteRollDie(modifier, ct);

                rollResults.Add(result);
            }

            if (ct.IsCancellationRequested) return;

            // ✅ All rolls done — hand off to CalculateScoreState with results
            cm.SwitchState(new CalculateScoreState(_items, rollResults));
        }
        catch (TaskCanceledException) { }
    }
}