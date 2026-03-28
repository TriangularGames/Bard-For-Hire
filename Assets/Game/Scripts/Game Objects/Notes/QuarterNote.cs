using UnityEngine;

public class QuarterNote : INote
{
    protected NoteType noteType;
    public NoteType NoteType => noteType;

    public int Score { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public bool Mult => throw new System.NotImplementedException();

    private void Awake()
    {
        noteType = NoteType.Quarter;
    }
}

