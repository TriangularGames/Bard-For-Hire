/// <summary>
/// Interface Notes, defining common properties
/// </summary>
public interface INote
{
    NoteType NoteType { get; }

    int Score { get; set; }

    bool Mult { get; }

    public int Playable { get; set; }
}
