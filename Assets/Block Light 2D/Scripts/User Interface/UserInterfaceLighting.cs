using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the UI for Block Lighting Engine 2D.
/// 
/// You can freely remove it from the game or disable it whenever you like, it won't
/// break anything. It's just there to help experiment with the features the asset has to offer.
/// </summary>
public class UserInterfaceLighting : Singleton<UserInterfaceLighting>
{
    [Header("General")]
    public GameObject tileSelectRoot;
    public GameObject gridToolRoot, gridTileRoot;
    public bool IsOpened
    {
        get { return canvas.gameObject.activeSelf; }
    }
    public ToolItem SelectedTool { get; set; }
    public TileItem SelectedTile { get; set; }
    public List<ToolItem> Tools { get; private set; }
    public List<TileItem> Tiles { get; private set; }

    private int selectedToolIndex, selectedTileIndex;
    private Canvas canvas;


    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();

        // Import tools and tiles into lists
        Tools = new List<ToolItem>();
        Tiles = new List<TileItem>();
        foreach (Transform transform in gridToolRoot.transform)
        {
            ToolItem tool = transform.GetComponent<ToolItem>();
            if (tool != null)
                Tools.Add(tool);
        }
        foreach (Transform transform in gridTileRoot.transform)
        {
            TileItem tile = transform.GetComponent<TileItem>();
            if (tile != null)
                Tiles.Add(tile);
        }
        if (Tools.Count > 0)
            SelectedTool = Tools[0];
        if (Tiles.Count > 0)
            SelectedTile = Tiles[0];
    }


    private void Update()
    {
        // Simple detection of key input for this UI
        float scrollwheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollwheelInput != 0f)
        {
            if (Input.GetKey(KeyCode.LeftControl))
                ScrollToTool(scrollwheelInput < 0f ? selectedToolIndex + 1 : selectedToolIndex - 1);
            else if (Input.GetKey(KeyCode.LeftAlt) && tileSelectRoot.activeSelf)
                ScrollToTile(scrollwheelInput < 0f ? selectedTileIndex + 1 : selectedTileIndex - 1);
        }

        if (Input.GetKeyDown(KeyCode.Z))
            canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
    }


    /// <summary>
    /// Deselects the current tool and selects the next tool.
    /// </summary>
    /// <param name="toolIndex"></param>
    public void ScrollToTool(int toolIndex)
    {
        if (toolIndex < 0)
            toolIndex = Tools.Count - 1;
        else if (toolIndex > Tools.Count - 1)
            toolIndex = 0;
        selectedToolIndex = toolIndex;
        Tools[selectedToolIndex].SelectTool();
    }

    /// <summary>
    /// Deselects the current tile and selects the next tile.
    /// </summary>
    /// <param name="tileIndex"></param>
    private void ScrollToTile(int tileIndex)
    {
        if (tileIndex < 0)
            tileIndex = Tiles.Count - 1;
        else if (tileIndex > Tiles.Count - 1)
            tileIndex = 0;
        selectedTileIndex = tileIndex;
        Tiles[selectedTileIndex].SelectTile();
    }


    /// <summary>
    /// Toggles all lighting effects on or off.
    /// </summary>
    public void ToggleLighting()
    {
        LightingManager.Instance.LightingIsActive = !LightingManager.Instance.LightingIsActive;
        if (LightingManager.Instance.LightingIsActive)
            EnableAllLighting();
        else
            DisableAllLighting();
    }


    /// <summary>
    /// Enable all lighting effects. This affects only the Tilemaps and not any underlying
    /// color maps.
    /// </summary>
    public void EnableAllLighting()
    {
        foreach (Chunk chunk in ChunkLoadManager.Instance.GetAllChunks())
            chunk.EnableLighting();
    }


    /// <summary>
    /// Disables all lighting effects. This affects only the Tilemaps and not any underlying
    /// color maps. Useful for being able to see all of the terrain while experimenting with
    /// the Terrainify 2D generation for example.
    /// </summary>
    public void DisableAllLighting()
    {
        foreach (Chunk chunk in ChunkLoadManager.Instance.GetAllChunks())
            chunk.DisableLighting();
    }
}