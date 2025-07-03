using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A lightweight struct container that represents a block's lighting data
/// while updating or removing lighting.
/// 一个block的灯光数据，当更新或者移动灯光的时候
/// </summary>
public struct LightNode
{
    public Vector3Int position;
    public Color color;
    public Chunk chunk;
    public LightSourceType type;

    public LightNode(Vector3Int position, Color color, Chunk chunk, LightSourceType type)
    {
        this.position = position;
        this.color = color;
        this.chunk = chunk;
        this.type = type;
    }
}

/// <summary>
/// 游戏中的光源
/// Describes a source of light in the game.
/// </summary>
public class LightSource : MonoBehaviour
{
    public float LightStrength { get; set; }
    public float LightPenetration { get; private set; }
    public float LightPenetrationBack { get; private set; }
    public float LightFalloff { get; private set; }
    public float LightFalloffBack { get; private set; }
    public LightSourceType LightType { get; set; }
    public Vector3Int Position { get; set; }

    public enum LightSourceState
    {
        IDLE,
        UPDATE,
        REMOVE
    }
    public LightSourceState state;
    public Color LightColor;
    public Queue<LightNode> queueLightPasses = new Queue<LightNode>();

    private Queue<LightNode> queueLightRemovalPasses = new Queue<LightNode>();
    private Queue<LightNode> queueRemoveInvalidPositions = new Queue<LightNode>();
    private List<Vector3Int> removalPositions = new List<Vector3Int>();
    private List<LightSource> removalLights = new List<LightSource>();
    private LightingManager lightingManager;
    private GenerationManager generationManager;
    private ChunkLoadManager chunkLoadManager;
    private int worldWidth, worldHeight;
    private Vector3Int vectorRight, vectorUp, vectorLeft, vectorDown;
    private Chunk chunkDirection;
    private Vector3Int newChunkPosition;
    private Color currentColor;
    private float lightValueDirection, blockFalloff;
    private Chunk.TileType typeBack, typeFront;
    private float passLightValue;
    private float lightPassThreshold;
    private Vector3Int lightPositionDirection;
    private Color newColor;

    #region Lifetime

    

    
    private void Awake()
    {
        // Save a Vector3Int version of its position for easy use with Tilemaps
        Position = new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);

        // Caching of often used values to prevent long lookups or GC for many light iterations at once
        lightingManager = LightingManager.Instance;
        generationManager = GenerationManager.Instance;
        chunkLoadManager = ChunkLoadManager.Instance;
        worldWidth = generationManager.worldWidth;
        worldHeight = generationManager.worldHeight;
        
        vectorRight = Vector3Int.right;
        vectorUp = Vector3Int.up;
        vectorLeft = Vector3Int.left;
        vectorDown = Vector3Int.down;
    }

    /// <summary>
    /// Set the light's core values and settings.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="strength"></param>
    /// <param name="lightType"></param>
    /// <param name="penetration"></param>
    /// <param name="penetrationBack"></param>
    public void Initialize(Color color, float strength, LightSourceType lightType, float penetration, float penetrationBack)
    {
        LightColor = color;
        LightStrength = strength;
        LightType = lightType;
        LightPenetration = penetration;
        LightPenetrationBack = penetrationBack;

        LightFalloff = 1.0f / LightPenetration;
        LightFalloffBack = 1.0f / LightPenetrationBack;
        lightPassThreshold = LightFalloffBack * 0.99999f;
    }
    #endregion

    /// <summary>
    /// Clears all tracked data used in light calculations.
    /// </summary>
    /// <param name="clearLists"></param>
    /// <param name="clearQueues"></param>
    private void ClearCachedData(bool clearLists = true, bool clearQueues = true)
    {
        if (clearLists)
        {
            removalLights.Clear();
            removalPositions.Clear();
        }
        if (clearQueues)
        {
            queueLightPasses.Clear();
            queueLightRemovalPasses.Clear();
            queueRemoveInvalidPositions.Clear();
        }
    }


    #region Update light
    /// <summary>
    /// Updates the given LightSource by trying to spread its light to surrounding tiles.
    /// This works on an overwrite basis. If the surrounding light is stronger, it stops.
    /// This is done in a single frame and useful for things like placing torches for example
    /// where you want the light to be there instantly. If you allow smoothing, look for the 
    /// UpdateLightSmooth() Coroutine instead.
    /// </summary>
    public void UpdateLight()
    {
        state = LightSourceState.UPDATE;

        ClearCachedData();        

        // Create a struct as lightweight data storage per visited block
        LightNode lightNode;
        lightNode.position = Position;
        lightNode.color = LightColor * LightStrength;
        lightNode.type = LightType;
        lightNode.chunk = chunkLoadManager.GetChunk(Position);
        if (lightNode.chunk == null)
            return;

        // Set the color of the light source's own tile
        Color currentColor = lightNode.chunk.GetChunkTileBlendedColor(Position, dayNight: false);
        lightNode.chunk.SetChunkTileColor(Position, new Color(
            Mathf.Max(currentColor.r, LightColor.r * LightStrength),
            Mathf.Max(currentColor.g, LightColor.g * LightStrength),
            Mathf.Max(currentColor.b, LightColor.b * LightStrength)), false, LightType);

        // Spread light
        queueLightPasses.Enqueue(lightNode);
        PerformLightPasses(queueLightPasses);

        state = LightSourceState.IDLE;
    }

    /// <summary>
    /// Generates colored lighting for every queued node.
    /// Makes use of Breadth-First Search using a FIFO Queue. This way, all blocks are only visited once.
    /// Executes passes per light channel (RGB) to ensure correct blending.
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="redChannel"></param>
    /// <param name="greenChannel"></param>
    /// <param name="blueChannel"></param>
    /// <returns></returns>
    public void PerformLightPasses(Queue<LightNode> queue, bool redChannel = true,
        bool greenChannel = true, bool blueChannel = true)
    {
        if (!redChannel && !greenChannel && !blueChannel)
            return;

        /* Generate a backup queue to refill the original queue after each channel
         * (since every channel execution empties the queue). */
        Queue<LightNode> queueBackup = new Queue<LightNode>(queue);

        // Spread light for each channel
        if (redChannel)
        {
            queue = new Queue<LightNode>(queueBackup);
            while (queue.Count > 0)
                ExecuteLightingPass(queue, LightingChannelMode.RED);
        }
        if (greenChannel)
        {
            queue = new Queue<LightNode>(queueBackup);
            while (queue.Count > 0)
                ExecuteLightingPass(queue, LightingChannelMode.GREEN);
        }
        if (blueChannel)
        {
            queue = new Queue<LightNode>(queueBackup);
            while (queue.Count > 0)
                ExecuteLightingPass(queue, LightingChannelMode.BLUE);
        }
    }
    #endregion

    #region Update light smooth
    /// <summary>
    /// Updates the given LightSource by trying to spread its light to surrounding tiles.
    /// Works exactly the same as UpdateLight(), but it'll space out calculations over multiple frames
    /// whenever the iteration limit is reached. This works on an overwrite basis. If the surrounding 
    /// light is stronger, it stops.
    /// </summary>
    /// <returns></returns>
    public IEnumerator UpdateLightSmooth()
    {
        state = LightSourceState.UPDATE;

        ClearCachedData();

        // Create a struct as lightweight data storage per block
        LightNode lightNode;
        lightNode.position = Position;
        lightNode.color = LightColor * LightStrength;
        lightNode.type = LightType;
        lightNode.chunk = chunkLoadManager.GetChunk(Position);
        if (lightNode.chunk == null)
            yield break;

        // Set the color of the light source's own tile
        Color currentColor = lightNode.chunk.GetChunkTileBlendedColor(Position);
        lightNode.chunk.SetChunkTileColor(Position, new Color(
            Mathf.Max(currentColor.r, LightColor.r * LightStrength),
            Mathf.Max(currentColor.g, LightColor.g * LightStrength),
            Mathf.Max(currentColor.b, LightColor.b * LightStrength)), false, LightType);

        // Spread light
        queueLightPasses.Enqueue(lightNode);
        yield return StartCoroutine(PerformLightPassesSmooth(queueLightPasses));

        state = LightSourceState.IDLE;
    }

    /// <summary>
    /// Generates colored lighting for every queued node.
    /// Makes use of Breadth-First Search using a FIFO Queue. This way, all blocks are only visited once.
    /// Executes passes per light channel (RGB) to ensure correct blending.
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="redChannel"></param>
    /// <param name="greenChannel"></param>
    /// <param name="blueChannel"></param>
    /// <returns></returns>
    public IEnumerator PerformLightPassesSmooth(Queue<LightNode> queue, bool redChannel = true,
        bool greenChannel = true, bool blueChannel = true)
    {
        if (!redChannel && !greenChannel && !blueChannel)
            yield break;

        /* Generate a backup queue to refill the original queue after each channel
         * (since every channel execution empties the queue). */
        Queue<LightNode> queueBackup = new Queue<LightNode>(queue);

        // Spread light for each channel
        if (redChannel)
        {
            queue = new Queue<LightNode>(queueBackup);
            while (queue.Count > 0)
            {
                ExecuteLightingPass(queue, LightingChannelMode.RED);

                if (PerformanceManager.Instance.HitFrameLimit())
                    yield return null;
            }
        }
        if (greenChannel)
        {
            queue = new Queue<LightNode>(queueBackup);
            while (queue.Count > 0)
            {
                ExecuteLightingPass(queue, LightingChannelMode.GREEN);

                if (PerformanceManager.Instance.HitFrameLimit())
                    yield return null;
            }
        }
        if (blueChannel)
        {
            queue = new Queue<LightNode>(queueBackup);
            while (queue.Count > 0)
            {
                ExecuteLightingPass(queue, LightingChannelMode.BLUE);

                if (PerformanceManager.Instance.HitFrameLimit())
                    yield return null;
            }
        }
    }
    #endregion

    #region Light spreading passes
    /// <summary>
    /// 按照左下右上的顺序，依次检测四个方向的光照
    /// Spreads the given channel light of all queued nodes in 4 directions.
    /// This is one pass that scans left, down, right and up in that order, so
    /// at most 4 tiles are affected by a single pass per node.
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="mode"></param>
    private void ExecuteLightingPass(Queue<LightNode> queue, LightingChannelMode mode)
    {
        // Get the LightNode that's first in line
        LightNode light = queue.Dequeue();

        /* Obtain light values from the corresponding channel to lessen overhead
         * on extension passes. */
        switch (mode)
        {
            case LightingChannelMode.RED:
                if (light.color.r <= 0f)
                    return;
                passLightValue = light.color.r;
                break;
            case LightingChannelMode.GREEN:
                if (light.color.g <= 0f)
                    return;
                passLightValue = light.color.g;
                break;
            case LightingChannelMode.BLUE:
                if (light.color.b <= 0f)
                    return;
                passLightValue = light.color.b;
                break;
            default:
                return;
        }

        // Try and spread its light to surrounding blocks
        ExtendQueueLightPass(queue, light, passLightValue, vectorLeft, mode);
        ExtendQueueLightPass(queue, light, passLightValue, vectorDown, mode);
        ExtendQueueLightPass(queue, light, passLightValue, vectorRight, mode);
        ExtendQueueLightPass(queue, light, passLightValue, vectorUp, mode);
    }

    /// <summary>
    /// 尝试将光拓展到给定的方向，如果成功则将其添加到队列中
    /// Try to extend the light in the given direction and add to the queue if successful.
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="lightNode"></param>
    /// <param name="lightValue"></param>
    /// <param name="direction"></param>
    /// <param name="mode"></param>
    private void ExtendQueueLightPass(Queue<LightNode> queue, LightNode lightNode, float lightValue,
        Vector3Int direction, LightingChannelMode mode)
    {
        lightPositionDirection = lightNode.position + direction;
        if ((lightPositionDirection.x < 0 || lightPositionDirection.x >= worldWidth) ||
            (lightPositionDirection.y < 0 || lightPositionDirection.y >= worldHeight))
            return;

        /* Get the right chunk for this position.
         * We don't want to use GetChunk on every check, just the ones where we enter a new chunk. */
        chunkDirection = lightNode.chunk;
        newChunkPosition = new Vector3Int(
            lightPositionDirection.x / generationManager.chunkSize,
            lightPositionDirection.y / generationManager.chunkSize, 0);
        if (newChunkPosition != chunkDirection.ChunkPosition)
        {
            chunkDirection = chunkLoadManager.GetChunk(lightPositionDirection);
            if (chunkDirection == null)
                return;
            else
                chunkDirection.IsCompletelyDark = false;
        }

        // Calculate the light falloff for the position in the given direction
        typeBack = chunkDirection.GetChunkTileType(lightPositionDirection, Chunk.TilemapType.BLOCKS_BACK);
        typeFront = chunkDirection.GetChunkTileType(lightPositionDirection, Chunk.TilemapType.BLOCKS_FRONT);
        if (typeFront != Chunk.TileType.AIR)
            blockFalloff = LightFalloff;
        else if (typeBack != Chunk.TileType.AIR)
            blockFalloff = LightFalloffBack;
        else
            return;

        currentColor = chunkDirection.GetChunkTileColor(lightPositionDirection, lightNode.type);
        lightValueDirection =
            (mode == LightingChannelMode.RED ?
                currentColor.r :
                (mode == LightingChannelMode.GREEN ?
                    currentColor.g :
                    currentColor.b));

        /* Spread light if the tile's channel color in this direction is lower in lightValue even after compensating
         * its falloff. lightPassThreshold acts as an additional performance boost and should be < light falloff back.
         * 如果tile在这个方向上的通道颜色在光值补偿后仍然较低，则应扩散光线。lightPassThreshold 作为一个额外的性能提升，应该小于 light falloff back。*/
        if (lightValueDirection + blockFalloff + lightPassThreshold < lightValue)
        {
            lightValue = Mathf.Clamp(lightValue - blockFalloff, 0f, 1f);
            newColor =
                (mode == LightingChannelMode.RED ?
                    new Color(lightValue, currentColor.g, currentColor.b) :
                (mode == LightingChannelMode.GREEN ?
                    new Color(currentColor.r, lightValue, currentColor.b) :
                    new Color(currentColor.r, currentColor.g, lightValue)));

            LightNode newLightNode;
            newLightNode.position = lightPositionDirection;
            newLightNode.color = newColor;
            newLightNode.type = lightNode.type;
            newLightNode.chunk = chunkDirection;

            chunkDirection.SetChunkTileColor(lightPositionDirection, newColor, false, lightNode.type);

            queue.Enqueue(newLightNode);
        }
    }
    #endregion



    #region Remove light
    /// <summary>
    /// Removes all light of the given LightSource.
    /// This does not delete its gameObject. Use RemoveLightSource() instead, since that combines both.
    /// </summary>
    public void RemoveLight()
    {
        state = LightSourceState.REMOVE;

        ClearCachedData();

        // Create a struct as lightweight data storage per block
        LightNode lightNode;
        lightNode.position = Position;
        lightNode.type = LightType;
        lightNode.chunk = chunkLoadManager.GetChunk(Position);
        if (lightNode.chunk == null)
            return;

        /* If there are many LightSources close together, the color of the LightSource is drowned out.
         * To correctly remove this enough, instead of using the light's color to remove, use the color 
         * of the LightSource's tile if that color is greater. */
        Color currentColor = lightNode.chunk.GetChunkTileBlendedColor(Position, dayNight: false);
        lightNode.color = new Color(
            Mathf.Max(currentColor.r, LightColor.r * LightStrength),
            Mathf.Max(currentColor.g, LightColor.g * LightStrength),
            Mathf.Max(currentColor.b, LightColor.b * LightStrength));
        lightNode.chunk.SetChunkTileColor(Position, Color.black, false, LightType);

        // Remove the actual light
        queueLightRemovalPasses.Enqueue(lightNode);
        PerformLightRemovalPasses(queueLightRemovalPasses);

        /* If we touched LightSources during removal spreading, we completely drowned out their color.
         * In order to correctly fill in the void, we need to update these lights. Create a new list
         * of them first to prevent problems from collection changes to removalLights somewhere else. */
        List<LightSource> voidLights = new List<LightSource>(removalLights);
        foreach (LightSource lightSource in voidLights)
            if (lightSource != null && lightSource != this && lightSource.state == LightSourceState.IDLE)
                lightSource.UpdateLight();

        state = LightSourceState.IDLE;
    }

    /// <summary>
    /// Removes all queued nodes' light channels.
    /// </summary>
    /// <param name="queue"></param>
    private void PerformLightRemovalPasses(Queue<LightNode> queue)
    {
        /* Generate a backup queue to refill the original queue after each channel
         * (since every channel execution empties the queue).*/
        Queue<LightNode> queueBackup = new Queue<LightNode>(queue);

        // Remove all light from the given channel
        while (queue.Count > 0)
            ExecuteLightingRemovalPass(queue, LightingChannelMode.RED);
        // Spread stronger surrounding channel light that should fill this void. For example from another light source nearby
        RemoveInvalidSpreadPositions();
        PerformLightPasses(queueLightPasses, redChannel: true, greenChannel: false, blueChannel: false);

        // Repeat for other channels
        removalPositions.Clear();
        queueLightPasses.Clear();
        queue = new Queue<LightNode>(queueBackup);
        while (queue.Count > 0)
            ExecuteLightingRemovalPass(queue, LightingChannelMode.GREEN);
        RemoveInvalidSpreadPositions();
        PerformLightPasses(queueLightPasses, redChannel: false, greenChannel: true, blueChannel: false);

        removalPositions.Clear();
        queueLightPasses.Clear();
        queue = new Queue<LightNode>(queueBackup);
        while (queue.Count > 0)
            ExecuteLightingRemovalPass(queue, LightingChannelMode.BLUE);
        RemoveInvalidSpreadPositions();
        PerformLightPasses(queueLightPasses, redChannel: false, greenChannel: false, blueChannel: true);
    }

    /// <summary>
    /// Removes invalid positions from the local light pass queue of this light.
    /// 
    /// Sometimes while removing light, we need to fill in the void we leave behind with light from stronger lit
    /// tiles we come across. Due to falloff differences of the front and back layer, this sometimes targets tiles
    /// within its own removal, which results in spreading light from the light source we are actually trying to remove.
    /// 
    /// Checks whether collected nodes are also part of the positions we visited while removing light. If not, that node's
    /// light is external which is what we are looking for.
    /// </summary>
    private void RemoveInvalidSpreadPositions()
    {
        queueRemoveInvalidPositions = new Queue<LightNode>(queueLightPasses);
        queueLightPasses.Clear();
        foreach (LightNode lightNode in queueRemoveInvalidPositions)
            if (!removalPositions.Contains(lightNode.position))
                queueLightPasses.Enqueue(lightNode);
    }
    #endregion

    #region Remove light smooth
    /// <summary>
    /// Removes all light of the given LightSource smoothly.
    /// This does not delete its gameObject. Use RemoveLightSource() instead, since that combines both.
    /// 
    /// NOTE: Removing multiple lights smoothly at the same time and close together may cause artifacts. Use with care.
    /// </summary>
    public IEnumerator RemoveLightSmooth()
    {
        state = LightSourceState.REMOVE;

        ClearCachedData();

        // Create a struct as lightweight data storage per block
        LightNode lightNode;
        lightNode.position = Position;
        lightNode.type = LightType;
        lightNode.chunk = chunkLoadManager.GetChunk(Position);
        if (lightNode.chunk == null)
            yield break;

        /* If there are many LightSources close together, the color of the LightSource is drowned out.
         * To correctly remove this enough, instead of using the light's color to remove, use the color 
         * of the LightSource's tile if that color is greater. */
        Color currentColor = lightNode.chunk.GetChunkTileBlendedColor(Position);
        lightNode.color = new Color(
            Mathf.Max(currentColor.r, LightColor.r * LightStrength),
            Mathf.Max(currentColor.g, LightColor.g * LightStrength),
            Mathf.Max(currentColor.b, LightColor.b * LightStrength));
        lightNode.chunk.SetChunkTileColor(Position, Color.black, false, LightType);

        // Remove the actual light
        queueLightRemovalPasses.Enqueue(lightNode);
        yield return StartCoroutine(PerformLightRemovalPassesSmooth(queueLightRemovalPasses));

        /* If we touched LightSources during removal spreading, we completely drowned out their color.
         * In order to correctly fill in the void, we need to update these lights. Create a new list
         * of them first to prevent problems from collection changes to removalLights somewhere else. */
        List<LightSource> voidLights = new List<LightSource>(removalLights);
        foreach (LightSource lightSource in voidLights)
            if (lightSource != null && lightSource != this && lightSource.state == LightSourceState.IDLE)
                yield return StartCoroutine(lightSource.UpdateLightSmooth());

        state = LightSourceState.IDLE;
    }

    /// <summary>
    /// Removes all queued nodes' light channels smoothly.
    /// </summary>
    /// <param name="queue"></param>
    private IEnumerator PerformLightRemovalPassesSmooth(Queue<LightNode> queue)
    {
        /* Generate a backup queue to refill the original queue after each channel
         * (since every channel execution empties the queue).*/
        Queue<LightNode> queueBackup = new Queue<LightNode>(queue);


        // Remove all light from the given channel
        while (queue.Count > 0)
        {
            ExecuteLightingRemovalPass(queue, LightingChannelMode.RED);

            if (PerformanceManager.Instance.HitFrameLimit())
                yield return null;
        }
        // Spread stronger surrounding channel light that should fill this void. For example from another light source nearby
        yield return StartCoroutine(RemoveInvalidSpreadPositionsSmooth());
        yield return StartCoroutine(PerformLightPassesSmooth(queueLightPasses, redChannel: true, greenChannel: false, blueChannel: false));

        // Repeat for other channels
        removalPositions.Clear();
        queueLightPasses.Clear();
        queue = new Queue<LightNode>(queueBackup);
        while (queue.Count > 0)
        {
            ExecuteLightingRemovalPass(queue, LightingChannelMode.GREEN);

            if (PerformanceManager.Instance.HitFrameLimit())
                yield return null;
        }
        yield return StartCoroutine(RemoveInvalidSpreadPositionsSmooth());
        yield return StartCoroutine(PerformLightPassesSmooth(queueLightPasses, redChannel: false, greenChannel: true, blueChannel: false));

        removalPositions.Clear();
        queueLightPasses.Clear();
        queue = new Queue<LightNode>(queueBackup);
        while (queue.Count > 0)
        {
            ExecuteLightingRemovalPass(queue, LightingChannelMode.BLUE);

            if (PerformanceManager.Instance.HitFrameLimit())
                yield return null;
        }
        yield return StartCoroutine(RemoveInvalidSpreadPositionsSmooth());
        yield return StartCoroutine(PerformLightPassesSmooth(queueLightPasses, redChannel: false, greenChannel: false, blueChannel: true));
    }

    /// <summary>
    /// Removes invalid positions from the local light pass queue of this light smoothly.
    /// 
    /// Sometimes while removing light, we need to fill in the void we leave behind with light from stronger lit
    /// tiles we come across. Due to falloff differences of the front and back layer, this sometimes targets tiles
    /// within its own removal, which results in spreading light from the light source we are actually trying to remove.
    /// 
    /// Checks whether collected nodes are also part of the positions we visited while removing light. If not, that node's
    /// light is external which is what we are looking for.
    /// </summary>
    /// <returns></returns>
    private IEnumerator RemoveInvalidSpreadPositionsSmooth()
    {
        queueRemoveInvalidPositions = new Queue<LightNode>(queueLightPasses);
        queueLightPasses.Clear();
        foreach (LightNode lightNode in queueRemoveInvalidPositions)
        {
            if (!removalPositions.Contains(lightNode.position))
                queueLightPasses.Enqueue(lightNode);

            if (PerformanceManager.Instance.HitFrameLimit())
                yield return null;
        }
    }
    #endregion

    #region Light removal passes
    /// <summary>
    /// Removes the given channel light of all queued nodes in 4 directions.
    /// This is one pass that scans left, down, right and up in that order, so
    /// at most 4 tiles are affected by a single pass.
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="mode"></param>
    private void ExecuteLightingRemovalPass(Queue<LightNode> queue, LightingChannelMode mode)
    {
        // Get the LightNode that's first in line
        LightNode light = queue.Dequeue();

        /* Detect passing over LightSources, while removing, to update them later. When we touch
         * such a LightSource, it means we completely drowned out its color and we need to
         * update the light again to fill in the blanks correctly. */
        LightSource lightSource = lightingManager.GetLightSource(light.position);
        if (lightSource != null && !removalLights.Contains(lightSource))
            removalLights.Add(lightSource);

        // Track removed positions
        removalPositions.Add(light.position);

        /* Obtain light values from the corresponding channel to lessen overhead
         * on extension passes. */
        switch (mode)
        {
            case LightingChannelMode.RED:
                if (light.color.r <= 0f)
                    return;
                passLightValue = light.color.r;
                break;
            case LightingChannelMode.GREEN:
                if (light.color.g <= 0f)
                    return;
                passLightValue = light.color.g;
                break;
            case LightingChannelMode.BLUE:
                if (light.color.b <= 0f)
                    return;
                passLightValue = light.color.b;
                break;
            default:
                return;
        }

        // Try and spread the light removal
        ExtendQueueLightRemovalPass(queue, light, passLightValue, vectorLeft, mode);
        ExtendQueueLightRemovalPass(queue, light, passLightValue, vectorDown, mode);
        ExtendQueueLightRemovalPass(queue, light, passLightValue, vectorRight, mode);
        ExtendQueueLightRemovalPass(queue, light, passLightValue, vectorUp, mode);
    }


    /// <summary>
    /// Try to extend the removal of light in the given direction.
    /// </summary>
    /// <param name="queue"></param>
    /// <param name="light"></param>
    /// <param name="lightValue"></param>
    /// <param name="direction"></param>
    /// <param name="mode"></param>
    private void ExtendQueueLightRemovalPass(Queue<LightNode> queue, LightNode light, float lightValue,
        Vector3Int direction, LightingChannelMode mode)
    {
        /* Get the right chunk for this position.
         * We don't want to use GetChunk on every check, just the ones where we enter a new chunk. */
        chunkDirection = light.chunk;
        newChunkPosition = new Vector3Int(
            (light.position.x + direction.x) / generationManager.chunkSize,
            (light.position.y + direction.y) / generationManager.chunkSize, 0);
        if (newChunkPosition != light.chunk.ChunkPosition)
            chunkDirection = chunkLoadManager.GetChunk(light.position + direction);
        if (chunkDirection == null)
            return;

        currentColor = chunkDirection.GetChunkTileColor(light.position + direction, light.type, chunkDirection.IsUnloading);
        lightValueDirection =
            (mode == LightingChannelMode.RED ?
                currentColor.r :
                (mode == LightingChannelMode.GREEN ?
                    currentColor.g :
                    currentColor.b));

        if (lightValueDirection > 0f)
        {
            // Continue removing and extending while the block I'm looking at has a lower lightValue for this channel
            if (lightValueDirection < lightValue)
            {
                Color newColor =
                    (mode == LightingChannelMode.RED ?
                        new Color(0f, currentColor.g, currentColor.b) :
                        (mode == LightingChannelMode.GREEN ?
                            new Color(currentColor.r, 0f, currentColor.b) :
                            new Color(currentColor.r, currentColor.g, 0f)));

                LightNode lightRemovalNode;
                lightRemovalNode.position = light.position + direction;
                lightRemovalNode.color = currentColor;
                lightRemovalNode.type = light.type;
                lightRemovalNode.chunk = chunkDirection;

                chunkDirection.SetChunkTileColor(light.position + direction, newColor, false, light.type, chunkDirection.IsUnloading);

                queue.Enqueue(lightRemovalNode);
            }
            /* I just found a tile with a higher lightValue for this channel which means another strong light source
             * must be nearby. Add tile to the update queue and spread their light after all removal to fill in the blanks 
             * this removal leaves behind.
             *   
             * Because we switch between two different falloff rates, this sometimes targets tiles within its own
             * light. These are later filtered out before spreading the light (using removalPositions). */
            else
            {
                LightNode lightNode;
                lightNode.position = light.position + direction;
                lightNode.color = currentColor;
                lightNode.type = light.type;
                lightNode.chunk = chunkDirection;
                queueLightPasses.Enqueue(lightNode);
            }
        }
    }
    #endregion
}