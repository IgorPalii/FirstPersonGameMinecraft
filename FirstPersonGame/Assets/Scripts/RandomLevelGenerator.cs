using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomLevelGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject groundPref, grassPref;
    private int baseHeight = 5, maxBlocksCountY = 25,
        chunkSize = 16, perlinNoiseSensetivity = 50, chunkCount = 2;
    private float seedX, seedY;

    [SerializeField]
    private GameObject chestPref;

    [SerializeField]
    private GameObject spawnerPref;
    private int spawnerAmount = 2;
    private List<GameObject> chunks = new List<GameObject>();

    void Start() {
        seedX = Random.Range(0, 10);
        seedY = Random.Range(0, 10);
        for (int x = 0; x < chunkCount; x++) {
            for (int z = 0; z < chunkCount; z++) {
                CreateChunk(x, z);
            }
        }

        for (int i = 0; i < spawnerAmount; i++)
        {
            CreateEnemySpawner();
        }
        BuildNavMeshOnLevel();
    }

    private void CreateChunk(int chunkNumX, int chunkNumZ) {
        GameObject chunk = new GameObject();
        chunk.transform.position =
            new Vector3(chunkNumX, 0f, chunkNumZ) * chunkSize;
        chunk.name = "chunk: " + chunkNumX * chunkSize + ", " + chunkNumZ * chunkSize;
        chunk.AddComponent<MeshFilter>();
        chunk.AddComponent<MeshRenderer>();
        chunk.AddComponent<Chunk>();

        for (int x = chunkNumX * chunkSize; x < chunkNumX * chunkSize + chunkSize; x++) {
            for (int z = chunkNumZ * chunkSize; z < chunkNumZ * chunkSize + chunkSize; z++) {
                float xSample = seedX + (float)x / perlinNoiseSensetivity;
                float ySample = seedY + (float)z / perlinNoiseSensetivity;
                float sample = Mathf.PerlinNoise(xSample, ySample);
                int height = baseHeight + (int)(sample * maxBlocksCountY);
                for (int y = 0; y < height; y++) {
                    GameObject temp;
                    if (y == height - 1) {
                        temp = Instantiate(grassPref, new Vector3(x, y, z),
                        Quaternion.identity);
                        CreateChest(x, height, z, chunk.transform);
                    }
                    else {
                        temp = Instantiate(groundPref, new Vector3(x, y, z),
                        Quaternion.identity);
                    }                    
                    temp.transform.SetParent(chunk.transform);
                }
            }
        }

        chunk.AddComponent<NavMeshSurface>();
        chunks.Add(chunk);
    }

    private void CreateChest(int x, int y, int z, Transform chunk) { 
        int createChestChance = Random.Range(0, 100);
        if (createChestChance > 98) {
            GameObject chest =  Instantiate(chestPref, new Vector3(x, y, z),
                        Quaternion.identity);
            chest.transform.SetParent(chunk);
        }
    }

    private void CreateEnemySpawner()
    {
        int spawnerPosX = Random.Range(0, chunkSize * chunkCount);
        int spawnerPosZ = Random.Range(0, chunkSize * chunkCount);
        Instantiate(spawnerPref, new Vector3(spawnerPosX, maxBlocksCountY, spawnerPosZ), Quaternion.identity);
    }

    public void BuildNavMeshOnLevel()
    {
        foreach (GameObject chunk in chunks)
        {
            chunk.GetComponent<NavMeshSurface>().BuildNavMesh();
        }
    }
}
