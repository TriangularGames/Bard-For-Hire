using UnityEngine;

public class DestroyText : MonoBehaviour
{
    private ObjectPool _pool;

    public Vector3 Offset = new Vector3(0.6f, 1f, 0);
    public Vector3 Slide = new Vector3(0, 0.09f, 0);

    public void Setup(ObjectPool pool)
    {
        _pool = pool;
        transform.localPosition = Offset;
    }

    private void FixedUpdate()
    {
        transform.localPosition += Slide;
    }

    public void ReturnToPool()
    {
        _pool.ReturnObject(gameObject);
    }
}
