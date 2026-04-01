using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteController : MonoBehaviour
{
    [SerializeField] public NoteData noteData;

    [SerializeField] private TMP_Text scoreTxt;
    [SerializeField] private TMP_Text playableText;

    public void Setup()
    {
        SetSprite();
        SetScoreTxt();
        SetPlayableTxt();
    }

    private void SetSprite()
    {
        GetComponent<Image>().sprite = noteData.icon;
    }

    private void SetScoreTxt()
    {
        if (noteData.Mult)
        {
            scoreTxt.text = "x" + noteData.Score.ToString();
        }
        else
        {
            scoreTxt.text = "+" + noteData.Score.ToString();
        }
        
    }

    private void SetPlayableTxt()
    {
        playableText.text = "(" + noteData.Playable.ToString() + ")";
    }
}
