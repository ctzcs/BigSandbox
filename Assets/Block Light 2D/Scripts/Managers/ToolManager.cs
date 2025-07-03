using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles ghosting and frame execution of the selected lighting tool.
/// Part of the lighting UI and should only be active if the lighting UI is active.
/// </summary>
public class ToolManager : Singleton<ToolManager>
{
    public bool IsGhosting { get; private set; }    
    public Color ghostColor, ghostColorBlocked;
    public SpriteRenderer ghost;
    public Sprite lightSpriteAdd, lightSpriteRemove;

    private bool toolIsActive = false;
    private Vector3Int mousePosition, ghostPosition, ghostRelativePosition;
    private Chunk chunk, chunkGhost;
    private LightingManager lightingManager;
    private ChunkLoadManager chunkLoadManager;
    private UserInterfaceLighting userInterfaceLighting;
    private GenerationManager generationManager;
    private SliderDataCustomizer itemData;
    private Vector3 normalizedColor;

    private void Start()
    {
        lightingManager = LightingManager.Instance;
        chunkLoadManager = ChunkLoadManager.Instance;
        userInterfaceLighting = UserInterfaceLighting.Instance;
        generationManager = GenerationManager.Instance;
    }


    /// <summary>
    /// Enable ghost dragging for the given item.
    /// </summary>
    /// <param name="data"></param>
    public void GhostTile(SliderDataCustomizer data)
    {
        if (data == null)
            return;

        Unghost();
        itemData = data;
        ghost.sprite = itemData.itemTile.sprite;
        ghost.color = ghostColor;
        IsGhosting = true;
    }


    /// <summary>
    /// Enable ghost dragging for the given item.
    /// </summary>
    /// <param name="sprite"></param>
    public void GhostTile(Sprite sprite)
    {
        if (sprite == null)
            return;

        Unghost();
        itemData = null;
        ghost.sprite = sprite;
        ghost.color = Color.white;
        IsGhosting = true;
    }


    /// <summary>
    /// Stops ghosting.
    /// </summary>
    public void Unghost()
    {
        ghost.sprite = null;
        IsGhosting = false;
    }


    /// <summary>
    /// Determines whether a ghost can be placed or not, depending on its position
    /// within world bounds or whether the location is blocked with other objects or
    /// tiles etcetera.
    /// </summary>
    /// <param name="shiftIsPressed"></param>
    /// <param name="checkForObjects"></param>
    /// <returns></returns>
    private bool GhostCanBePlaced(bool shiftIsPressed = false, bool checkForObjects = true)
    {
        if (ghost.sprite == null)
            return false;

        ghostPosition = new Vector3Int(
            (int)ghost.transform.position.x, 
            (int)ghost.transform.position.y, 0);

        // Loop through sprite bounds positions
        for (int v = 0; v < ghost.sprite.bounds.max.y; v++)
        {
            for (int h = 0; h < ghost.sprite.bounds.max.x; h++)
            {
                ghostRelativePosition = new Vector3Int(ghostPosition.x + h, ghostPosition.y + v, 0);
                if ((ghostRelativePosition.x < 1 || ghostRelativePosition.x >= generationManager.worldWidth - 1) ||
                    (ghostRelativePosition.y < 1 || ghostRelativePosition.y >= generationManager.worldHeight - 1))
                    return false;

                chunkGhost = chunkLoadManager.GetChunk(ghostRelativePosition);
                if (chunkGhost != null && chunkGhost.GetChunkTileType(ghostRelativePosition, Chunk.TilemapType.BLOCKS_BACK) != Chunk.TileType.AIR)
                {
                    // Shift targets the back layer and there's something there already
                    if (shiftIsPressed)
                        return false;
                    else
                    {
                        if (checkForObjects)
                        {
                            if (lightingManager.GetLightSource(ghostRelativePosition) != null)
                                return false;
                        }

                        if (chunkGhost.GetChunkTileType(ghostRelativePosition, Chunk.TilemapType.BLOCKS_FRONT) != Chunk.TileType.AIR)
                            return false;
                    }
                }
            }
        }
        return true;
    }


    /// <summary>
    /// Places a tile at the current mouse position.
    /// </summary>
    /// <param name="mapType"></param>
    private void PlaceTile(Chunk.TilemapType mapType = Chunk.TilemapType.BLOCKS_FRONT)
    {
        Vector3Int tilePosition = new Vector3Int((int)ghost.transform.position.x, (int)ghost.transform.position.y, 0);
        if ((tilePosition.x < 0 || tilePosition.x >= generationManager.worldWidth) ||
            (tilePosition.y < 0 || tilePosition.y >= generationManager.worldHeight))
            return;
        chunk = chunkLoadManager.GetChunk(tilePosition);
        if (chunk == null || chunk.GetChunkTileType(tilePosition, mapType) != Chunk.TileType.AIR)
            return;

        Color currentColor = chunk.GetChunkTileBlendedColor(tilePosition, false);

        // Place block        
        chunk.SetChunkTile(tilePosition, mapType, itemData.itemTile);        
        chunk.SetChunkTileType(tilePosition, mapType, itemData.itemType);
        chunk.SetChunkTileColor(tilePosition, Color.black, false, LightSourceType.AMBIENT);

        // Mark the air blocks around it as light sources if valid
        for (float i = 0; i < Mathf.PI * 2f; i += Mathf.PI / 2f)
        {
            Vector3Int checkPosition = new Vector3Int(
                tilePosition.x + Mathf.RoundToInt(Mathf.Sin(i)),
                tilePosition.y + Mathf.RoundToInt(Mathf.Cos(i)), 0);

            if (chunkLoadManager.IsAirBlock(checkPosition) && !lightingManager.GetLightSource(checkPosition))
                lightingManager.CreateLightSource(checkPosition, lightingManager.ambientLightColor, 
                    LightSourceType.AMBIENT, lightingManager.ambientLightStrength);
        }

        /* We want to darken the surroundings now that we placed a block. Simulate this by placing a light and
         * instead of updating it, simply remove the surrounding light using the current color. */
        LightSourceType lightType = lightingManager.IsSameColorSignature(currentColor, lightingManager.ambientLightColor) ? LightSourceType.AMBIENT : LightSourceType.PLACED;
        LightSource existingLight = lightingManager.GetLightSource(tilePosition);
        if (existingLight == null)
            existingLight = lightingManager.CreateLightSource(tilePosition, currentColor, lightType, 
                lightingManager.ambientLightStrength, false);

        if (LightingManager.Instance.SmoothLightRemoval)
            StartCoroutine(lightingManager.RemoveLightSourceSmooth(existingLight));
        else
            lightingManager.RemoveLightSource(existingLight);
    }


    /// <summary>
    /// Removes a tile at the current mouse position.
    /// </summary>
    /// <param name="map"></param>
    public void RemoveTile(Chunk.TilemapType targetMap)
    {
        if ((mousePosition.x < 0 || mousePosition.x >= generationManager.worldWidth) ||
            (mousePosition.y < 0 || mousePosition.y >= generationManager.worldHeight))
            return;
        chunk = chunkLoadManager.GetChunk(mousePosition);
        if (chunk == null)
            return;
        if (chunk.GetChunkTileType(mousePosition, targetMap) == Chunk.TileType.AIR)
            return;

        // Only allow removal of back blocks if there's no front block
        if (targetMap == Chunk.TilemapType.BLOCKS_BACK &&
            chunk.GetChunkTileType(mousePosition, Chunk.TilemapType.BLOCKS_FRONT) != Chunk.TileType.AIR)
            return;

        // Remove tile
        chunk.SetChunkTile(mousePosition, targetMap, null);
        chunk.SetChunkTileType(mousePosition, targetMap, Chunk.TileType.AIR);

        // If the back layer is now air, it means we need to create an ambient light source there
        if (chunk.GetChunkTileType(mousePosition, Chunk.TilemapType.BLOCKS_BACK) == Chunk.TileType.AIR)
        {
            if (!lightingManager.GetLightSource(mousePosition))
            {
                lightingManager.CreateLightSource(mousePosition, lightingManager.ambientLightColor, LightSourceType.AMBIENT, 
                    lightingManager.ambientLightStrength);
            }
        }
        /* Otherwise I just removed a front tile. Brighten the scene by simply simulating a new light with
         * a slightly brighter color. Since we move from front layer to back layer, this difference is the
         * difference between the front and back layer light falloff values. */
        else
        {
            Color currentColor = chunk.GetChunkTileBlendedColor(mousePosition, false);
            if (currentColor != Color.clear && currentColor != Color.black)
            {
                float colorIncrement = lightingManager.LightFalloff - lightingManager.LightFalloffBack;
                currentColor = new Color(
                    Mathf.Clamp(currentColor.r + colorIncrement, 0f, 1f),
                    Mathf.Clamp(currentColor.g + colorIncrement, 0f, 1f),
                    Mathf.Clamp(currentColor.b + colorIncrement, 0f, 1f));
                LightSourceType lightType = lightingManager.IsSameColorSignature(currentColor, lightingManager.ambientLightColor) ? LightSourceType.AMBIENT : LightSourceType.PLACED;
                chunk.SetChunkTileColor(mousePosition, currentColor, false, lightType);

                // Simulate light and remove just the light source object afterwards. Leave its light on the blocks
                LightSource source = lightingManager.CreateLightSource(mousePosition, currentColor, lightType, lightingManager.ambientLightStrength);
                if (LightingManager.Instance.SmoothLightRemoval)
                    StartCoroutine(lightingManager.RemoveLightSourceSmooth(source));
                else
                    lightingManager.RemoveLightSource(source);
            }
        }
    }


    /// <summary>
    /// Execute frame code for the currently active tool.
    /// </summary>
    /// <param name="tool"></param>
    public void ExecuteTool(ToolItem.ToolType tool)
    {
        bool shift = Input.GetKey(KeyCode.LeftShift);
        bool mouseLeft = Input.GetMouseButton(0);
        switch (tool)
        {
            case ToolItem.ToolType.PLACE_TILE:                
                if (GhostCanBePlaced(shift, false))
                {
                    ghost.color = ghostColor;
                    if (mouseLeft)
                        PlaceTile(shift ? Chunk.TilemapType.BLOCKS_BACK
                            : Chunk.TilemapType.BLOCKS_FRONT);
                }
                else
                    ghost.color = ghostColorBlocked;
                break;

            case ToolItem.ToolType.REMOVE_TILE:
                if (mouseLeft)
                    RemoveTile(shift ? Chunk.TilemapType.BLOCKS_BACK
                            : Chunk.TilemapType.BLOCKS_FRONT);
                break;

            case ToolItem.ToolType.PLACE_LIGHT:
                chunk = chunkLoadManager.GetChunk(mousePosition);
                if (chunk != null && chunk.IsLoading == false && chunk.GetChunkTile(mousePosition, Chunk.TilemapType.BLOCKS_BACK) && GhostCanBePlaced())
                {
                    ghost.color = ghostColor;
                    if (mouseLeft)
                    {
                        Color lightColor;
                        if (lightingManager.RandomPlacedLightColor)
                        {
                            normalizedColor = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)).normalized;
                            lightColor = new Color(normalizedColor.x, normalizedColor.y, normalizedColor.z);
                        }
                        else
                            lightColor = lightingManager.placedLightColor;

                        LightSource newSource = lightingManager.CreateLightSource(mousePosition, lightColor, 
                            LightSourceType.PLACED, lightingManager.placedLightStrength, updateLight:false);

                        if (lightingManager.SmoothLightPlacement)
                            StartCoroutine(newSource.UpdateLightSmooth());
                        else
                            newSource.UpdateLight();

                        // Make the light's sprite visible (by default invisible for ambient lights)
                        newSource.GetComponent<SpriteRenderer>().enabled = true;

                        chunk.PlacedLightSources.Add(newSource);
                    }
                }
                else
                    ghost.color = ghostColorBlocked;
                break;

            case ToolItem.ToolType.REMOVE_LIGHT:
                if (mouseLeft)
                {
                    LightSource lightSource = lightingManager.GetLightSource(mousePosition);

                    /* Only target lights that were placed by the player. Ambient lights can only be removed
                     * by placing a block on top of them and is handled in PlaceTile(). */
                    if (lightSource != null && lightSource.state == LightSource.LightSourceState.IDLE && lightSource.LightType == LightSourceType.PLACED)
                    {
                        if (lightingManager.SmoothLightRemoval)
                            StartCoroutine(lightingManager.RemoveLightSourceSmooth(lightSource));
                        else
                            lightingManager.RemoveLightSource(lightSource);
                    }
                }
                break;

            default:
                break;
        }
    }


    private void Update()
    {        
        mousePosition = chunkLoadManager.GetTilePositionAtMouse();
        if (!ghost.gameObject.activeSelf)
            ghost.gameObject.SetActive(true);

        // Move ghost with the mouse (also compensate for sprite origin)
        ghost.transform.position = new Vector3(
            mousePosition.x + (ghost.sprite != null ? ghost.sprite.pivot.x / ghost.sprite.pixelsPerUnit : 0f),
            mousePosition.y + (ghost.sprite != null ? ghost.sprite.pivot.y / ghost.sprite.pixelsPerUnit : 0f),
            mousePosition.z);

        // Toggle tool on / off depending on UI mouse over
        if (EventSystem.current.IsPointerOverGameObject())
        {
            toolIsActive = false;
            if (IsGhosting)
                Unghost();
        }
        else if (!toolIsActive && userInterfaceLighting.SelectedTool != null)
        {
            userInterfaceLighting.SelectedTool.SelectTool();
            toolIsActive = true;
        }

        // Run specific tool
        if (toolIsActive)
            ExecuteTool(userInterfaceLighting.SelectedTool.tool);
    }
}