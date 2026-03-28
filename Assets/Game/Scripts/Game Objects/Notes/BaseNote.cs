using UnityEngine;

/// <summary>
/// Base note class for all notes
/// </summary>
public class BaseNote : MonoBehaviour, INote
{
    [SerializeField] protected NoteType noteType;
    public NoteType NoteType => noteType;

    [SerializeField] protected int score;
    public int Score { get => score; set => SetScore(value); }

    [SerializeField] protected bool mult;
    public bool Mult => mult;

    protected virtual void Start()
    {
        // Example of how to use the NoteType property
        Debug.Log($"This note is of type: {NoteType}");
    }

    public void SetScore(int val)
    {
        score = val;
    }
}
