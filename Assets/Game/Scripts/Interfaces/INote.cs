/// <summary>
/// Interface Notes, defining common properties
/// </summary>
public interface INote
{
    NoteType NoteType { get; }

    int Score { get; set; }

    public bool Mult { get; }
}
