using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// Describes a chunk of terrain in the world.
/// 
/// Added additional functions to provide quick integration into
/// world systems, such as getting and setting tiles, the block
/// type, changing tile colors, setting tile rotation etcetera.
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class Chunk : MonoBehaviour
{
    public bool IsLoading { get; private set; }
    public bool IsUnloading { get; private set; }
    public bool IsCompletelyDark { get; set; }
    public Vector3Int Position { get; private set; }
    public Vector3Int ChunkPosition { get; private set; }
    public List<LightSource> AmbientLightSources { get; set; }
    public List<LightSource> PlacedLightSources { get; set; }
    public Tilemap tileMapBlocksFront;
    public Tilemap tileMapBlocksBack;
    public Color[,] tileColorsAmbient, tileColorsPlaced;
    public TileType[,] tileTypeBlocksFront, tileTypeBlocksBack;
    public enum TileType
    {
        AIR,
        COAL,
        COPPER,
        DIRT,
        DIRT_GRASS,
        IRON,
        STONE
    }
    public enum TilemapType
    {
        BLOCKS_FRONT,
        BLOCKS_BACK,
        ENUM_END
    }

    private BoxCollider2D chunkCollider;
    private List<Color[,]> allColorArrays = new List<Color[,]>();
    private GenerationManager generationManager;
    private LightingManager lightingManager;
    private Vector3Int relativePosition;
    private Quaternion matrixRotation;
    private Matrix4x4 matrix;
    private Tilemap targetTilemap;
    private Color[,] targetColorMap;
    private TileType[,] targetTypeMap;
    private Color arrayColor, resultColor, totalBlendedColor;
    private float rOperation, gOperation, bOperation, backLayerShadowFactor;
    private int relativePositionX, relativePositionY;

    #region Lifetime
    private void Awake()
    {
        generationManager = GenerationManager.Instance;
        lightingManager = LightingManager.Instance;

        Position = new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
        ChunkPosition = new Vector3Int(
            Position.x / generationManager.chunkSize,
            Position.y / generationManager.chunkSize, 0);

        AmbientLightSources = new List<LightSource>();
        PlacedLightSources = new List<LightSource>();
        IsCompletelyDark = true;

        // Stores block types for each tile in the chunk
        tileTypeBlocksFront = new TileType[
            generationManager.chunkSize,
            generationManager.chunkSize];
        tileTypeBlocksBack = new TileType[
            generationManager.chunkSize,
            generationManager.chunkSize];

        // Stores color data for each tile in the chunk
        tileColorsAmbient = new Color[
            generationManager.chunkSize,
            generationManager.chunkSize];
        tileColorsPlaced = new Color[
            generationManager.chunkSize,
            generationManager.chunkSize];
        allColorArrays.Add(tileColorsAmbient);
        allColorArrays.Add(tileColorsPlaced);

        // Set the offset and size of the chunk's box collider based on the chunkSize.
        chunkCollider = GetComponent<BoxCollider2D>();
        chunkCollider.size = new Vector2(
            generationManager.chunkSize,
            generationManager.chunkSize);
        chunkCollider.offset = new Vector2(
            generationManager.chunkSize / 2f,
            generationManager.chunkSize / 2f);        
    }

    #endregion

    /// <summary>
    /// Generates this chunk based on the seed.
    /// </summary>
    public IEnumerator LoadChunk()
    {
        IsLoading = true;
        yield return StartCoroutine(generationManager.GenerateChunk(this));
        IsLoading = false;
    }


    /// <summary>
    /// Deletes this chunk from the world.
    /// 
    /// NOTE: Removes light sources without clearing their light. This may cause artifacts to remain on surrounding chunks, especially
    /// for placed lights. This is fixed however if chunks were to be saved to disk, together with their light data, and loaded instead
    /// of regenerated based on the seed as of now. If you absolutely need a fix for this, use smooth chunk unloading.
    /// </summary>
    public void UnloadChunk()
    {
        /* Destroying the chunk doesn't happen instantly. DestroyImmediate works, but it is
         * safer to just track a bool until it is loaded out of memory. */
        IsUnloading = true;

        // Remove all chunk light objects
        foreach (LightSource light in AmbientLightSources)
            if (light != null)
                lightingManager.RemoveLightSource(light, removeLight: false);
        foreach (LightSource light in PlacedLightSources)
            if (light != null)
                lightingManager.RemoveLightSource(light, removeLight: false);

        Destroy(gameObject);
    }

    /// <summary>
    /// Deletes this chunk from the world smoothly. Removes the light of all its light sources smoothly first.
    /// </summary>
    /// <returns></returns>
    public IEnumerator UnloadChunkSmooth()
    {
        /* Destroying the chunk doesn't happen instantly. DestroyImmediate works, but it is
         * safer to just track a bool until it is loaded out of memory. */
        IsUnloading = true;

        // Remove all chunk light objects smoothly
        foreach (LightSource light in AmbientLightSources)
            if (light != null)
                StartCoroutine(lightingManager.RemoveLightSourceSmooth(light, removeLight: true));
        foreach (LightSource light in PlacedLightSources)
            if (light != null)
                StartCoroutine(lightingManager.RemoveLightSourceSmooth(light, removeLight: true));

        // Make sure all lights have finished removing themselves before continuing
        while (true)
        {
            bool breakOut = true;
            foreach (LightSource light in AmbientLightSources)
            {
                if (light != null && light.state != LightSource.LightSourceState.IDLE)
                {
                    breakOut = false;
                    break;
                }
            }
            // If all ambient light sources turned out to be idle or null at this point, check the placed lights if any
            if (breakOut)
            {
                foreach (LightSource light in PlacedLightSources)
                {
                    if (light != null && light.state != LightSource.LightSourceState.IDLE)
                    {
                        breakOut = false;
                        break;
                    }
                }
            }

            // Stop if all lights in this chunk are idle (or null from being destroyed)
            if (breakOut)
                break;

            yield return null;
        }

        // We removed all chunk light smoothly. Now delete this chunk
        Destroy(gameObject);
    }


    /// <summary>
    /// Changes the color of every tile in this chunk to white, making them fully visible.
    /// Only changes visuals on the Tilemaps. The color map still retains and updates correct lighting in the background.
    /// </summary>
    public void DisableLighting()
    {
        for (int h = 0; h < generationManager.chunkSize; h++)
            for (int v = 0; v < generationManager.chunkSize; v++)
                SetChunkTileColor(new Vector3Int(Position.x + h, Position.y + v, 0), Color.white, true, LightSourceType.ALL);    
    }


    /// <summary>
    /// Changes the color of every tile in this chunk to its color map equivalent, effectively restoring their lighting.
    /// Only changes visuals on the Tilemaps. The color map is used to restore the Tile colors and is not affected at all.   
    /// </summary>
    public void EnableLighting()
    {
        for (int h = 0; h < generationManager.chunkSize; h++)
        {
            for (int v = 0; v < generationManager.chunkSize; v++)
            {
                SetChunkTileColor(new Vector3Int(Position.x + h, Position.y + v, 0), tileColorsAmbient[h, v], true, LightSourceType.AMBIENT);
                SetChunkTileColor(new Vector3Int(Position.x + h, Position.y + v, 0), tileColorsPlaced[h, v], true, LightSourceType.PLACED);
            }
        }
    }


    /// <summary>
    /// Returns the right Tilemap for the given TilemapType constant for this chunk.
    /// </summary>
    /// <param name="mapType"></param>
    /// <param name="forceRequest"></param>
    /// <returns></returns>
    private Tilemap GetMap(TilemapType mapType, bool forceRequest = false)
    {
        if (!forceRequest && IsUnloading)
            return null;

        switch (mapType)
        {
            case TilemapType.BLOCKS_FRONT:
                return tileMapBlocksFront;
            case TilemapType.BLOCKS_BACK:
                return tileMapBlocksBack;
            default:
                return null;
        }
    }


    /// <summary>
    /// Returns the right TileType array for the given TilemapType constant for this chunk.
    /// These TileType arrays hold the block types for all chunk blocks.
    /// </summary>
    /// <param name="mapType"></param>
    /// <param name="forceRequest"></param>
    /// <returns></returns>
    private TileType[,] GetTypeMap(TilemapType mapType, bool forceRequest = false)
    {
        if (!forceRequest && IsUnloading)
            return null;

        switch (mapType)
        {
            case TilemapType.BLOCKS_FRONT:
                return tileTypeBlocksFront;
            case TilemapType.BLOCKS_BACK:
                return tileTypeBlocksBack;
            default:
                return null;
        }
    }


    /// <summary>
    /// Returns the right color map layer for the given LightSourceType constant for this chunk.
    /// The color maps store the color per tile in the chunk.
    /// </summary>
    /// <param name="mapType"></param>
    /// <param name="forceRequest"></param>
    /// <returns></returns>
    private Color[,] GetColorMap(LightSourceType mapType, bool forceRequest = false)
    {
        if (!forceRequest && IsUnloading)
            return null;

        switch (mapType)
        {
            case LightSourceType.AMBIENT:
                return tileColorsAmbient;
            case LightSourceType.PLACED:
                return tileColorsPlaced;
            default:
                return null;
        }
    }


    /// <summary>
    /// Returns the tile stored in the given Tilemap, on the given position.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="mapType"></param>
    /// <param name="forceRequest"></param>
    /// <returns></returns>
    public TileBase GetChunkTile(Vector3Int position, TilemapType mapType, bool forceRequest = false)
    {
        if (!forceRequest && IsUnloading)
            return null;

        targetTilemap = GetMap(mapType, forceRequest);
        if (targetTilemap == null)
            return null;

        relativePosition = new Vector3Int(position.x - Position.x, position.y - Position.y, 0);
        if (relativePosition.x < 0 || relativePosition.x >= generationManager.chunkSize ||
            relativePosition.y < 0 || relativePosition.y >= generationManager.chunkSize)
            return null;

        return targetTilemap.GetTile(relativePosition);
    }


    /// <summary>
    /// Fills the given position in the given Tilemap with the given Tile.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="mapType"></param>
    /// <param name="tile"></param>
    /// <param name="forceRequest"></param>
    public void SetChunkTile(Vector3Int position, TilemapType mapType, Tile tile, bool forceRequest = false)
    {
        if (!forceRequest && IsUnloading)
            return;

        targetTilemap = GetMap(mapType, forceRequest);
        if (targetTilemap == null)
            return;

        relativePosition = new Vector3Int(position.x - Position.x, position.y - Position.y, 0);
        if (relativePosition.x < 0 || relativePosition.x >= generationManager.chunkSize ||
            relativePosition.y < 0 || relativePosition.y >= generationManager.chunkSize)
            return;
        //设置相关的位置为某种tile
        targetTilemap.SetTile(relativePosition, tile);
    }


    /// <summary>
    /// Returns the color of the tile at the given position on the lightmap layer belonging to the given LightSourceType.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="mapType"></param>
    /// <param name="forceRequest"></param>
    /// <returns></returns>
    public Color GetChunkTileColor(Vector3Int position, LightSourceType mapType = LightSourceType.AMBIENT, bool forceRequest = false)
    {
        if (mapType == LightSourceType.ALL || (!forceRequest && IsUnloading))
            return Color.clear;

        targetColorMap = GetColorMap(mapType, forceRequest);
        if (targetColorMap == null)
            return Color.clear;

        relativePositionX = position.x - Position.x;
        relativePositionY = position.y - Position.y;
        if (relativePositionX < 0 || relativePositionX >= generationManager.chunkSize ||
            relativePositionY < 0 || relativePositionY >= generationManager.chunkSize)
            return Color.clear;

        return targetColorMap[relativePositionX, relativePositionY];
    }


    /// <summary>
    /// Returns the blended color of the tile at the given position which is the color of all lightmaps blended together (max).
    /// dayNight can be false to ignore the game's night level, returning the blended color at full brightness.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="dayNight"></param>
    /// <param name="forceRequest"></param>
    /// <returns></returns>
    public Color GetChunkTileBlendedColor(Vector3Int position, bool dayNight = true, bool forceRequest = false)
    {
        if (!forceRequest && IsUnloading)
            return Color.clear;

        relativePositionX = position.x - Position.x;
        relativePositionY = position.y - Position.y;
        if (relativePositionX < 0 || relativePositionX >= generationManager.chunkSize ||
            relativePositionY < 0 || relativePositionY >= generationManager.chunkSize)
            return Color.clear;

        resultColor = Color.black;
        foreach (Color[,] colorArray in allColorArrays)
        {
            arrayColor = colorArray[relativePositionX, relativePositionY];

            // Subtract night level if this is the ambient lighting layer
            if (colorArray == tileColorsAmbient && dayNight)
            {
                rOperation = arrayColor.r - lightingManager.NightLevel;
                gOperation = arrayColor.g - lightingManager.NightLevel;
                bOperation = arrayColor.b - lightingManager.NightLevel;
                arrayColor = new Color(
                    rOperation < 0f ? 0f : (rOperation > 1f ? 1f : rOperation),
                    gOperation < 0f ? 0f : (gOperation > 1f ? 1f : gOperation),
                    bOperation < 0f ? 0f : (bOperation > 1f ? 1f : bOperation));
            }

            resultColor = new Color(
                resultColor.r > arrayColor.r ? resultColor.r : arrayColor.r,
                resultColor.g > arrayColor.g ? resultColor.g : arrayColor.g,
                resultColor.b > arrayColor.b ? resultColor.b : arrayColor.b);
        }
        return resultColor;
    }


    /// <summary>
    /// Sets the color of the tile in the given Tilemap at the given position.
    /// 在给与的Tilemap中设置给定位置的Tile的颜色。
    /// NOTE: Tiles by default have TileFlags that prevent color changes, rotation
    /// lock etc. Make sure you unlock these tiles by changing its TileFlags in code or go 
    /// into the inspector and visit your tiles, turn on debug mode and change it there,
    /// before using this function. I've already done this for any tiles used here, but it's
    /// good to remember when you want to add more.  
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    /// <param name="visualsOnly"></param>
    /// <param name="mapType"></param>
    /// <param name="forceRequest"></param>
    public void SetChunkTileColor(Vector3Int position, Color color, bool visualsOnly = false, LightSourceType mapType = LightSourceType.AMBIENT, bool forceRequest = false)
    {
        if (!forceRequest && IsUnloading)
            return;

        relativePositionX = position.x - Position.x;
        relativePositionY = position.y - Position.y;
        if (relativePositionX < 0 || relativePositionX >= generationManager.chunkSize ||
            relativePositionY < 0 || relativePositionY >= generationManager.chunkSize)
            return;

        relativePosition = new Vector3Int(relativePositionX, relativePositionY, 0);
        if (!visualsOnly)
        {
            if (mapType != LightSourceType.ALL)
            {
                targetColorMap = GetColorMap(mapType, forceRequest);
                if (targetColorMap == null)
                    return;

                targetColorMap[relativePositionX, relativePositionY] = color;
            }
            else
            {
                foreach (Color[,] colorArray in allColorArrays)
                    colorArray[relativePositionX, relativePositionY] = color;
            }
        }

        // Visuals
        totalBlendedColor = !lightingManager.LightingIsActive ? Color.white : (mapType == LightSourceType.ALL ? color : GetChunkTileBlendedColor(position));
        backLayerShadowFactor = lightingManager.BackLayerShadowFactor;
        tileMapBlocksFront.SetColor(relativePosition, totalBlendedColor);
        tileMapBlocksBack.SetColor(relativePosition, new Color(
            totalBlendedColor.r * backLayerShadowFactor,
            totalBlendedColor.g * backLayerShadowFactor,
            totalBlendedColor.b * backLayerShadowFactor));
    }


    /// <summary>
    /// Returns the tile type of a block in this chunk, in the given Tilemap.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="mapType"></param>
    /// <param name="forceRequest"></param>
    /// <returns></returns>
    public TileType GetChunkTileType(Vector3Int position, TilemapType mapType, bool forceRequest = false)
    {
        if (!forceRequest && IsUnloading)
            return TileType.AIR;

        targetTypeMap = GetTypeMap(mapType, forceRequest);
        if (targetTypeMap == null)
            return TileType.AIR;

        relativePositionX = position.x - Position.x;
        relativePositionY = position.y - Position.y;
        if (relativePositionX < 0 || relativePositionX >= generationManager.chunkSize ||
            relativePositionY < 0 || relativePositionY >= generationManager.chunkSize)
            return TileType.AIR;

        return targetTypeMap[relativePositionX, relativePositionY];
    }


    /// <summary>
    /// Sets the tile type of a block in this chunk, in the given Tilemap.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="mapType"></param>
    /// <param name="type"></param>
    /// <param name="forceRequest"></param>
    public void SetChunkTileType(Vector3Int position, TilemapType mapType, TileType type, bool forceRequest = false)
    {
        if (!forceRequest && IsUnloading)
            return;

        targetTypeMap = GetTypeMap(mapType, forceRequest);
        if (targetTypeMap == null)
            return;

        relativePositionX = position.x - Position.x;
        relativePositionY = position.y - Position.y;
        if (relativePositionX < 0 || relativePositionX >= generationManager.chunkSize ||
            relativePositionY < 0 || relativePositionY >= generationManager.chunkSize)
            return;

        targetTypeMap[relativePositionX, relativePositionY] = type;
    }


    /// <summary>
    /// Sets the rotation of a given tile at the given position in the given Tilemap.
    /// Useful for rotating tiles in 90 degree increments for seemingly different textures for example.
    /// 
    /// NOTE: Tiles by default have TileFlags that prevent color changes, rotation
    /// lock etc. Make sure you unlock these tiles by changing its TileFlags or go 
    /// into the inspector and visit your tiles, turn on debug mode and change it there,
    /// before using this function. I've already done this for any tiles used here, but it's
    /// good to remember when you want to add more.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="mapType"></param>
    /// <param name="rotation"></param>
    /// <param name="forceRequest"></param>
    public void SetChunkTileRotation(Vector3Int position, TilemapType mapType, Vector3 rotation, bool forceRequest = false)
    {
        if (!forceRequest && IsUnloading)
            return;

        targetTilemap = GetMap(mapType, forceRequest);
        if (targetTilemap == null)
            return;

        relativePosition = new Vector3Int(position.x - Position.x, position.y - Position.y, 0);
        if (relativePosition.x < 0 || relativePosition.x >= generationManager.chunkSize ||
            relativePosition.y < 0 || relativePosition.y >= generationManager.chunkSize)
            return;

        matrixRotation = Quaternion.Euler(rotation);
        matrix = Matrix4x4.Rotate(matrixRotation);
        targetTilemap.SetTransformMatrix(relativePosition, matrix);
    }
}