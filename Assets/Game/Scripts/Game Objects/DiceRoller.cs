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
    public async Task<int> RollDie()
    {
        display.text = "Rolling die...";
        await Task.Delay(500);
        int oneD20 = Random.Range(1, 21);
        display.text = "You rolled: " + oneD20;
        return oneD20;
    }
}
