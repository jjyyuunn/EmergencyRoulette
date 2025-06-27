using System.Collections.Generic;
using UnityEngine;

public class ModuleShopPrefabPooler : MonoBehaviour
{
    public static ModuleShopPrefabPooler Instance { get; private set; }

    public GameObject itemPrefab;
    public int initialPoolSize = 5;
    public int maxPoolSize = 10;

    public Transform poolRoot;

    private Queue<GameObject> pool = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            var obj = Instantiate(itemPrefab, poolRoot);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        GameObject obj;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = Instantiate(itemPrefab, poolRoot);
        }

        obj.transform.SetParent(null);
        obj.SetActive(true);
        return obj;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
       
        if (poolRoot != null)
            obj.transform.SetParent(poolRoot);
        else
            Debug.LogWarning("poolRoot is null!");

        if (pool.Count >= maxPoolSize)
        {
            Destroy(obj);
        }
        else
        {
            pool.Enqueue(obj);
        }
    }


    public void ClearAll()
    {
        while (pool.Count > 0)
            Destroy(pool.Dequeue());
    }
}
