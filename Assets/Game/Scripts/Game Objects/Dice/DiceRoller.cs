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

    [Header("Roll Forces")]
    [SerializeField] private Vector2 lateralForceRange = new Vector2(3.5f, 6.0f);
    [SerializeField] private Vector2 upwardForceRange = new Vector2(1.0f, 2.2f);
    [SerializeField] private Vector2 torqueRange = new Vector2(8f, 20f);

    [Header("Timing")]
    [SerializeField] private float maxRollDuration = 5f;
    [SerializeField] private float postSettleDelay = 0.15f;

    private bool isRolling;

    public void RollDie(MonoBehaviour runner, Action<int> onResult)
    {
        if (isRolling) return;
        runner.StartCoroutine(RollRoutine(onResult));
    }

    private IEnumerator RollRoutine(Action<int> onResult)
    {
        isRolling = true;
        display.text = "Rolling die...";

        Dice die = Instantiate(
            diePrefab,
            spawnPoint.position,
            UnityEngine.Random.rotation
        );

        Rigidbody rb = die.GetComponent<Rigidbody>();
        die.ResetSettleState();

        Vector3 lateralDir = UnityEngine.Random.insideUnitSphere;
        lateralDir.y = 0f;
        lateralDir = lateralDir.normalized;

        float lateral = UnityEngine.Random.Range(lateralForceRange.x, lateralForceRange.y);
        float upward = UnityEngine.Random.Range(upwardForceRange.x, upwardForceRange.y);

        // Vector3 impulse = lateralDir * lateral + Vector3.up * upward;
        // rb.AddForce(impulse, ForceMode.Impulse);

        Vector3 torque = new Vector3(
            UnityEngine.Random.Range(torqueRange.x, torqueRange.y),
            UnityEngine.Random.Range(torqueRange.x, torqueRange.y),
            UnityEngine.Random.Range(torqueRange.x, torqueRange.y)
        );
        rb.AddTorque(torque, ForceMode.Impulse);

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

        int value = die.GetTopFaceValue();
        display.text = $"You rolled: {value}";
        onResult?.Invoke(value);

        // Optional: keep die for visuals, or cleanup:
        // Destroy(die.gameObject, 2f);

        isRolling = false;
    }
}