using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject grassBlock;
    public GameObject dirtBlock;
    public GameObject stoneBlock;
    public GameObject woodBlock;
    public GameObject leafBlock;
    public GameObject ironOreBlock;
    public GameObject airBlock;
    public GameObject player;
    public int chunkSize = 32;
    public int maxHeight = 32;
    public int viewDistance = 2;
    public int seed = 0;
    public float treeChance = 0.1f;
    public float caveChance = 0.05f;
    public float hillChance = 0.2f;
    public float ironOreChance = 0.05f;
    public int ironOreClusterMinSize = 4;
    public int ironOreClusterMaxSize = 8;

    private Dictionary<Vector2Int, GameObject> chunks = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        Random.InitState(seed);
        GenerateChunk(new Vector2Int(0, 0));
    }

    void Update()
    {
        Vector2Int playerChunkPos = GetPlayerChunkPos();
        for (int x = playerChunkPos.x - viewDistance; x <= playerChunkPos.x + viewDistance; x++)
        {
            for (int z = playerChunkPos.y - viewDistance; z <= playerChunkPos.y + viewDistance; z++)
            {
                Vector2Int chunkPos = new Vector2Int(x, z);
                if (!chunks.ContainsKey(chunkPos))
                {
                    GenerateChunk(chunkPos);
                }
            }
        }
    }

    void GenerateChunk(Vector2Int chunkPos)
    {
        GameObject chunkObject = new GameObject("Chunk (" + chunkPos.x + ", " + chunkPos.y + ")");
        chunkObject.transform.parent = transform;
        chunks.Add(chunkPos, chunkObject);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int z = 0; z < chunkSize; z++)
            {
                int height = Mathf.RoundToInt(maxHeight * Mathf.PerlinNoise((chunkPos.x * chunkSize + x) / 16.0f,
                    (chunkPos.y * chunkSize + z) / 16.0f));

                for (int y = 0; y < height; y++)
                {
                    GameObject block;
                    if (y == 0)
                    {
                        block = Instantiate(stoneBlock,
                            new Vector3(chunkPos.x * chunkSize + x, y, chunkPos.y * chunkSize + z), Quaternion.identity,
                            chunkObject.transform);
                    }
                    else if (y < height - 4)
                    {
                        float rand = Random.Range(0.0f, 1.0f);
                        if (rand < caveChance)
                        {
                            GenerateCave(chunkPos.x * chunkSize + x, y, chunkPos.y * chunkSize + z);
                            continue;
                        }
                        else
                        {
                            block = Instantiate(stoneBlock,
                                new Vector3(chunkPos.x * chunkSize + x, y, chunkPos.y * chunkSize + z),
                                Quaternion.identity, chunkObject.transform);
                        }
                    }
                    else if (y < height - 1)
                    {
                        float rand = Random.Range(0.0f, 1.0f);
                        if (rand < hillChance)
                        {
                            block = Instantiate(dirtBlock, new Vector3(chunkPos.x * chunkSize + x, y, chunkPos.y * chunkSize + z),
                                Quaternion.identity, chunkObject.transform);
                        }
                        else
                        {
                            block = Instantiate(grassBlock,
                                new Vector3(chunkPos.x * chunkSize + x, y, chunkPos.y * chunkSize + z),
                                Quaternion.identity, chunkObject.transform);
                            if (Random.Range(0.0f, 1.0f) < treeChance)
                            {
                                GenerateTree(chunkPos.x * chunkSize + x, y + 1, chunkPos.y * chunkSize + z);
                            }
                        }

                        if (x == 0 || x == chunkSize - 1 || z == 0 || z == chunkSize - 1 || y == 0 || y == height - 1)
                        {
                            block.GetComponent<Block>().SetVisible(true);
                        }

                        // Generate iron ore clusters within caves
                        if (block.CompareTag("Stone") && y < height - 4)
                        {
                            float rand2 = Random.Range(0.0f, 1.0f);
                            if (rand2 < ironOreChance)
                            {
                                int clusterSize = Random.Range(ironOreClusterMinSize, ironOreClusterMaxSize + 1);
                                GenerateIronOreCluster(chunkPos.x * chunkSize + x, y, chunkPos.y * chunkSize + z,
                                    clusterSize);
                            }
                        }
                    }
                }
            }
        }

        void GenerateTree(int x, int y, int z)
        {
            GameObject wood = Instantiate(woodBlock, new Vector3(x, y, z), Quaternion.identity, chunks[new Vector2Int(x / chunkSize, z / chunkSize)].transform);
            for (int i = 1; i <= 2; i++)
            {
                Instantiate(woodBlock, new Vector3(x, y + i, z), Quaternion.identity, chunks[new Vector2Int(x / chunkSize, z / chunkSize)].transform);
            }
            for (int i = -1; i <= 1; i++)
            {
                for (int j = 2; j <= 3; j++)
                {
                    Instantiate(leafBlock, new Vector3(x + i, y + j, z), Quaternion.identity, chunks[new Vector2Int(x / chunkSize, z / chunkSize)].transform);
                }
            }
            for (int i = -1; i <= 1; i++)
            {
                for (int j = 2; j <= 3; j++)
                {
                    Instantiate(leafBlock, new Vector3(x, y + j, z + i), Quaternion.identity, chunks[new Vector2Int(x / chunkSize, z / chunkSize)].transform);
                }
            }
        }

        void GenerateIronOreCluster(int startX, int startY, int startZ, int size)
        {
            for (int i = 0; i < size; i++)
            {
                int offsetX = Random.Range(-1, 2);
                int offsetY = Random.Range(-1, 2);
                int offsetZ = Random.Range(-1, 2);

                int x = startX + offsetX;
                int y = startY + offsetY;
                int z = startZ + offsetZ;

                GameObject ironOre = Instantiate(ironOreBlock, new Vector3(x, y, z), Quaternion.identity,
                    chunks[new Vector2Int(x / chunkSize, z / chunkSize)].transform);
            }
        }
        
        void GenerateCave(int x, int y, int z)
        {
            int caveSize = Random.Range(5, 20); // Random cave size (1 to 3)
            float caveRadius = Random.Range(2.0f, 5.0f); // Random cave radius

            for (int xOffset = -caveSize; xOffset <= caveSize; xOffset++)
            {
                for (int yOffset = -caveSize; yOffset <= caveSize; yOffset++)
                {
                    for (int zOffset = -caveSize; zOffset <= caveSize; zOffset++)
                    {
                        float distance = Mathf.Sqrt(xOffset * xOffset + yOffset * yOffset + zOffset * zOffset);
                        if (distance <= caveSize)
                        {
                            int caveX = x + xOffset;
                            int caveY = y + yOffset;
                            int caveZ = z + zOffset;

                            DestroyBlock(caveX, caveY, caveZ); // Destroy the block within the cave
                        }
                    }
                }
            }
        }
        
        void DestroyBlock(int x, int y, int z)
        {
            Vector2Int chunkPos = new Vector2Int(Mathf.FloorToInt(x / chunkSize), Mathf.FloorToInt(z / chunkSize));
            Vector3 blockPos = new Vector3(x, y, z);

            if (chunks.ContainsKey(chunkPos))
            {
                Transform chunkTransform = chunks[chunkPos].transform;
                Block block = chunkTransform.GetComponentInChildren<Block>();

                if (block != null)
                {
                    // Destroy the block game object
                    Destroy(block.gameObject);
                }
            }
        }
        
    }
    Vector2Int GetPlayerChunkPos()
    {
        return new Vector2Int(Mathf.FloorToInt(player.transform.position.x / chunkSize), Mathf.FloorToInt(player.transform.position.z / chunkSize));
    }
}
