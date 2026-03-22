using UnityEngine;

/// <summary>
/// Base note class for all notes
/// </summary>
public class BaseNote : INote
{
    protected NoteType noteType;
    public NoteType NoteType => noteType;

    protected virtual void Start()
    {
        // Example of how to use the NoteType property
        Debug.Log($"This note is of type: {NoteType}");
    }
}
