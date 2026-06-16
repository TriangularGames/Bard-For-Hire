using UnityEngine;
using UnityEngine.UI;

public class ForceInput : Singleton<ForceInput>
{
    private int requiredSelectionCount = 0;
    public void RequireSelectionCount(int count) // The selection amount required for tutorial
    {
        requiredSelectionCount = count;
    }
    public bool SelectionRequirementMet(int currentCount)
    {
        if (requiredSelectionCount <= 0) return true;
        return currentCount >= requiredSelectionCount;
    }

    public void ClearRequirements() // Remove those requirements
    {
        requiredSelectionCount = 0;
    }
}
