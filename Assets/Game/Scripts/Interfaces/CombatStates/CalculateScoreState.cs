using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class CalculateScoreState : ICombatState
{
    private readonly List<ItemData> _items;
    private readonly List<int> _rollResults;
    private CancellationTokenSource _cts;

    public CalculateScoreState(List<ItemData> items, List<int> rollResults)
    {
        _items = items;
        _rollResults = rollResults;
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
                .CalculateScore(_items, _rollResults);

            if (ct.IsCancellationRequested) return;

            // ? Scoring done — transition to AttackState
            cm.SwitchState(new AttackState());
        }
        catch (TaskCanceledException) { }
    }
}