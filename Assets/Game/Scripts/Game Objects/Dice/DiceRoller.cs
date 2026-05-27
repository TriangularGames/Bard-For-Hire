using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [SerializeField] private TMP_Text display;

    public async Task<int> RollDie(int GameSpeed)
    {
        Debug.Log("Rolling die...");
        display.text = "Rolling die...";
        await Task.Delay(600 * GameSpeed);
        int roll = RollOnce();
        Debug.Log("You rolled: " + roll);
        display.text = "You rolled: " + roll;
        return roll;
    }

    public int RollOnce()
    {
        int roll = UnityEngine.Random.Range(1, 21);

        // Natural20: doubles the chance of rolling a 20
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.Natural20))
        {
            int second = UnityEngine.Random.Range(1, 21);
            if (second == 20) roll = 20;
        }

        // WeightedDice: rolls above 10 become 50% more likely
        if (UpgradeManager.Instance.HasUpgrade(UpgradeID.WeightedDice))
        {
            int second = UnityEngine.Random.Range(1, 21);
            roll = Mathf.Max(roll, second);
        }

        return roll;
    }

    // EarlyAdvantage: rolls 2 dice and takes the higher (advantage)
    public async Task<int> RollWithAdvantage()
    {
        display.text = "Rolling with advantage!!!!";
        await Task.Delay(600);

        int a = UnityEngine.Random.Range(1, 21);
        int b = UnityEngine.Random.Range(1, 21);
        int roll = Mathf.Max(a, b);

        display.text = "You rolled: " + roll + " (advantage)";
        return roll;
    }

    public void ResetText()
    {
        display.text = "";
    }
}