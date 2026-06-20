public interface ICombatState
{
    public void EnterState(CombatManager cm);
    public void UpdateState(CombatManager cm);
    public void ExitState(CombatManager cm);
}
