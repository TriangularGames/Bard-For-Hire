using System.Collections.Generic;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    private IMenuState currentState;
    private Stack<IMenuState> stateHistory;
    public IMenuState GetCurrentState() { return currentState; }

    public override void Awake()
    {
        base.Awake();

        stateHistory = new Stack<IMenuState>();
    }

    public void SwitchState(IMenuState newState, bool addToHistory = true)
    {
        currentState?.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);

        if (addToHistory) stateHistory.Push(currentState); // Add state to history only if not resuming previous state
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
        else currentState.ExitState(this);
    }
}
