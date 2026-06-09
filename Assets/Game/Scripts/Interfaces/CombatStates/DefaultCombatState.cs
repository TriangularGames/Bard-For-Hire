public class DefaultCombatState : ICombatState
{
    public void EnterState(CombatManager cm)
    {
        EventBus.Publish(new RoundEndedEvent());   
    }
    public void ExitState(CombatManager cm)
    {
        EventBus.Publish(new RoundStartedEvent()); 
    }
    public void UpdateState(CombatManager cm) { }
}