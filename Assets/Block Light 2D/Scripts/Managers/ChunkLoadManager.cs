using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Handles the loading and unloading of chunks in the game.
/// </summary>
public class ChunkLoadManager : Singleton<ChunkLoadManager>
{
    public bool SmoothChunkUnload { get; set; }

    [Header("Chunks")]
    public SliderData chunkData;
    public GameObject chunkPrefab;
    public GameObject chunkRoot;
    public LayerMask chunkLayer;

    private WaitForSeconds scanCooldownTime = new WaitForSeconds(1f);
    private List<Chunk> chunksToLoad = new List<Chunk>();
    private Chunk lastChunkCreated;
    private GenerationManager generationManager;
    private LightingManager lightingManager;
    private float cameraX;
    private Vector3Int chunkPosition, worldPosition;
    private Rect loadBoundaries;
    private Collider2D hit;
    private Chunk newChunk, chunkPrevious;
    private Rect bounds;

    private LightSource ambientSeamLight;
    private LightSource placedSeamLight;
    private Vector3Int nodePosition;
    private Vector3Int newChunkPosition;
    private Color nodeColor;
    private Vector3 regionStart;
    private Vector3 regionEnd;
    private int regionStartX;
    private int regionStartY;
    private int regionEndX;
    private int regionEndY;
    private Vector3 rawPosition;


    #region Lifetime
    
    
    private IEnumerator Start()
    {
        generationManager = GenerationManager.Instance;
        lightingManager = LightingManager.Instance;

        // Sets the camera in the horizontal center of the world, at the surface
        cameraX = 0.5f * generationManager.worldWidth;
        Camera.main.transform.position = new Vector3(cameraX, generationManager.surfaceHeights[(int)cameraX],
            Camera.main.transform.position.z);

        SmoothChunkUnload = false;

        yield return null;
        StartCoroutine(LoadChunks());
        StartCoroutine(UnloadChunks());
    }
    #endregion

    /// <summary>
    /// Returns whether the given position is an air block or not.
    /// Checks the TileType on the given position in the related chunk.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public bool IsAirBlock(Vector3Int tilePosition, Chunk chunk = null)
    {
        if (chunk == null)
        {
            chunk = GetChunk(tilePosition);
            if (chunk == null)
                return true;
        }

        for (int i = 0; i < (int)Chunk.TilemapType.ENUM_END; i++)
        {
            if (chunk.GetChunkTileType(tilePosition, (Chunk.TilemapType)i) != Chunk.TileType.AIR)
                return false;
        }
        return true;
    }


    /// <summary>
    /// 检查是否有相邻瓷砖且不是空气
    /// Checks whether a given tile position has adjacent tiles that are not air blocks.
    /// Uses a simple sin and cos trigonometry calculation to quickly find the up, right, 
    /// down and left sides of the position in that order.
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public bool HasAdjacentTiles(Vector3Int tilePosition, Chunk chunk = null)
    {
        if (chunk == null)
        {
            chunk = GetChunk(tilePosition);
            if (chunk == null)
                return false;
        }

        for (float i = 0; i < Mathf.PI * 2f; i += Mathf.PI / 2f)
        {
            if (!IsAirBlock(new Vector3Int(
                tilePosition.x + Mathf.RoundToInt(Mathf.Sin(i)),
                tilePosition.y + Mathf.RoundToInt(Mathf.Cos(i)), 0)))
                return true;
        }
        return false;
    }


    /// <summary>
    /// Removes all chunks currently in game.
    /// Interrupts the current loading and unloading process until all chunks have been removed.
    /// </summary>
    public void ClearAllChunks()
    {
        StopAllCoroutines();

        foreach (Chunk chunk in GetAllChunks())
            chunk.UnloadChunk();

        StartCoroutine(LoadChunks());
        StartCoroutine(UnloadChunks());
    }


    /// <summary>
    /// Returns a list with all chunks currently in the game.
    /// </summary>
    /// <returns></returns>
    public List<Chunk> GetAllChunks()
    {
        List<Chunk> chunks = new List<Chunk>();
        foreach (Transform child in chunkRoot.transform)
        {
            Chunk chunk = child.GetComponent<Chunk>();
            if (chunk != null)
                chunks.Add(chunk);
        }
        return chunks;
    }


    /// <summary>
    /// 找出所有在边界内的Chunk
    /// Returns a list with all chunks currently in the game, inside the given Rect boundaries.
    /// Optionally allows inversion, returning all chunks outside the given Rect boundaries instead.
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="inverse"></param>
    /// <returns></returns>
    public List<Chunk> GetAllChunks(Rect rect, bool inverse = false)
    {
        List<Chunk> chunks = new List<Chunk>();
        foreach (Transform child in chunkRoot.transform)
        {
            Chunk chunk = child.GetComponent<Chunk>();
            if (chunk != null)
            {
                if (!inverse)
                {
                    if (rect.Contains(chunk.ChunkPosition))
                        chunks.Add(chunk);
                }
                else
                {
                    if (!rect.Contains(chunk.ChunkPosition))
                        chunks.Add(chunk);
                }
            }
        }
        return chunks;
    }


    /// <summary>
    /// Returns the chunk at the given position.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Chunk GetChunk(Vector3Int position)
    {
        hit = Physics2D.OverlapPoint(new Vector2(position.x + 0.5f, position.y + 0.5f), chunkLayer);
        return hit != null ? hit.GetComponent<Chunk>() : null;
    }


    /// <summary>
    /// Converts mouse position to tile position and returns it.
    /// </summary>
    /// <returns></returns>
    public Vector3Int GetTilePositionAtMouse()
    {
        rawPosition = Camera.main.ScreenToWorldPoint(
            new Vector2(Input.mousePosition.x, Input.mousePosition.y));
        return new Vector3Int((int)rawPosition.x, (int)rawPosition.y, 0);
    }


    /// <summary>
    /// 返回相机视口所在的矩形
    /// Returns a Rect that defines the area where chunks can be loaded.
    /// </summary>
    /// <returns></returns>
    private Rect GetChunkLoadBounds()
    {
        regionStart = Camera.main.transform.position + 
            Vector3.left * chunkData.GetSliderData(SliderData.SliderField.CHUNK_RADIUS_HORIZONTAL) + 
            Vector3.down * chunkData.GetSliderData(SliderData.SliderField.CHUNK_RADIUS_VERTICAL);
        regionEnd = Camera.main.transform.position +
            Vector3.right * chunkData.GetSliderData(SliderData.SliderField.CHUNK_RADIUS_HORIZONTAL) + 
            Vector3.up * chunkData.GetSliderData(SliderData.SliderField.CHUNK_RADIUS_VERTICAL);

        // Convert to int for automatic flooring of coordinates
        regionStartX = (int)regionStart.x / generationManager.chunkSize;
        regionStartY = (int)regionStart.y / generationManager.chunkSize;
        regionEndX = ((int)regionEnd.x + generationManager.chunkSize) / generationManager.chunkSize;
        regionEndY = ((int)regionEnd.y + generationManager.chunkSize) / generationManager.chunkSize;

        return new Rect(regionStartX, regionStartY, regionEndX - regionStartX, regionEndY - regionStartY);
    }


    #region IEnumerator

    

    
    /// <summary>
    /// Starts and maintains the sequence for loading chunks.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadChunks()
    {
        while (true)
        {
            yield return StartCoroutine(PerformLoadChunks());
            yield return StartCoroutine(PerformStitchChunks(chunksToLoad));
            yield return scanCooldownTime;
        }
    }


    /// <summary>
    /// Starts and maintains the sequence for unloading chunks.
    /// </summary>
    /// <returns></returns>
    private IEnumerator UnloadChunks()
    {
        while (true)
        {
            PerformUnloadChunks();
            yield return scanCooldownTime;
        }
    }


    /// <summary>
    /// Scans and loads chunks in the current chunk loading bounds.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformLoadChunks()
    {
        // Load all chunks that should be loaded depending on bounds
        loadBoundaries = GetChunkLoadBounds();
        chunksToLoad.Clear();
        lastChunkCreated = null;
        for (int h = (int)loadBoundaries.xMax; h >= (int)loadBoundaries.xMin; h--)
        {
            for (int v = (int)loadBoundaries.yMax; v >= (int)loadBoundaries.yMin; v--)
            {
                if ((h < 0 || h >= generationManager.worldWidth / generationManager.chunkSize) ||
                    (v < 0 || v >= generationManager.worldHeight / generationManager.chunkSize))
                    continue;

                chunkPosition = new Vector3Int(h, v, 0);
                worldPosition = new Vector3Int(h * generationManager.chunkSize, v * generationManager.chunkSize, 0);
                if (loadBoundaries.Contains(chunkPosition) && !GetChunk(worldPosition))
                {
                    lastChunkCreated = Instantiate(chunkPrefab, worldPosition, Quaternion.identity, chunkRoot.transform)
                        .GetComponent<Chunk>();
                    chunksToLoad.Add(lastChunkCreated);
                    StartCoroutine(lastChunkCreated.LoadChunk());
                }

                if (PerformanceManager.Instance.HitFrameLimit())
                    yield return null;
            }
        }

        // If no chunks were loaded at all, it is pointless to continue
        if (chunksToLoad.Count == 0)
            yield break;

        /* All chunks that need to be loaded can load simultaneously, but make sure that all chunks have finished loading
         * before moving past this point. */
        while (true)
        {
            bool breakOut = true;
            foreach (Chunk chunk in chunksToLoad)
            {
                if (chunk != null && chunk.IsLoading)
                {
                    breakOut = false;
                    break;
                }
            }

            if (breakOut)
                break;

            yield return null;
        }

        // Sort the chunks on distance from the camera
        chunksToLoad = chunksToLoad.OrderBy(
            x => Vector2.Distance(Camera.main.transform.position, x != null ? x.transform.position : Camera.main.transform.position)).ToList();

        // Creates light sources for each chunk
        foreach (Chunk chunk in chunksToLoad)
            yield return StartCoroutine(lightingManager.PerformCreateChunkLighting(chunk));
    }

    /// <summary>
    /// Performs lighting seam stitching for all chunks in the given list in order to correctly blend the edges with other chunks.
    /// </summary>
    /// <param name="chunks"></param>
    /// <returns></returns>
    private IEnumerator PerformStitchChunks(List<Chunk> chunks)
    {
        if (chunks == null || chunks.Count == 0)
            yield break;

        foreach (Chunk chunk in chunks)
        {
            if (chunk == null)
                continue;            
         
            newChunk = null;
            chunkPrevious = chunk;
            bounds = new Rect(0, 0, generationManager.chunkSize, generationManager.chunkSize);

            // Create 2 dummy lights for both light layers so we are able to execute light spreading
            ambientSeamLight = lightingManager.CreateLightSource(Vector3Int.zero, Color.clear, LightSourceType.AMBIENT, updateLight: false);
            placedSeamLight = lightingManager.CreateLightSource(Vector3Int.zero, Color.clear, LightSourceType.PLACED, updateLight: false);
            for (int i = -1; i < generationManager.chunkSize + 1; i++)
            {
                for (int j = -1; j < generationManager.chunkSize + 1; j++)
                {
                    // Any coordinates inside the current chunk are invalid since we just want the edge ones just outside of it
                    if (bounds.Contains(new Vector3Int(i, j, 0)))
                        continue;

                    nodePosition = new Vector3Int(chunk.Position.x + i, chunk.Position.y + j, 0);
                    if ((nodePosition.x < 0 || nodePosition.x >= generationManager.worldWidth) ||
                        (nodePosition.y < 0 || nodePosition.y >= generationManager.worldHeight))
                        continue;

                    /* Get the appropriate chunk for this position. GetChunk() may be slow when used often, like in this situation. 
                     * Convert the tile position to chunk position and see if the chunk position changed before calling it. */
                    newChunkPosition = new Vector3Int(
                        nodePosition.x / generationManager.chunkSize,
                        nodePosition.y / generationManager.chunkSize, 0);
                    if (newChunkPosition != chunkPrevious.ChunkPosition)
                    {
                        newChunk = GetChunk(nodePosition);
                        if (newChunk == null)
                            continue;
                        chunkPrevious = newChunk;
                    }

                    // Queue this tile for light spreading if it's not fully dark
                    nodeColor = chunkPrevious.GetChunkTileBlendedColor(nodePosition, dayNight: false);
                    if (nodeColor != Color.clear && nodeColor != Color.black)
                    {
                        LightNode lightNode;
                        lightNode.position = nodePosition;
                        lightNode.color = nodeColor;
                        lightNode.chunk = chunkPrevious;

                        // Select the correct layer for this tile's color by comparing color signatures
                        lightNode.type = lightingManager.IsSameColorSignature(nodeColor, lightingManager.ambientLightColor)
                            ? LightSourceType.AMBIENT : LightSourceType.PLACED;

                        // Place the tile color on the appropriate light layer
                        if (lightNode.type == LightSourceType.AMBIENT)
                            ambientSeamLight.queueLightPasses.Enqueue(lightNode);
                        else
                            placedSeamLight.queueLightPasses.Enqueue(lightNode);
                    }

                    if (PerformanceManager.Instance.HitFrameLimit())
                        yield return null;
                }
            }
            // Spread the light for all queued seam tiles
            yield return StartCoroutine(ambientSeamLight.PerformLightPassesSmooth(ambientSeamLight.queueLightPasses));
            yield return StartCoroutine(placedSeamLight.PerformLightPassesSmooth(placedSeamLight.queueLightPasses));
            lightingManager.RemoveLightSource(ambientSeamLight);
            lightingManager.RemoveLightSource(placedSeamLight);
        }
    }
    #endregion

    /// <summary>
    /// Unloads chunks that are outside the chunk loading bounds.
    /// </summary>
    /// <returns></returns>
    private void PerformUnloadChunks()
    {
        foreach (Chunk chunk in GetAllChunks(GetChunkLoadBounds(), true))
        {
            if (chunk != null && !chunk.IsUnloading)
            {
                if (SmoothChunkUnload)
                    StartCoroutine(chunk.UnloadChunkSmooth());
                else
                    chunk.UnloadChunk();
            }
        }
    }
}