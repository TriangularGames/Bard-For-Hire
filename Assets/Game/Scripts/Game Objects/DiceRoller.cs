using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [SerializeField] TMP_Text display;

    /// <summary>
    /// Rolls a Twenty-Sided Die
    /// </summary>
    /// <returns>Die roll value</returns>
    public int RollDie()
    {
        display.text = "Rolling die...";
        int oneD20 = Random.Range(1, 21);
        display.text = "You rolled: " + oneD20;
        return oneD20;
    }
}
