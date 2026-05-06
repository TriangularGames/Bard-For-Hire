using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Dice : MonoBehaviour
{
    [Header("Settle Detection")]
    [SerializeField] private float linearSleepThreshold = 0.05f;
    [SerializeField] private float angularSleepThreshold = 0.05f;
    [SerializeField] private float settleTimeRequired = 0.35f;

    [Header("Gravity")]
    [Tooltip("If enabled, sets Physics.gravity at runtime.")]
    [SerializeField] private bool overrideGlobalGravity = false;
    [SerializeField] private Vector3 customGravity = new Vector3(0f, 0f, 9.81f);

    [Header("Auto Face Generation (Editor)")]
    [SerializeField] private Transform meshRoot;
    [SerializeField] private Transform facesRoot;
    [SerializeField, Range(0.90f, 0.9999f)] private float normalDotThreshold = 0.985f;
    [SerializeField] private bool clearExistingFaces = true;

    private Rigidbody rb;
    private readonly List<DieFace> faces = new List<DieFace>();
    private float settledTimer;

    /// <summary>
    /// Called when the object is spawned, sets up the gravity and face cache
    /// </summary>
    private void Awake()
    {
        if (overrideGlobalGravity)
        {
            Physics.gravity = customGravity;
        }

        rb = GetComponent<Rigidbody>()
             ?? GetComponentInChildren<Rigidbody>(true)
             ?? GetComponentInParent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError($"[{nameof(Dice)}] No Rigidbody found on {name} (self/children/parent).");
        }

        RebuildFaceCache();
    }

    /// <summary>
    /// Rebuilds the face cache by getting all DieFace components in children
    /// </summary>
    public void RebuildFaceCache()
    {
        faces.Clear();
        faces.AddRange(GetComponentsInChildren<DieFace>(true));
    }

    /// <summary>
    /// Resets the settle state timer
    /// </summary>
    public void ResetSettleState()
    {
        settledTimer = 0f;
    }

    /// <summary>
    /// Checks if the die is settled by checking if the linear and angular velocities are below the sleep thresholds
    /// </summary>
    /// <param name="dt">The time delta</param>
    /// <returns>True if the die is settled, false otherwise</returns>
    public bool IsSettled(float dt)
    {
        if (rb == null) return false;

        bool slowEnough =
            rb.linearVelocity.sqrMagnitude <= linearSleepThreshold * linearSleepThreshold &&
            rb.angularVelocity.sqrMagnitude <= angularSleepThreshold * angularSleepThreshold;

        settledTimer = slowEnough ? (settledTimer + dt) : 0f;
        return settledTimer >= settleTimeRequired;
    }

    /// <summary>
    /// Gets the top face value by finding the face with the highest dot product with the gravity up
    /// </summary>
    /// <returns>The top face value</returns>
    public int GetTopFaceValue()
    {
        if (faces.Count == 0)
        {
            RebuildFaceCache();
            if (faces.Count == 0)
            {
                Debug.LogWarning($"[{nameof(Dice)}] No DieFace markers found under {name}.");
                return -1;
            }
        }

        // get the gravity up vector for custom gravity
        Vector3 upRef = GetGravityUp();

        int bestValue = -1;
        float bestDot = float.NegativeInfinity;

        foreach (DieFace face in faces)
        {
            // Convention: face marker's local +Y points outward from the face.
            float dot = Vector3.Dot(face.transform.up, upRef);
            if (dot > bestDot)
            {
                bestDot = dot;
                bestValue = face.value;
            }
        }

        return bestValue;
    }

    /// <summary>
    /// Gets the gravity up vector for custom gravity
    /// </summary>
    /// <returns>The gravity up vector</returns>
    public static Vector3 GetGravityUp()
    {
        return Physics.gravity.sqrMagnitude > 0.0001f
            ? (-Physics.gravity).normalized
            : Vector3.up;
    }

    /// <summary>
    /// Gets the gravity scale by dividing the gravity magnitude by the baseline
    /// </summary>
    /// <param name="baseline">The baseline gravity</param>
    /// <returns>The gravity scale</returns>
    public static float GetGravityScale(float baseline = 9.81f)
    {
        if (baseline <= 0f) baseline = 9.81f;
        return Physics.gravity.magnitude / baseline;
    }
}