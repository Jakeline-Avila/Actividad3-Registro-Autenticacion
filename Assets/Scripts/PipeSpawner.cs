using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pipePrefab;
    [SerializeField] private float spawnTime = 2.5f;
    [SerializeField] private float minY = -1.5f;
    [SerializeField] private float maxY = 1.5f;

    private float timer = 1.8f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnTime)
        {
            SpawnPipe();
            timer = 0f;
        }
    }

    private void SpawnPipe()
    {
        float randomY = Random.Range(minY, maxY);

        Vector3 spawnPosition = new Vector3(
            transform.position.x,
            randomY,
            0
        );

        Instantiate(pipePrefab, spawnPosition, Quaternion.identity);
    }
}