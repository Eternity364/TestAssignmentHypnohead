using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance { get; private set; }

    private Dictionary<string, List<GameObject>> pools = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple ObjectPool instances detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    public GameObject GetObject(GameObject prefab, bool setActive = true)
    {
        string key = prefab.name;

        if (!pools.ContainsKey(key))
        {
            pools[key] = new List<GameObject>();
        }

        foreach (GameObject obj in pools[key])
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(setActive);
                return obj;
            }
        }

        GameObject newObj = Instantiate(prefab);
        newObj.name = prefab.name;
        pools[key].Add(newObj);
        newObj.SetActive(setActive);
        
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
    }
}