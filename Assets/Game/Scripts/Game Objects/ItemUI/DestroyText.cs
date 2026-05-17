using UnityEngine;

public class DestroyText : MonoBehaviour
{
    public float DestroyTime = 1.0f;
    public Vector2 Offset = new Vector2(0, 1.5f);
    public Vector2 RandomizeIntensity = new Vector2(1f, 0);

    void Start()
    {
        Destroy(gameObject, DestroyTime);
        GetComponent<RectTransform>().anchoredPosition += Offset;
        GetComponent<RectTransform>().anchoredPosition += new Vector2(Random.Range(-RandomizeIntensity.x, RandomizeIntensity.x),
            Random.Range(-RandomizeIntensity.y, RandomizeIntensity.y));
    }
}
