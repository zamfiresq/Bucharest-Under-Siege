using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject carPrefab;
    public Transform[] spawnPoints;

    void Start()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(carPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}