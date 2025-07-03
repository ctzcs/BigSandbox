using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the UI for Terrainify 2D.
/// 
/// You can freely remove it from the game or disable it whenever you like, it won't
/// break anything. It's just there to help experiment with the features the asset has to offer.
/// </summary>
public class UserInterfaceTerrainify : Singleton<UserInterfaceTerrainify>
{
    public bool SmoothPerlinMap { get; set; }

    [Header("Grid Items")]
    public GameObject gridItemPrefab;
    public GameObject gridItemRoot;
    [Header("Customizer")]
    public GameObject customizerPanel;
    public GameObject customizerEntryRoot;
    public TextMeshProUGUI customizerTitle;
    [Header("Perlin Preview Map")]
    public GameObject perlinPreviewRoot;
    public RawImage perlinPreviewImage;
    public Toggle perlinPreviewAllToggle;
    public GridItem SelectedItem { get; set; }
    public SliderObject MousedOverSlider { get; set; }
    public List<GridItem> GridItems { get; private set; }
    public List<SliderObject> CustomizerSliders { get; private set; }
    
    private int selectedItemIndex;
    private Texture2D perlinTexture;
    private Color[] perlinColorMap;
    private float scrollwheelInput;
    private Canvas canvas;
    private UnityEngine.Coroutine mapRoutine;


    private void Awake()
    {
        SmoothPerlinMap = false;
    }

    private void Start()
    {
        canvas = GetComponentInChildren<Canvas>();

        // Fill GridItems using the imported scriptable object data
        GridItems = new List<GridItem>();              
        foreach (ScriptableObject obj in GenerationManager.Instance.blockData)
        {
            GridItem gridItem = Instantiate(gridItemPrefab, gridItemRoot.transform).GetComponent<GridItem>();
            gridItem.InitializeItem(obj as SliderDataCustomizer);
            GridItems.Add(gridItem);
        }

        // Retrieve all customizer sliders in the UI
        CustomizerSliders = new List<SliderObject>();
        foreach (Transform child in customizerEntryRoot.transform)
        {
            SliderObject customizerSlider = child.GetComponent<SliderObject>();
            if (customizerSlider != null)
                CustomizerSliders.Add(customizerSlider);
        }

        // Prepare the Perlin layer previewer
        perlinTexture = new Texture2D(
            (int)perlinPreviewImage.rectTransform.sizeDelta.x, 
            (int)perlinPreviewImage.rectTransform.sizeDelta.y);
        perlinPreviewImage.texture = perlinTexture;

        // Select the first item for customization
        if (GridItems.Count > 0)
            GridItems[0].SelectItem();
    }


    private void Update()
    {
        scrollwheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (scrollwheelInput != 0f)
        {
            // Scroll through grid items if shift is held together with mouse scroll
            if (Input.GetKey(KeyCode.LeftShift))
                ScrollToAdjacentItem(scrollwheelInput < 0f ? 1 : -1);
        }

        if (Input.GetKeyDown(KeyCode.Z))
            canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
    }


    /// <summary>
    /// Deselects the current grid item and selects the next grid item.
    /// </summary>
    /// <param name="direction"></param>
    public void ScrollToAdjacentItem(int direction)
    {
        // Wrap around
        selectedItemIndex += Math.Sign(direction);
        if (selectedItemIndex < 0)
            selectedItemIndex = GridItems.Count - 1;
        else if (selectedItemIndex > GridItems.Count - 1)
            selectedItemIndex = 0;

        // Skip the default block that is hidden automatically
        if (GridItems[selectedItemIndex].Data == GenerationManager.Instance.defaultBlock)
        {
            ScrollToAdjacentItem(direction);
            return;
        }

        GridItems[selectedItemIndex].SelectItem();
    }


    /// <summary>
    /// Regenerates the Perlin layer previewer.
    /// It can display any duo of customizer sliders that are moused over
    /// or a mix of other slider sets together into one blended Perlin map.
    /// </summary>
    /// <param name="perlinSpeed"></param>
    /// <param name="perlinLevel"></param>
    /// <param name="zonePerlinSpeed"></param>
    /// <param name="zonePerlinLevel"></param>
    /// <param name="mapPerlinSpeed"></param>
    /// <param name="mapPerlinLevel"></param>
    private IEnumerator CalculatePerlinPreviewMap(float perlinSpeed, float perlinLevel,
        float zonePerlinSpeed = 0f, float zonePerlinLevel = 0f, float mapPerlinSpeed = 0f, float mapPerlinLevel = 0f)
    {
        perlinColorMap = new Color[perlinTexture.width * perlinTexture.height];
        for (int v = 0; v < perlinTexture.height; v++)
        {
            for (int h = 0; h < perlinTexture.width; h++)
            {
                Vector3Int samplePosition = new Vector3Int(h, v, 0);
                
                /* Sample all given layers for a black or white pixel result.
                 * Depth is disabled since we want the whole map to display everything. */
                float sample = GenerationManager.Instance.CheckPerlinEligibility(
                    samplePosition, -1f, -1f, perlinSpeed, perlinLevel, zonePerlinSpeed, 
                    zonePerlinLevel, mapPerlinSpeed, mapPerlinLevel) ? 1f : 0f;

                perlinColorMap[h + v * perlinTexture.width] = new Color(sample, sample, sample);

                if (SmoothPerlinMap && PerformanceManager.Instance.HitFrameLimit())
                    yield return null;
            }
            if (SmoothPerlinMap)
            {
                perlinTexture.SetPixels(perlinColorMap);
                perlinTexture.Apply();
                perlinPreviewImage.texture = perlinTexture;
            }
        }

        // Apply the color map to the texture
        perlinTexture.SetPixels(perlinColorMap);
        perlinTexture.Apply();
        perlinPreviewImage.texture = perlinTexture;
    }

    /// <summary>
    /// Updates the perlin map previewer with the right data.
    /// </summary>
    /// <returns></returns>
    public void UpdatePerlinPreviewMap()
    {
        SliderDataCustomizer dataObject = SelectedItem.Data;
        if (mapRoutine != null)
            StopCoroutine(mapRoutine);

        // Multi-layer view
        if (perlinPreviewAllToggle.isOn)
        {
            mapRoutine = StartCoroutine(CalculatePerlinPreviewMap(
                dataObject.GetSliderData(SliderData.SliderField.PERLIN_SPEED),
                dataObject.GetSliderData(SliderData.SliderField.PERLIN_LEVEL),
                dataObject.GetSliderData(SliderData.SliderField.ZONE_PERLIN_SPEED),
                dataObject.GetSliderData(SliderData.SliderField.ZONE_PERLIN_LEVEL),
                dataObject.GetSliderData(SliderData.SliderField.MAP_PERLIN_SPEED),
                dataObject.GetSliderData(SliderData.SliderField.MAP_PERLIN_LEVEL)));
        }
        // Per-layer view
        else if (MousedOverSlider != null)
        {
            switch (MousedOverSlider.field)
            {
                case SliderData.SliderField.PERLIN_SPEED:
                case SliderData.SliderField.PERLIN_LEVEL:
                    mapRoutine = StartCoroutine(CalculatePerlinPreviewMap(
                        dataObject.GetSliderData(SliderData.SliderField.PERLIN_SPEED),
                        dataObject.GetSliderData(SliderData.SliderField.PERLIN_LEVEL)));
                    break;
                case SliderData.SliderField.ZONE_PERLIN_SPEED:
                case SliderData.SliderField.ZONE_PERLIN_LEVEL:
                    mapRoutine = StartCoroutine(CalculatePerlinPreviewMap(
                        dataObject.GetSliderData(SliderData.SliderField.ZONE_PERLIN_SPEED),
                        dataObject.GetSliderData(SliderData.SliderField.ZONE_PERLIN_LEVEL)));
                    break;
                case SliderData.SliderField.MAP_PERLIN_SPEED:
                case SliderData.SliderField.MAP_PERLIN_LEVEL:
                    mapRoutine = StartCoroutine(CalculatePerlinPreviewMap(
                        dataObject.GetSliderData(SliderData.SliderField.MAP_PERLIN_SPEED),
                        dataObject.GetSliderData(SliderData.SliderField.MAP_PERLIN_LEVEL)));
                    break;
            }
        }
    }


    /// <summary>
    /// Restarts generation and clears all currently active chunks from the game.
    /// </summary>
    public void OnButtonGenerate()
    {
        GenerationManager.Instance.Initialize();
        ChunkLoadManager.Instance.ClearAllChunks();        
    }


    /// <summary>
    /// Randomize the seed in the game.
    /// </summary>
    public void OnButtonSeed()
    {
        GenerationManager.Instance.SetSeed();
    }
}