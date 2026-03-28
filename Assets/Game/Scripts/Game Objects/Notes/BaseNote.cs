using UnityEngine;

/// <summary>
/// Base note class for all notes
/// </summary>
public class BaseNote : MonoBehaviour, INote
{
    /// <summary>
    /// This Note's NoteType
    /// </summary>
    [SerializeField] protected NoteType noteType;
    public NoteType NoteType => noteType;

    /// <summary>
    /// This Note's Score
    /// </summary>
    [SerializeField] protected int score;
    public int Score { get => score; set => SetScore(value); }

    /// <summary>
    /// If this Note's Score is a Mulitiplier
    /// </summary>
    [SerializeField] protected bool mult;
    public bool Mult => mult;

    /// <summary>
    /// Die roll minmum for Note to be scored
    /// </summary>
    [SerializeField] protected int playable;
    public int Playable { get => playable; set => SetPlayable(value); }

/// <summary>
/// Set the Score Value
/// </summary>
/// <param name="val">Value to Change to</param>
public void SetScore(int val)
    {
        score = val;
    }

    /// <summary>
    /// Set the Playable Value
    /// </summary>
    /// <param name="val">Value to Change to</param>
    public void SetPlayable(int val)
    {
        score = val;
    }
}
