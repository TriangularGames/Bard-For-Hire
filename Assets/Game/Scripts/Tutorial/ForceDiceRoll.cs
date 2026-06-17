using UnityEngine;

public class ForceDiceRoll : Singleton<ForceDiceRoll>
{
    public bool forceSuccess;
    public bool forceFirstRoll;
    private int[] forcedRolls = { 4, 20, 12, 1, 13, 11, 15, 17, 19, 20, 12, 16 }; // Forced rolls for tutorial
    private int rollIndex = 0;
    private bool useForcedSequence = false;

    public void StartForcedSequence() // Starts the sequence of forced rolls
    {
        useForcedSequence = true;
        rollIndex = 0;
    }

    public bool WeOverrideNow() => useForcedSequence || forceSuccess; // Should we override rolls?

    public int GetOverrideRoll(int naturalRoll) // Gets the roll to override
    {
        if (useForcedSequence && rollIndex < forcedRolls.Length) // Use the forced rolls if there are forced rolls left
        {
            int roll = forcedRolls[rollIndex];
            rollIndex++;
            if (rollIndex >= forcedRolls.Length)
                useForcedSequence = false;
            return roll;
        }

        return naturalRoll;
    }
}
