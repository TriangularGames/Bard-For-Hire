using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DiceRoller : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text display;
    [SerializeField] private Dice diePrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform displayPoint;

    [Header("Roll Forces")]
    [SerializeField] private Vector2 lateralForceRange = new Vector2(1.0f, 1.2f);
    [SerializeField] private Vector2 upwardForceRange = new Vector2(1.0f, 1.2f);
    [SerializeField] private Vector2 torqueRange = new Vector2(0.01f, 0.02f);

    [Header("Timing")]
    [SerializeField] private float maxRollDuration = 5f;
    [SerializeField] private float postSettleDelay = 0.15f;
    [SerializeField] private float diceLerp = 1f;
    [SerializeField] private float diceShowTimer = 3f;

    private bool isRolling;
    private float elapsedTime;
    private bool hasRolled;
    private bool shouldRemoveDice = false;
    private Dice diceRef;

    private void Update()
    {
        if (hasRolled && diceRef)
        {
            elapsedTime += Time.deltaTime;
            diceRef.transform.position = Vector3.Lerp(diceRef.transform.position, displayPoint.position, elapsedTime / diceLerp); // reset position for next roll
        }
    }

    /// <summary>
    /// Starts coroutine to roll the die and get the result, prevents multiple rolls at once
    /// </summary>
    /// <param name="runner"></param>
    /// <param name="onResult"></param>
    public void RollDie(MonoBehaviour runner, Action<int> onResult)
    {
        if (isRolling) return;
        runner.StartCoroutine(RollRoutine(onResult));
    }

    /// <summary>
    /// Rolls the die and returns the result
    /// </summary>
    /// <param name="onResult">The action to call when the roll is complete</param>
    /// <returns>The result of the roll</returns>
    private IEnumerator RollRoutine(Action<int> onResult)
    {
        isRolling = true;
        display.text = "Rolling die...";

        // spawn and initialize die at the spawn point with random rotation
        Dice die = Instantiate(
            diePrefab,
            spawnPoint.position,
            UnityEngine.Random.rotation
        );

        // get the rigidbody component and reset the settle state
        Rigidbody rb = die.GetComponent<Rigidbody>();
        die.ResetSettleState();

        // add random lateral force to make rolling look random
        Vector3 lateralDir = UnityEngine.Random.insideUnitSphere;
        lateralDir.z = 0f;
        lateralDir = lateralDir.normalized;

        float lateral = UnityEngine.Random.Range(lateralForceRange.x, lateralForceRange.y);
        float upward = UnityEngine.Random.Range(upwardForceRange.x, upwardForceRange.y);

        // add optional impulse
        Vector3 impulse = lateralDir * lateral + Vector3.up * upward;
        rb.AddForce(impulse, ForceMode.Impulse);

        // add random rolling torque to spin the dice at random speeds
        Vector3 torque = new Vector3(
            UnityEngine.Random.Range(torqueRange.x, torqueRange.y),
            UnityEngine.Random.Range(torqueRange.x, torqueRange.y),
            UnityEngine.Random.Range(torqueRange.x, torqueRange.y)
        );
        rb.AddTorque(torque, ForceMode.Impulse);

        // wait for the die to settle after rolling
        float t = 0f;
        while (t < maxRollDuration)
        {
            t += Time.deltaTime;
            if (die.IsSettled(Time.deltaTime))
            {
                yield return new WaitForSeconds(postSettleDelay);
                break;
            }
            yield return null;
        }

        // get top face value and display it
        int value = die.GetTopFaceValue();
        display.text = $"You rolled: {value}";
        hasRolled = true;
        diceRef = die;
        //die.GetComponent<Rigidbody>().isKinematic = true; // stop physics interactions
        onResult?.Invoke(value);
        StartCoroutine(HideDice(diceShowTimer));

        // Optional: keep die for visuals, or cleanup:
        // Destroy(die.gameObject, 2f);

        isRolling = false;
    }

    private IEnumerator HideDice(float timer)
    {
        yield return new WaitForSeconds(timer);
        Destroy(diceRef.gameObject);
        elapsedTime = 0f;
        display.text = "";
    }
}