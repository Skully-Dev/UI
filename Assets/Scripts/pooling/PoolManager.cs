using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //prefab you want to spawn
    public GameObject resourceSource;
    //list of objects spawned
    public List<GameObject> spawnedResources;
    //list of objects pooled
    public List<GameObject> pool;

    public bool isSpawnedAsChild = false;

    public GameObject Spawn()
    {
        GameObject spawnGameObject;

        if (pool.Count == 0)
        {
            spawnGameObject = (GameObject)Instantiate(resourceSource);
        }
        else
        {
            pool[0].SetActive(true);
            spawnGameObject = pool[0];
            pool.RemoveAt(0);
        }

        if (isSpawnedAsChild)
        {
            spawnGameObject.transform.SetParent(transform);
        }

        spawnedResources.Add(spawnGameObject);
        return spawnGameObject;
    }

    public void Despawn(GameObject despawnObject)
    {
        if (despawnObject != null)
        {
            pool.Add(despawnObject);

            spawnedResources.Remove(despawnObject);

            despawnObject.SetActive(false);
        }
    }
}
