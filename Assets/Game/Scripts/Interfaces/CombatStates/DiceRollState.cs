using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class DiceRollState : ICombatState
{
    private readonly List<ItemData> _items;
    private CancellationTokenSource _cts;

    // Pass the items to roll for when creating the state
    public DiceRollState(List<ItemData> items)
    {
        _items = items;
    }

    public void EnterState(CombatManager cm)
    {
        _cts = new CancellationTokenSource();
        ExecuteRoll(cm, _cts.Token);
    }

    public void ExitState(CombatManager cm)
    {
        // Cancel if we leave early e.g. enemy dies mid-roll
        _cts?.Cancel();
        _cts?.Dispose();
    }

    public void UpdateState(CombatManager cm) { }

    private void ExecuteRoll(CombatManager cm, CancellationToken ct)
    {
        Debug.Log("Rolling dice");
    }
}
