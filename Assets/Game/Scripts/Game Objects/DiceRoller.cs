using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    public int RollDie()
    {
        Debug.Log("Rolling die...");
        int oneD20 = Random.Range(1, 21);
        Debug.Log("You rolled: " + oneD20);
        return oneD20;
    }
}
