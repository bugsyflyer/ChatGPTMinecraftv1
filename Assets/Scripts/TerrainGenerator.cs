using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject player;

    private int chunkSize = 32;
    private int maxHeight = 32;
    private int renderDistance = 1;
    private int chunkLoadDistance = 2;

    private Chunk[,] chunks;
    private int playerChunkX;
    private int playerChunkZ;

    private void Start()
    {
        chunks = new Chunk[chunkLoadDistance * 2 + 1, chunkLoadDistance * 2 + 1];
        GenerateTerrainAroundPlayer();
    }

    private void Update()
    {
        int newPlayerChunkX = Mathf.FloorToInt(player.transform.position.x / chunkSize);
        int newPlayerChunkZ = Mathf.FloorToInt(player.transform.position.z / chunkSize);

        if (newPlayerChunkX != playerChunkX || newPlayerChunkZ != playerChunkZ)
        {
            playerChunkX = newPlayerChunkX;
            playerChunkZ = newPlayerChunkZ;
            GenerateTerrainAroundPlayer();
        }
    }

    private void GenerateTerrainAroundPlayer()
    {
        for (int x = playerChunkX - chunkLoadDistance; x <= playerChunkX + chunkLoadDistance; x++)
        {
            for (int z = playerChunkZ - chunkLoadDistance; z <= playerChunkZ + chunkLoadDistance; z++)
            {
                if (chunks[x, z] == null)
                {
                    GenerateChunk(x, z);
                }
            }
        }
    }

    private void GenerateChunk(int chunkX, int chunkZ)
    {
        GameObject chunkGameObject = new GameObject("Chunk " + chunkX + ", " + chunkZ);
        chunkGameObject.transform.position = new Vector3(chunkX * chunkSize, 0, chunkZ * chunkSize);

        Chunk chunk = chunkGameObject.AddComponent<Chunk>();
        chunk.Initialize(chunkSize, maxHeight, blockPrefab);
        chunk.GenerateTerrain();
        chunk.GenerateTrees();
        chunk.GenerateCaves();

        chunks[chunkX, chunkZ] = chunk;
    }
}