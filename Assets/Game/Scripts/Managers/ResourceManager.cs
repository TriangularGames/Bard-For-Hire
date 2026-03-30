using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public NoteData[] NoteData;
    public AudienceMemberData[] AudienceMemberData;
    private void Start()
    {
        NoteData = Resources.LoadAll<NoteData>("ScriptableObjects/Notes");
        AudienceMemberData = Resources.LoadAll<AudienceMemberData>("ScriptableObjects/Audience");
    }
}
