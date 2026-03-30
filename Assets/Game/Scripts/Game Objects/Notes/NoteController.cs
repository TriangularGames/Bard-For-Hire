using UnityEngine;
using UnityEngine.UI;

public class NoteController : MonoBehaviour
{
    [SerializeField] public NoteData noteData;

    public void SetSprite()
    {
        GetComponent<Image>().sprite = noteData.icon;
    }
}
