using System.Collections.Generic;

using UnityEngine;

public class RuneRockPool : MonoBehaviour
{
    [Header("Pooling Settings")]
    public GameObject runeRockPrefab; // Prefab to pool

    [SerializeField]private List<GameObject> pool = new List<GameObject>();
    

    public GameObject GetFromPool()
    {
        if (pool.Count > 0)
        {
            GameObject rock = pool[pool.Count - 1];
            rock.SetActive(true);
            return rock;
        }
        else
        {
            // Expand the pool if needed
            GameObject rock = Instantiate(runeRockPrefab, transform);
            return rock;
        }
    }

    public void ReturnToPool(GameObject rock)
    {
        rock.SetActive(false);
        pool.Add(rock);
    }
}
