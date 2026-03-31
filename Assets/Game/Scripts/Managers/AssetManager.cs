using System.Collections.Generic;
using UnityEngine;

public class AssetManager : Singleton<AssetManager>
{
    [Header("Prefab Registry")]
    [Tooltip("Prefabs available for runtime instantiation.")]
    [SerializeField] private GameObject[] registeredPrefabs;

    private Dictionary<string, GameObject> prefabRegistry;

    public override void Awake()
    {
        base.Awake();
        InitializeRegistry();
    }

    private void InitializeRegistry()
    {
        prefabRegistry = new Dictionary<string, GameObject>();
        if (registeredPrefabs == null) return;

        foreach (var prefab in registeredPrefabs)
        {
            if (prefab != null && !prefabRegistry.ContainsKey(prefab.name))
            {
                prefabRegistry.Add(prefab.name, prefab);
            }
        }
        Debug.Log($"[AssetManager] Registered {prefabRegistry.Count} prefabs.");
    }

    /// <summary>
    /// Get the Prefab from the Registry
    /// </summary>
    /// <param name="prefabName">Name of Prefab to get</param>
    /// <returns>Prefab GameObject</returns>
    public GameObject GetPrefab(string prefabName)
    {
        if (prefabRegistry.TryGetValue(prefabName, out GameObject prefab))
        {
            return prefab;
        }
        Debug.LogWarning($"[AssetManager] Prefab '{prefabName}' not found.");
        return null;
    }

    /// <summary>
    /// Spawn a prefab at specific position
    /// </summary>
    /// <param name="prefabName">Name of Prefab to spawn</param>
    /// <param name="position">Position to spawn</param>
    /// <param name="rotation">Rotation of Object</param>
    /// <returns>Prefab GameObject</returns>
    public GameObject Spawn(string prefabName, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = GetPrefab(prefabName);
        if (prefab != null)
        {
            return Instantiate(prefab, position, rotation);
        }
        return null;
    }

    /// <summary>
    /// Spawn a Prefab parented to specific object
    /// </summary>
    /// <param name="prefabName">Name of Prefab to spawn</param>
    /// <param name="parent">Parent object to spawn as child of</param>
    /// <returns>Prefab GameObject</returns>
    public GameObject Spawn(string prefabName, Transform parent)
    {
        GameObject prefab = GetPrefab(prefabName);
        if (prefab != null)
        {
            return Instantiate(prefab, parent);
        }
        return null;
    }
}
