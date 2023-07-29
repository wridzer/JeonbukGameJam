using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSpawner : MonoBehaviour
{
    [SerializeField] private GameObject plantPrefab;
    [SerializeField] private float spawnDelay = 5f;
    [SerializeField] private Vector3 spawnPlacement;

    private void Start()
    {
        StartCoroutine(SpawnDelay(0));
    }

    public void PlantRemoved()
    {
        StartCoroutine(SpawnDelay());
    }

    private IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        GameObject plant = Instantiate(plantPrefab, transform.position + spawnPlacement, new Quaternion(-0.330512464f, 0.625109255f, -0.625109136f, 0.330512494f));
        plant.transform.parent = transform;
    }

    private IEnumerator SpawnDelay(int delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject plant = Instantiate(plantPrefab, transform.position + spawnPlacement, new Quaternion(-0.330512464f, 0.625109255f, -0.625109136f, 0.330512494f));
        plant.transform.parent = transform;
    }
}
