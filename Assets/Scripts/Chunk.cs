using UnityEngine;

public class Chunk : MonoBehaviour
{
    private int chunkSize;
    private int maxHeight;
    private GameObject blockPrefab;

    private BlockType[,,] terrainBlocks;

    public void Initialize(int size, int height, GameObject prefab)
    {
        chunkSize = size;
        maxHeight = height;
        blockPrefab = prefab;
    }

    public void GenerateTerrain()
    {
        terrainBlocks = new BlockType[chunkSize, maxHeight, chunkSize];

        // Generate the terrain blocks
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (y == 0)
                    {
                        terrainBlocks[x, y, z] = BlockType.Stone; // Stone layer
                    }
                    else if (y < maxHeight - 4)
                    {
                        terrainBlocks[x, y, z] = BlockType.Dirt; // Dirt layer
                    }
                    else
                    {
                        terrainBlocks[x, y, z] = BlockType.Grass; // Grass layer
                    }
                }
            }
        }

        // Create the visual blocks
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < maxHeight; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    Vector3 position = new Vector3(x, y, z);
                    BlockType blockType = terrainBlocks[x, y, z];
                    CreateBlock(position, blockType);
                }
            }
        }
    }

    public void GenerateTrees()
    {
        // Generate trees in the chunk
        int maxTreeCount = chunkSize / 8; // Maximum number of trees per chunk
        int treeCount = Random.Range(0, maxTreeCount + 1);

        for (int i = 0; i < treeCount; i++)
        {
            int treeX = Random.Range(0, chunkSize);
            int treeZ = Random.Range(0, chunkSize);
            int treeHeight = Random.Range(maxHeight / 2, maxHeight - 4);

            Vector3 treePosition = new Vector3(treeX, treeHeight, treeZ);
            GenerateTree(treePosition);
        }
    }

    private void GenerateTree(Vector3 position)
    {
        // Generate a single tree at the given position
        int trunkHeight = Random.Range(4, 8);

        for (int y = 0; y < trunkHeight; y++)
        {
            Vector3 blockPosition = position + new Vector3(0, y, 0);
            CreateBlock(blockPosition, BlockType.Wood);
        }

        int leavesRadius = 2;
        int leavesHeight = trunkHeight + Random.Range(2, 5);

        for (int x = -leavesRadius; x <= leavesRadius; x++)
        {
            for (int y = trunkHeight; y <= leavesHeight; y++)
            {
                for (int z = -leavesRadius; z <= leavesRadius; z++)
                {
                    if (Mathf.Abs(x) + Mathf.Abs(z) <= leavesRadius)
                    {
                        Vector3 blockPosition = position + new Vector3(x, y, z);
                        CreateBlock(blockPosition, BlockType.Leaves);
                    }
                }
            }
        }
    }

    public void GenerateCaves()
    {
        // Generate caves in the chunk
        int maxCaveCount = chunkSize / 16; // Maximum number of caves per chunk
        int caveCount = Random.Range(0, maxCaveCount + 1);

        for (int i = 0; i < caveCount; i++)
        {
            int caveX = Random.Range(2, chunkSize - 2);
            int caveZ = Random.Range(2, chunkSize - 2);
            int caveY = Random.Range(maxHeight / 2, maxHeight - 4);

            Vector3 cavePosition = new Vector3(caveX, caveY, caveZ);
            GenerateCave(cavePosition);
        }
    }

    private void GenerateCave(Vector3 position)
    {
        // Generate a single cave at the given position
        int caveRadius = Random.Range(2, 5);
        int caveHeight = Random.Range(4, 8);

        for (int x = -caveRadius; x <= caveRadius; x++)
        {
            for (int y = -caveRadius; y <= caveRadius; y++)
            {
                for (int z = -caveRadius; z <= caveRadius; z++)
                {
                    if (Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2) <= Mathf.Pow(caveRadius, 2))
                    {
                        Vector3 blockPosition = position + new Vector3(x, y, z);
                        RemoveBlock(blockPosition);
                    }
                }
            }
        }

        // Generate iron ore deposits in the caves
        int maxIronOreDepositCount = caveRadius / 2; // Maximum number of iron ore deposits per cave
        int ironOreDepositCount = Random.Range(0, maxIronOreDepositCount + 1);

        for (int i = 0; i < ironOreDepositCount; i++)
        {
            int oreX = Random.Range(-caveRadius, caveRadius + 1);
            int oreY = Random.Range(-caveRadius, caveRadius + 1);
            int oreZ = Random.Range(-caveRadius, caveRadius + 1);
            Vector3 orePosition = position + new Vector3(oreX, oreY, oreZ);

            int oreClusterSize = Random.Range(4, 9);
            GenerateIronOreDeposit(orePosition, oreClusterSize);
        }
    }

    private void GenerateIronOreDeposit(Vector3 position, int clusterSize)
    {
        for (int i = 0; i < clusterSize; i++)
        {
            int offsetX = Random.Range(-1, 2);
            int offsetY = Random.Range(-1, 2);
            int offsetZ = Random.Range(-1, 2);

            Vector3 blockPosition = position + new Vector3(offsetX, offsetY, offsetZ);
            CreateBlock(blockPosition, BlockType.IronOre);
        }
    }

    private void CreateBlock(Vector3 position, BlockType blockType)
    {
        GameObject block = Instantiate(blockPrefab, position, Quaternion.identity, transform);
        // Set the block's material, texture, or appearance based on the block type
        // You can use different materials or textures for different block types
    }

    private void RemoveBlock(Vector3 position)
    {
        // Remove the block at the given position if it exists
        // You can implement logic to remove or hide the block visually
    }
    public enum BlockType
    {
        Stone,
        Dirt,
        Grass,
        Wood,
        Leaves,
        IronOre
    }
    
}
