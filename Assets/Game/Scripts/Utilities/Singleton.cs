using UnityEngine;
/// <summary>
/// Singleton pattern implementation
/// </summary>
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;
    private static readonly object padlock = new object();

    /// <summary>
    /// Singleton instance
    /// </summary>
    public static T Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = (T)FindFirstObjectByType(typeof(T));
                    if (FindObjectsByType(typeof(T), FindObjectsSortMode.None).Length > 1)
                    {
                        Debug.LogError("There is more than one " + typeof(T).Name + " in the scene");
                    }
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.hideFlags = HideFlags.HideAndDontSave;
                        instance = obj.AddComponent<T>();
                    }
                }
                return instance;
            }
        }
    }

    public virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}