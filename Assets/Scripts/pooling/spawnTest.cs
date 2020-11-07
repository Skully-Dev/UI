using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnTest : MonoBehaviour
{
    [SerializeField]
    PoolManager spherePool;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GameObject theThingISpawned = spherePool.Spawn();
            //When spawning, we need to reset the values of the gameobject
            theThingISpawned.transform.position = Vector3.zero;
        }
        StartCoroutine(despawnSpheres());
        StartCoroutine(spawnSpheres());
    }

    IEnumerator despawnSpheres()
    {
        while (spherePool.spawnedResources.Count > 0)
        {
            yield return new WaitForSecondsRealtime(1f);

            spherePool.Despawn(spherePool.spawnedResources[0]);
        }
    }

    IEnumerator spawnSpheres()
    {
        while (spherePool.spawnedResources.Count > 0)
        {
            yield return new WaitForSecondsRealtime(2f);

            GameObject theThingISpawned = spherePool.Spawn();
            //When spawning, we need to reset the values of the gameobject
            theThingISpawned.transform.position = Vector3.zero;

        }
    }
}
