using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public NoteData[] NoteData;
    public UpgradeData[] UpgradeData;
    public AudienceMemberData[] AudienceMemberData;

    private void Start()
    {
        NoteData = Resources.LoadAll<NoteData>("ScriptableObjects/Notes");
        UpgradeData = Resources.LoadAll<UpgradeData>("ScriptableObjects/Upgrades");
        AudienceMemberData = Resources.LoadAll<AudienceMemberData>("ScriptableObjects/Audience");
    }
}
