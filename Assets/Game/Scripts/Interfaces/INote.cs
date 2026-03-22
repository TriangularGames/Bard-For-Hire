using UnityEngine;

/// <summary>
/// Interface for all notes in the game, defining common properties and behaviors for different note types.
/// </summary>
public interface INote
{
    NoteType NoteType { get; }

}
