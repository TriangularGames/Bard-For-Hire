using System;
using UnityEngine;

/// <summary>
/// State for when game is in the Performance Scene
/// </summary>
public class PerformanceState : IGameState
{
    public void EnterState(GameManager gm)
    {
        EventBus.Publish<PerformanceStartEvent>(new PerformanceStartEvent());
        EventBus.Subscribe<PerformanceEndEvent>(OnPerformanceEnd);
    }

    /// <summary>
    /// When the Performance is completed,
    /// </summary>
    /// <param name="event"></param>
    private void OnPerformanceEnd(PerformanceEndEvent @event)
    {
        GameManager.Instance.SwitchState(new ShopState());
    }

    public void ExitState(GameManager gm)
    {
    }

    public void UpdateState(GameManager gm)
    {
    }
}
