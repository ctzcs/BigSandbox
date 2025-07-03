using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles the main generation control of the world.
/// </summary>
public class GenerationManager : Singleton<GenerationManager>
{
    [Header("General Components")]
    public SliderData surfaceData;
    public SliderDataCustomizer defaultBlock;

    [Header("World Settings")]
    public int seed;
    [NonSerialized]
    public int chunkSize = 64;
    public int worldWidth;
    public int worldHeight;

    [NonSerialized]
    public int[] surfaceHeights;
    public List<ScriptableObject> blockData;

    [Space]
    [Range(0f, 1f)]
    /* Start surface generation at this height relative to world height.
     * 0.5f would be 0.5f * worldHeight. */
    private readonly float surfaceHeightPosition = 0.5f;
    private int surfaceHeightAverage;
    private Vector2 perlinOffset;    
    private readonly float perlinOffsetMax = 10000f;
    private float perlinAddition;
    private Vector3Int tilePosition;

    private void Awake()
    {
        SetSeed(seed);
        Initialize();
    }


    /// <summary>
    /// Starts a new world generation process.
    /// </summary>
    public void Initialize()
    {
        surfaceHeights = new int[worldWidth];
        surfaceHeightAverage = (int)(worldHeight * surfaceHeightPosition);
        chunkSize = (int)ChunkLoadManager.Instance.chunkData.GetSliderData(SliderData.SliderField.CHUNK_SIZE);
        GenerateWorldBase();
    }


    /// <summary>
    /// Changes the seed of the game to a new seed.
    /// </summary>
    /// <param name="newSeed"></param>
    public void SetSeed(int newSeed = -1)
    {
        seed = newSeed == -1 ? (int)System.DateTime.Now.Ticks : newSeed;
        UnityEngine.Random.InitState(seed);
    }


    /// <summary>
    /// Generates surface height data based on the current seed.
    /// </summary>
    public void GenerateWorldBase()
    {
        SetSeed(seed);
        perlinOffset = new Vector2(
            UnityEngine.Random.Range(0f, perlinOffsetMax),
            UnityEngine.Random.Range(0f, perlinOffsetMax));
        perlinAddition = 0;

        int index = 0;
        while (true)
        {
            // Stop if out of bounds
            if (index < 0 || index >= worldWidth)
                return;

            // Sample
            float noiseX = perlinOffset.x + perlinAddition;
            float noiseY = perlinOffset.y + perlinAddition;

            // Sets an average height to continue on and manipulate this height with additional Perlin noise for hills
            surfaceHeightAverage += (int)((Mathf.Clamp(Mathf.PerlinNoise(noiseX, noiseY), 0f, 1f) - 0.475f) *
                surfaceData.GetSliderData(SliderData.SliderField.SURFACE_AVG_HEIGHT_MULTIPLIER));
            surfaceHeights[index] = surfaceHeightAverage + (int)(Mathf.PerlinNoise(-noiseX, -noiseY) *
                surfaceData.GetSliderData(SliderData.SliderField.SURFACE_HEIGHT_MULTIPLIER));
            perlinAddition += surfaceData.GetSliderData(SliderData.SliderField.SURFACE_PERLIN_SPEED);
            index++;
        }
    }


    /// <summary>
    /// A quick check whether the given PerlinNoise parameters exceeds the given threshold.
    /// Used to check whether a type of ore can spawn depending on the perlin height for example.
    /// NOTE: Double sampling is used to avoid straight line syndrome on certain Perlin coordinates.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="perlinSpeed"></param>
    /// <param name="perlinLevel"></param>
    /// <returns></returns>
    public bool CheckPerlinLevel(Vector3Int tilePosition, float perlinSpeed, float perlinLevel)
    {
        return (Mathf.PerlinNoise(
                    perlinOffset.x + tilePosition.x * perlinSpeed,
                    perlinOffset.y + tilePosition.y * perlinSpeed) +
                Mathf.PerlinNoise(
                    perlinOffset.x - tilePosition.x * perlinSpeed,
                    perlinOffset.y - tilePosition.y * perlinSpeed)) / 2f >= perlinLevel;
    }


    /// <summary>
    /// Uses multiple CheckPerlinLevel calls to extensively check whether a certain type of ore or
    /// block can be spawned.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="depthMin"></param>
    /// <param name="depthMax"></param>
    /// <param name="perlinSpeed"></param>
    /// <param name="perlinLevel"></param>
    /// <param name="zonePerlinSpeed"></param>
    /// <param name="zonePerlinLevel"></param>
    /// <param name="mapPerlinSpeed"></param>
    /// <param name="mapPerlinLevel"></param>
    /// <returns></returns>
    public bool CheckPerlinEligibility(Vector3Int tilePosition, float depthMin, float depthMax, float perlinSpeed, float perlinLevel,
        float zonePerlinSpeed = 0f, float zonePerlinLevel = 0f, float mapPerlinSpeed = 0f, float mapPerlinLevel = 0f)
    {
        // Depth check
        if (depthMin != -1f && depthMax != -1f)
        {
            int depth = surfaceHeights[tilePosition.x] - tilePosition.y;
            if (!(depth >= depthMin && depth < depthMax))
                return false;
        }

        // Perlin sampling
        if ((mapPerlinSpeed == 0f && mapPerlinLevel == 0f) ||
            CheckPerlinLevel(tilePosition, mapPerlinSpeed, mapPerlinLevel))
        {
            if ((zonePerlinSpeed == 0f && zonePerlinLevel == 0f) ||
                CheckPerlinLevel(tilePosition, zonePerlinSpeed, zonePerlinLevel))
            {
                if (perlinSpeed == 0f && perlinLevel == 0f)
                    return false;

                return (CheckPerlinLevel(tilePosition, perlinSpeed, perlinLevel));
            }
        }
        return false;
    }


    /// <summary>
    /// Create a brand new chunk using Perlin data combined with the given seed.
    /// </summary>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public IEnumerator GenerateChunk(Chunk chunk)
    {
        for (int v = 0; v < chunkSize; v++)
        {
            for (int h = 0; h < chunkSize; h++)
            {
                tilePosition = new Vector3Int(chunk.Position.x + h, chunk.Position.y + v, 0);
                if ((tilePosition.x < 0 || tilePosition.x >= worldWidth) ||
                    (tilePosition.y < 0 || tilePosition.y >= worldHeight))
                    continue;

                if (tilePosition.y <= surfaceHeights[tilePosition.x])
                {
                    // Start with the default block
                    SliderDataCustomizer blockData = defaultBlock;

                    // Loop through the blocks and overwrite the default block if that block can be spawned
                    for (int i = 0; i < this.blockData.Count; i++)
                    {
                        SliderDataCustomizer block = this.blockData[i] as SliderDataCustomizer;
                        if (block != defaultBlock)
                        {
                            if (CheckPerlinEligibility(tilePosition,
                                block.GetSliderData(SliderData.SliderField.DEPTH_MIN),
                                block.GetSliderData(SliderData.SliderField.DEPTH_MAX),
                                block.GetSliderData(SliderData.SliderField.PERLIN_SPEED),
                                block.GetSliderData(SliderData.SliderField.PERLIN_LEVEL),
                                block.GetSliderData(SliderData.SliderField.ZONE_PERLIN_SPEED),
                                block.GetSliderData(SliderData.SliderField.ZONE_PERLIN_LEVEL),
                                block.GetSliderData(SliderData.SliderField.MAP_PERLIN_SPEED),
                                block.GetSliderData(SliderData.SliderField.MAP_PERLIN_LEVEL)))
                            {
                                blockData = block;
                                break;
                            }
                        }
                    }

                    // Set the desired tile
                    chunk.SetChunkTile(tilePosition, Chunk.TilemapType.BLOCKS_FRONT, blockData.itemTile);
                    chunk.SetChunkTile(tilePosition, Chunk.TilemapType.BLOCKS_BACK,
                        blockData.itemType == Chunk.TileType.AIR ? defaultBlock.itemTile : blockData.itemTile);
                    chunk.SetChunkTileType(tilePosition, Chunk.TilemapType.BLOCKS_FRONT, blockData.itemType);
                    chunk.SetChunkTileType(tilePosition, Chunk.TilemapType.BLOCKS_BACK,
                        blockData.itemType == Chunk.TileType.AIR ? defaultBlock.itemType : blockData.itemType);

                    chunk.SetChunkTileColor(tilePosition, Color.black, false, LightSourceType.AMBIENT);

                    if (PerformanceManager.Instance.HitFrameLimit())
                        yield return null;
                }
            }
        }
    }
}