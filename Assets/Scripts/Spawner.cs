using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject prefab;
    public Vector3 spawnPoint;
    public float min = 1.0f;
    public float max = 3.5f;

    private void Start()
    {
        Invoke("Spawn", Random.Range(min, max));
    }

    private void Spawn()
    {
        GameObject gObj = Instantiate(prefab);
        gObj.transform.position = spawnPoint;
        Invoke("Spawn", Random.Range(min, max));
    }
}
