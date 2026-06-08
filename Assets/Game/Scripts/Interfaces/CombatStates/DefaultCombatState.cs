public class DefaultCombatState : ICombatState
{
    public void EnterState(CombatManager cm)
    {
        EventBus.Publish(new ScoringEndedEvent());   
    }
    public void ExitState(CombatManager cm)
    {
        EventBus.Publish(new ScoringStartedEvent()); 
    }
    public void UpdateState(CombatManager cm) { }
}