using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LightingChannelMode
{
    RED,
    GREEN,
    BLUE
}
public enum LightSourceType
{
    AMBIENT,
    PLACED,
    ALL
}

/// <summary>
/// Handles core lighting control.
/// </summary>
public class LightingManager : Singleton<LightingManager>
{
    public bool RandomPlacedLightColor { get; set; }
    public bool LightingIsActive { get; set; }    

    public float LightPenetration { get; private set; }
    public float LightPenetrationBack { get; private set; }
       
    public float LightFalloff { get; private set; }
    public float LightFalloffBack { get; private set; }
    public float BackLayerShadowFactor { get; private set; }

    public bool SmoothLightPlacement { get; set; }
    public bool SmoothLightRemoval { get; set; }   
    public float NightLevel { get; private set; }

    [Header("Block Lighting")]    
    public SliderData lightingData;
    public SliderData cycleData;
    public GameObject lightSourceRoot;
    public GameObject lightSourcePrefab;   
    public LayerMask lightSourceLayer;
    public Color ambientLightColor = Color.white;
    public Color placedLightColor;
    [Range(0f, 1f)]
    public float ambientLightStrength = 1f;
    [Range(0f, 1f)]
    public float placedLightStrength = 1f;

    [Header("Day/Night Cycle")]
    public Color dayColor;
    public Color dawnColor, nightColor;

    private float cycleSpeed, cycleInterval, cycleSpan, dawnSpan, dawnFocusPoint, cycleOffset;

    // Caching
    private int worldWidth;
    private float dawnDistance, dawnFactor;
    private Vector3Int tilePosition, tilePositionNext, rectPosition;
    private Chunk positionChunk;
    private GenerationManager generationManager;
    private ChunkLoadManager chunkLoadManager;
    private Rect rectPositions;
    private Rect rectChunk;
    private Collider2D hit;
    private Collider2D[] hits;
    private float firstScale;
    private float secondScale;

    #region Lifetime
    private void Start()
    {
        LightingIsActive = true;
        RandomPlacedLightColor = true;
        SmoothLightPlacement = true;
        SmoothLightRemoval = false;        

        generationManager = GenerationManager.Instance;
        chunkLoadManager = ChunkLoadManager.Instance;
        worldWidth = generationManager.worldWidth;

        cycleOffset = Mathf.PI * 1.5f;

        UpdateLightSliderData();
        UpdateCycleData();
        StartCoroutine(PerformDayNightTimer());
    }

    private void Update()
    {
        cycleOffset += cycleSpeed * Time.deltaTime;
        NightLevel = Mathf.Sin(cycleOffset) * cycleSpan + cycleSpan;

        dawnDistance = Mathf.Abs(NightLevel - dawnFocusPoint);
        dawnFactor = Mathf.Clamp(-dawnDistance + dawnSpan, 0f, dawnSpan);
        Camera.main.backgroundColor = Color.Lerp(Color.Lerp(dayColor, nightColor, NightLevel / (cycleSpan * 2f)), dawnColor, dawnFactor / dawnSpan);
    }
    #endregion
    
    
    /// <summary>
    /// Updates light penetration values. Called whenever a lighting slider value changes.
    /// </summary>
    public void UpdateLightSliderData()
    {               
        LightPenetration = lightingData.GetSliderData(SliderData.SliderField.LIGHTING_PENETRATION);
        LightPenetrationBack = lightingData.GetSliderData(SliderData.SliderField.LIGHTING_PENETRATION_BACK);
        BackLayerShadowFactor = lightingData.GetSliderData(SliderData.SliderField.LIGHTING_BACK_SHADOW);

        LightFalloff = 1f / LightPenetration;
        LightFalloffBack = 1f / LightPenetrationBack;
    }

    /// <summary>
    /// Updates day/night cycle values. Called whenever a day/night cycle slider value changes.
    /// </summary>
    public void UpdateCycleData()
    {
        cycleSpeed = cycleData.GetSliderData(SliderData.SliderField.CYCLE_SPEED);
        cycleInterval = cycleData.GetSliderData(SliderData.SliderField.CYCLE_INTERVAL);
        cycleSpan = cycleData.GetSliderData(SliderData.SliderField.CYCLE_SPAN);
        dawnSpan = cycleData.GetSliderData(SliderData.SliderField.DAWN_SPAN);
        dawnFocusPoint = cycleData.GetSliderData(SliderData.SliderField.DAWN_FOCUS_POINT);
    }


    /// <summary>
    /// Returns whether the given colors have the same channel signature, independent of channel strength.
    /// Mainly used to see if a given color originated from an ambient light or not, without needing a reference.
    /// 
    /// Scales both colors as close to 1 as possible, without changing the channel differences.
    /// For example, (0.2, 0.5, 0.2) would be scaled to (0.7, 1, 0.7).
    /// Then it compares magnitudes to decide whether they are similar.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public bool IsSameColorSignature(Color first, Color second)
    {
        firstScale = 1f - first.maxColorComponent;
        secondScale = 1f - second.maxColorComponent;

        return Mathf.Abs(
            new Vector3(second.r + secondScale, second.g + secondScale, second.b + secondScale).magnitude -
            new Vector3(first.r + firstScale, first.g + firstScale, first.b + firstScale).magnitude) <
            LightFalloffBack * 0.9999f;
    }

    /// <summary>
    /// General timer for executing day/night cycle updates.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformDayNightTimer()
    {
        while (true)
        {
            if (cycleSpeed > 0f)
                yield return StartCoroutine(PerformDayNightCycle());

            yield return new WaitForSeconds(cycleInterval);
        }
    }

    /// <summary>
    /// Performs one iteration over all loaded chunks and refreshes their color to fit the current night level.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PerformDayNightCycle()
    {        
        foreach (Chunk chunk in chunkLoadManager.GetAllChunks())
        {
            if (chunk.IsLoading || chunk.IsCompletelyDark)
                continue;

            for (int h = 0; h < generationManager.chunkSize; h++)            
            {
                for (int v = 0; v < generationManager.chunkSize; v++)
                {                    
                    tilePosition = new Vector3Int(chunk.Position.x + h, chunk.Position.y + v, 0);

                    // Refresh the color at this position. Only changes visuals (the Tilemaps)
                    chunk.SetChunkTileColor(tilePosition, chunk.GetChunkTileBlendedColor(tilePosition), true, LightSourceType.ALL);

                    if (PerformanceManager.Instance.HitFrameLimit())
                        yield return null;
                }
            }
        }
    }

    /// <summary>
    /// Creates a light source with the given color and light strength.
    /// Optionally able to turn off automatic light update after creation.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="color"></param>
    /// <param name="type"></param>
    /// <param name="strength"></param>
    /// <param name="updateLight"></param>
    /// <returns></returns>
    public LightSource CreateLightSource(Vector3Int position, Color color, LightSourceType type = LightSourceType.AMBIENT, 
        float strength = 1f, bool updateLight = true)
    {
        LightSource lightSource = Instantiate(lightSourcePrefab, position, 
            Quaternion.identity, lightSourceRoot.transform).GetComponent<LightSource>();
        lightSource.Initialize(color, strength, type, LightPenetration, LightPenetrationBack);

        if (updateLight)
            lightSource.UpdateLight();

        return lightSource;
    }

    /// <summary>
    /// Creates all ambient LightSources for the given chunk.
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="updateLights"></param>
    /// <returns></returns>
    public List<LightSource> CreateAmbientLightSources(Chunk chunk, bool updateLights = true)
    {
        rectPositions = new Rect();
        rectChunk = new Rect(
            new Vector2(chunk.Position.x - 1, chunk.Position.y - 1),
            new Vector2(generationManager.chunkSize + 1, generationManager.chunkSize + 1));
        List<LightSource> chunkLights = new List<LightSource>();

        // Start and end one block outside the chunk to fix light source gaps inbetween chunks
        for (int h = -1; h < generationManager.chunkSize; h++)
        {
            if ((chunk.Position.x + h < 0 || chunk.Position.x + h + 1 >= worldWidth))
                continue;

            // Make a rect from the current surface level position to the next
            tilePosition = new Vector3Int(chunk.Position.x + h, generationManager.surfaceHeights[chunk.Position.x + h] + 1, 0);
            tilePositionNext = new Vector3Int(chunk.Position.x + h + 1, generationManager.surfaceHeights[chunk.Position.x + h + 1] + 1, 0);
            rectPositions.position = new Vector2(tilePosition.x, tilePosition.y);
            rectPositions.size = new Vector2(1f, tilePositionNext.y - tilePosition.y);

            // Check if the chunk rect contains this rect to see if the surface height is within this chunk
            if (!rectChunk.Contains(new Vector2(rectPositions.xMin, rectPositions.yMax), true) &&
                !rectChunk.Contains(new Vector2(rectPositions.xMax, rectPositions.yMin), true))
                continue;

            /* If any tiles within rectPositions are valid for an ambient light source, create one on that position.
             * Compensate for inverse Rect bounds first, since the Rect.*min and Rect.*max don't account for this. */
            float minimumX = Mathf.Min(rectPositions.xMin, rectPositions.xMax);
            float maximumX = Mathf.Max(rectPositions.xMin, rectPositions.xMax);
            float minimumY = Mathf.Min(rectPositions.yMin, rectPositions.yMax);
            float maximumY = Mathf.Max(rectPositions.yMin, rectPositions.yMax);
            for (float rectX = minimumX; rectX <= maximumX; rectX++)
            {
                for (float rectY = minimumY; rectY <= maximumY; rectY++)
                {
                    rectPosition = new Vector3Int((int)rectX, (int)rectY, 0);
                    positionChunk = chunkLoadManager.GetChunk(rectPosition);

                    if (chunkLoadManager.IsAirBlock(rectPosition, positionChunk) && !GetLightSource(rectPosition))
                        if (chunkLoadManager.HasAdjacentTiles(rectPosition, positionChunk))
                            chunkLights.Add(CreateLightSource(rectPosition, ambientLightColor, LightSourceType.AMBIENT, ambientLightStrength, updateLights));
                }
            }
        }
        return chunkLights;
    }

    /// <summary>
    /// Creates the given chunk's light sources and builds their light smoothly.
    /// </summary>
    /// <param name="chunk"></param>
    /// <returns></returns>
    public IEnumerator PerformCreateChunkLighting(Chunk chunk)
    {
        // Create ambient lights for each new chunk. Disabled automatic update since we want to smooth this out to retain high performance
        chunk.AmbientLightSources = CreateAmbientLightSources(chunk, false);
        if (chunk.AmbientLightSources.Count > 0)
        {
            /* Reverse the list of lights depending on the chunk's position, so it always starts at the
             * side the player would see first. */
            if (chunk.Position.x < Camera.main.transform.position.x)
                chunk.AmbientLightSources.Reverse();

            // Update these ambient lights smoothly, all at once.
            foreach (LightSource light in chunk.AmbientLightSources)
                StartCoroutine(light.UpdateLightSmooth());

            // Make sure all the lights have finished spreading their light completely before moving on
            while (true)
            {
                bool breakOut = true;
                foreach (LightSource light in chunk.AmbientLightSources)
                {
                    if (light != null && light.state != LightSource.LightSourceState.IDLE)
                    {
                        breakOut = false;
                        break;
                    }
                }

                if (breakOut)
                    break;

                yield return null;
            }
        }
    }


    /// <summary>
    /// Removes the given LightSource object.
    /// By default, tries to remove its light first.
    /// </summary>
    /// <param name="light"></param>
    /// <param name="removeLight"></param>
    public void RemoveLightSource(LightSource light, bool removeLight = true)
    {
        if (light == null)
            return;

        if (removeLight)
            light.RemoveLight();

        Destroy(light.gameObject);
    }

    /// <summary>
    /// Removes the given LightSource object.
    /// By default, tries to smoothly remove its light first.
    /// </summary>
    /// <param name="light"></param>
    /// <param name="removeLight"></param>
    public IEnumerator RemoveLightSourceSmooth(LightSource light, bool removeLight = true)
    {
        if (light == null)
            yield break;

        if (removeLight)
            yield return StartCoroutine(light.RemoveLightSmooth());

        Destroy(light.gameObject);
    }


    /// <summary>
    /// Returns the LightSource at the given position or null if no LightSource was found.
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public LightSource GetLightSource(Vector3Int position)
    {
        hit = Physics2D.OverlapPoint(new Vector2(position.x + 0.5f, position.y + 0.5f), lightSourceLayer);
        return hit != null ? hit.GetComponent<LightSource>() : null;
    }


    /// <summary>
    /// Returns all LightSources that are inside or overlap with the given Rect bounds.
    /// </summary>
    /// <param name="rect"></param>
    /// <returns></returns>
    public List<LightSource> GetLightSources(Rect rect)
    {
        List<LightSource> lights = new List<LightSource>();
        hits = Physics2D.OverlapBoxAll(rect.center, rect.size / 2f, 0f, lightSourceLayer);

        foreach (Collider2D coll in hits)
        {
            LightSource light = coll.GetComponent<LightSource>();
            if (light != null)
                lights.Add(light);
        }
        return lights;
    }
}