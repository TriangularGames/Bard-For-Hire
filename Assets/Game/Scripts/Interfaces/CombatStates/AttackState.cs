public class AttackState : ICombatState
{
    public void EnterState(CombatManager cm)
    {
        // Attacks already happened inside ScoreManager.OnRollComplete
        // Just return to default so player can act again
        cm.SwitchState(new DefaultCombatState());
    }

    public void ExitState(CombatManager cm) { }
    public void UpdateState(CombatManager cm) { }
}