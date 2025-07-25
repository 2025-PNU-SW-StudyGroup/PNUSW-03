using UnityEngine;
using System.Collections.Generic;

public class TileSpawner : MonoBehaviour
{
    [Header("Tile Settings")]
    public GameObject[] tilePrefabs;
    public int initialTileCount = 10;
    public float tileSpacing = 5f;
    public float moveSpeed = 5f;
    public float destroyZ = -30f;

    [HideInInspector] public Vector3 moveDirection = Vector3.back;

    public ObstacleFadeManager fadeManager;

    private Queue<GameObject> spawnQueue = new();
    private List<GameObject> spawnedTiles = new();
    private GameObject lastTile;
    private bool spawningFinished = false;

    private void Start()
    {
        foreach (var prefab in tilePrefabs)
        {
            spawnQueue.Enqueue(prefab);
        }

        SpawnInitialTiles();
    }

    private void SpawnInitialTiles()
    {
        Vector3 spawnPos = transform.position;

        for (int i = 0; i < initialTileCount; i++)
        {
            SpawnTileAt(spawnPos);
            spawnPos += Vector3.forward * tileSpacing;
        }
    }

    private void SpawnTileAt(Vector3 position)
    {
        if (spawnQueue.Count == 0)
        {
            spawningFinished = true;
            return;
        }

        GameObject prefab = spawnQueue.Dequeue();
        GameObject tile = Instantiate(prefab, position, Quaternion.identity, transform);
        tile.name = $"Tile_{spawnedTiles.Count:D2}";

        TileRunner runner = tile.GetComponent<TileRunner>();
        if (runner != null)
        {
            runner.spawner = this;
        }

        RegisterObstacles(tile);

        spawnedTiles.Add(tile);
        lastTile = tile;
    }

    private void RegisterObstacles(GameObject tile)
    {
        foreach (Transform child in tile.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag("Obstacle"))
            {
                Renderer[] renderers = child.GetComponentsInChildren<Renderer>();
                foreach (Renderer r in renderers)
                {
                    fadeManager.RegisterObstacle(r);
                }
            }
        }
    }

    public void RecycleTile(TileRunner tile)
    {
        if (spawningFinished)
            return;

        Vector3 newPos = lastTile.transform.position + Vector3.forward * tileSpacing;
        SpawnTileAt(newPos);

        Destroy(tile.gameObject); // 더 이상 재활용하지 않고 제거
    }
}