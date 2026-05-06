using UnityEngine;

[CreateAssetMenu(fileName = "Note", menuName = "Scriptable Objects/Note")]
public class NoteData : ScriptableObject
{
    [Header("Note Type")]
    [SerializeField] public NoteType NoteType;
    [SerializeField] public bool Rest;

    [Header("Scoring Value")]
    [SerializeField] public int Score;
    [SerializeField] public bool Mult;

    [Header("Playable Value")]
    [SerializeField] public int Playable;

    [Header("Visual Data")]
    [SerializeField] public Sprite icon;

    [Header("Purchase Cost")]
    [SerializeField] public int cost;
}
