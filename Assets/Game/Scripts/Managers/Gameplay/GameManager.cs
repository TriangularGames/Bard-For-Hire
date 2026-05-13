using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private IGameState currentState;
    private Stack<IGameState> stateHistory;
    public IGameState GetCurrentState() { return currentState; }

    public override void Awake()
    {
        base.Awake();

        stateHistory = new Stack<IGameState>();
    }

    public void SwitchState(IGameState newState, bool addToHistory = true)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);

        if (addToHistory) stateHistory.Push(currentState); // Add state to history only if not resuming previous state

        Debug.Log("Switched to state: " + currentState.GetType().Name);
    }

    public void Update()
    {
        currentState?.UpdateState(this);
    }

    public void ResumePreviousState()
    {
        Debug.Log("Resume Previous State");
        stateHistory.Pop(); // Remove current state
        if (stateHistory.Count > 0) SwitchState(stateHistory.Peek(), addToHistory: false);
    }
}
