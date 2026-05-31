using UnityEngine;

public class DestroyText : MonoBehaviour
{
    private ObjectPool _pool;

    public Vector3 Offset = new Vector3(0f, 0f, 0f);
    public Vector3 Slide = new Vector3(0, 0.01f, 0);

    public void Setup(ObjectPool pool)
    {
        _pool = pool;
        // TODO: fix the position to actually be near the health
        //transform.localPosition = Offset;
    }

    private void Update()
    {
        // TODO: actually get the text to slide
        // worked before adding pooling, now refuses to work
        //transform.localPosition += Slide;
    }

    public void ReturnToPool()
    {
        _pool.ReturnObject(gameObject);
    }
}
