using UnityEngine;
using System.Collections.Generic;


public class GameObjectPool : MonoBehaviour
{
    [Header("Pool Settings")]
    [Tooltip("Prefab to instantiate if the pool is empty.")]
    [SerializeField] private GameObject prefab; // Prefab to instantiate if pool is empty

    [Header("Pool Status")]
    [Tooltip("The queue holding pooled GameObjects.")]
    private Queue<GameObject> poolQueue = new Queue<GameObject>();

    [Tooltip("Singleton instance of the GameObjectPool.")]
    public static GameObjectPool Instance;

    #region Unity Methods

    /// <summary>
    /// Called when the script instance is being loaded. Sets up the singleton instance.
    /// </summary>
    void Awake()
    {
        Instance = this;
    }

    #endregion

    #region Pool Management

    /// <summary>
    /// Retrieves a GameObject from the pool if available or instantiates a new one if not.
    /// </summary>
    /// <param name="gameobject">The requested GameObject type (usually the prefab).</param>
    /// <returns>A GameObject instance from the pool or a new one if the pool is empty.</returns>
    public GameObject GetFromPool(GameObject gameobject)
    {
        if (poolQueue.Count > 0)
        {
            var obj = poolQueue.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(prefab);
    }

    /// <summary>
    /// Returns a GameObject to the pool and deactivates it.
    /// </summary>
    /// <param name="obj">The GameObject to return to the pool.</param>
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }

    #endregion
}